﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Collections;
using UnityEngine;
using System.ComponentModel;
using System.IO;

namespace System
{
    public static class ObjectExt
    {
        public static Type GetEnumeratedType<T>(this IEnumerable<T> _)
        {
            return typeof(T);
        }

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