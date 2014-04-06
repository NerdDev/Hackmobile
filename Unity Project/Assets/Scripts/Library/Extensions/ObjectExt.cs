using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Collections;
using UnityEngine;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace System
{
    public static class ObjectExt
    {
        public static Type GetEnumeratedType<T>(this IComparable<T> _)
        {
            return typeof(T);
        }

        public static bool IsNotAFreaking<T>(this object obj)
        {
            return !(obj is T);
        }

        public static string Dump(this object element)
        {
            return Dump(element, 2, new List<string>());
        }

        public static string Dump(this object element, List<string> filter)
        {
            return Dump(element, 2, filter);
        }

        public static string Dump(this object element, int indentSize, List<string> filter)
        {
            return ObjDump.Dump(element, indentSize, filter);
        }

        public static IEnumerable<KeyValuePair<string, string>> GetGUIDisplays(this Object o)
        { // Not implemented
            return new List<KeyValuePair<string, string>>();
        }

        /*
        public static int GetHash(this Object o) 
        {
            Type type = o.GetType();
            if (IsPrimitive(type)) return o.GetHashCode();
            else return RecursiveHashFields(o, type);
        }

        private static int RecursiveHashFields(object originalObject, Type typeToReflect)
        {
            int hash = 3;
            if (typeToReflect.BaseType != null && typeToReflect.BaseType != typeof(Object))
            {
                hash += RecursiveHashFields(originalObject, typeToReflect.BaseType);
            }
            hash += GetInternalHash(originalObject, typeToReflect, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, info => !info.IsPrivate);
            return hash;
        }

        private static int GetInternalHash(object originalObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null, Func<PropertyInfo, bool> propFilter = null)
        {
            int hash = 7;
            int[] hashMultipliers = { 2, 3, 5, 7, 9, 13, 17 };
                int i = 0;
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (fieldInfo.IsDefined(typeof(NotHashable), true)) continue;
                if (filter != null && filter(fieldInfo) == false) continue;
                
                if (!IsPrimitive(fieldInfo.FieldType))
                {
                    var originalFieldValue = fieldInfo.GetValue(originalObject);
                    if (originalFieldValue == null) continue;
                    hash += originalFieldValue.GetHash() * hashMultipliers[i];
                }
                else
                {
                    var originalFieldValue = fieldInfo.GetValue(originalObject);
                    if (originalFieldValue == null) continue;
                    hash += originalFieldValue.GetHashCode() * hashMultipliers[i];
                }
                i++;
                if (i >= hashMultipliers.Length) i = 0;
            }
            foreach (PropertyInfo propInfo in typeToReflect.GetProperties(bindingFlags))
            {
                if (propInfo.IsDefined(typeof(NotHashable), true)) continue;
                if (propFilter != null && propFilter(propInfo) == false) continue;

                if (!IsPrimitive(propInfo.PropertyType))
                {
                    var originalFieldValue = propInfo.GetValue(originalObject, null);
                    if (originalFieldValue == null) continue;
                    hash += originalFieldValue.GetHash() * hashMultipliers[i];
                }
                else
                {
                    var originalFieldValue = propInfo.GetValue(originalObject, null);
                    if (originalFieldValue == null) continue;
                    hash += originalFieldValue.GetHashCode() * hashMultipliers[i];
                }
                i++;
                if (i >= hashMultipliers.Length) i = 0;
            }
            return hash;
        }

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        */

        public static List<T> FindAllDerivedObjects<T>(this Object obj, bool recursive = true)
        {
            List<T> ret = new List<T>();
            if (obj != null)
            {
                Type objType = obj.GetType();
                if (!objType.IsPrimitive())
                {
                    Type target = typeof(T);
                    var hashSet = new HashSet<object>(new ReferenceEqualityComparer());
                    hashSet.Add(obj);
                    if (target.IsAssignableFrom(objType))
                    {
                        ret.Add((T)obj);
                    }
                    ret.AddRange(FindAllDerivedObjects<T>(obj, obj.GetType(), target, hashSet, recursive));
                }
            }
            return ret;
        }

        private static List<T> FindAllDerivedObjects<T>(Object obj, Type objType, Type target, HashSet<Object> set, bool recursive)
        {
            List<T> ret = new List<T>();
            foreach (var field in objType.GetFields())
            {
                Type fieldType = field.FieldType;
                if (!fieldType.IsPrimitive())
                {
                    object fieldObj = field.GetValue(obj);
                    if (fieldObj == null) continue;
                    fieldType = fieldObj.GetType();
                    set.Add(obj);
                    if (target.IsAssignableFrom(fieldType))
                    {
                        ret.Add((T)fieldObj);
                    }
                    if (recursive)
                    {
                        ret.AddRange(FindAllDerivedObjects<T>(fieldObj, fieldType, target, set, true));
                    }
                }
            }
            return ret;
        }
    }

    public class ObjDump
    {
        private int depth;
        private readonly int indent;
        private readonly StringBuilder stringBuilder;
        private readonly List<int> foundElements;
        private List<string> filter;

        private ObjDump(int indentSize, List<string> filter)
        {
            this.filter = filter;
            indent = indentSize;
            stringBuilder = new StringBuilder();
            foundElements = new List<int>();
        }

        public static string Dump(object element)
        {
            return Dump(element, 2, new List<string>());
        }

        public static string Dump(object element, List<string> filter)
        {
            return Dump(element, 2, filter);
        }

        public static string Dump(object element, int indentSize, List<string> filter)
        {
            var instance = new ObjDump(indentSize, filter);
            return instance.DumpElement(element);
        }

        private string DumpElement(object element)
        {
            if (element == null || element is ValueType || element is string)
            {
                Write(FormatValue(element));
            }
            //yeah, this is crude, but it works cleanly enough
            else if (filter.Contains(element.GetType().Name))
            {
                Write("filtered");
            }
            else
            {
                var objectType = element.GetType();
                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    Write("{{{0}}}", objectType.FullName);
                    foundElements.Add(element.GetHashCode());
                    depth++;
                }

                var enumerableElement = element as IEnumerable;
                if (enumerableElement != null)
                {
                    try
                    {
                        foreach (object item in enumerableElement)
                        {
                            if (item is IEnumerable && !(item is string))
                            {
                                depth++;
                                DumpElement(item);
                                depth--;
                            }
                            else
                            {
                                if (!AlreadyTouched(item))
                                    DumpElement(item);
                                else
                                    Write("{{{0}}} <-- bidirectional reference found", item.GetType().FullName);
                            }
                        }
                    }
                    catch
                    {
                        Write("null");
                    }

                }
                else
                {
                    MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                    foreach (var memberInfo in members)
                    {
                        var fieldInfo = memberInfo as FieldInfo;
                        var propertyInfo = memberInfo as PropertyInfo;

                        if (fieldInfo == null && propertyInfo == null)
                        {
                            continue;
                        }

                        Type type = null;
                        object value = null;
                        if (fieldInfo != null)
                        {
                            type = fieldInfo.FieldType;
                            value = fieldInfo.GetValue(element);
                        }
                        else
                        {
                            type = propertyInfo.PropertyType;
                            try
                            {
                                value = propertyInfo.GetValue(element, null);
                            }
                            catch
                            {
                                //Write("null");
                            }
                        }

                        //type = fieldInfo != null ? fieldInfo.FieldType : propertyInfo.PropertyType;
                        //object value = fieldInfo != null
                        //                   ? fieldInfo.GetValue(element)
                        //                   : propertyInfo.GetValue(element, null);

                        if (type.IsValueType || type == typeof(string))
                        {
                            Write("{0}: {1}", memberInfo.Name, FormatValue(value));
                        }
                        else
                        {
                            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
                            Write("{0}: {1}", memberInfo.Name, isEnumerable ? "..." : "{ }");

                            var alreadyTouched = !isEnumerable && AlreadyTouched(value);
                            depth++;
                            if (!alreadyTouched)
                            {
                                DumpElement(value);
                            }
                            else
                            {
                                Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName);
                            }
                            depth--;
                        }
                    }
                }

                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    depth--;
                }
            }

            return stringBuilder.ToString();
        }

        private bool AlreadyTouched(object value)
        {
            if (value == null)
            {
                return false;
            }
            var hash = value.GetHashCode();
            for (var i = 0; i < foundElements.Count; i++)
            {
                if (foundElements[i] == hash)
                {
                    return true;
                }
            }
            return false;
        }

        private void Write(string value, params object[] args)
        {
            string space = new string(' ', depth * indent);

            if (args != null)
            {
                value = string.Format(value, args);
            }

            stringBuilder.AppendLine(space + value);
        }

        private string FormatValue(object o)
        {
            if (o == null)
                return ("null");

            if (o is DateTime)
                return (((DateTime)o).ToShortDateString());

            if (o is string)
                return string.Format("\"{0}\"", o);

            if (o is ValueType)
                return (o.ToString());

            if (o is IEnumerable)
                return ("...");

            return ("{ }");
        }
    }

}
