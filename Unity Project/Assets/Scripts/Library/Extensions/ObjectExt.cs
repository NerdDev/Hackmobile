using System;
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
    }
    public class ObjectDumper
    {

        public static string Write(object element)
        {
            return Write(element, 0);
        }

        public static string Write(object element, int depth)
        {
            
            ObjectDumper dumper = new ObjectDumper(depth);
            //dumper.writer = log;
            dumper.WriteObject(null, element);
            return writer.ToString();
        }

        static StringBuilder writer = new StringBuilder();
        int pos;
        int level;
        int depth;

        private ObjectDumper(int depth)
        {
            this.depth = depth;
        }

        private void Write(string s)
        {
            if (s != null)
            {
                writer.Append(s);
                pos += s.Length;
            }
        }

        private void WriteIndent()
        {
            for (int i = 0; i < level; i++) writer.Append("  ");
        }

        private void WriteLine()
        {
            writer.AppendLine();
            pos = 0;
        }

        private void WriteTab()
        {
            Write("  ");
            while (pos % 8 != 0) Write(" ");
        }

        private void WriteObject(string prefix, object element)
        {
            if (element == null || element is ValueType || element is string)
            {
                WriteIndent();
                Write(prefix);
                WriteValue(element);
                WriteLine();
            }
            else
            {
                IEnumerable enumerableElement = element as IEnumerable;
                if (enumerableElement != null)
                {
                    foreach (object item in enumerableElement)
                    {
                        if (item is IEnumerable && !(item is string))
                        {
                            WriteIndent();
                            Write(prefix);
                            Write("...");
                            WriteLine();
                            if (level < depth)
                            {
                                level++;
                                WriteObject(prefix, item);
                                level--;
                            }
                        }
                        else
                        {
                            WriteObject(prefix, item);
                        }
                    }
                }
                else
                {
                    getFromType(prefix, element);
                }
            }
        }

        private void getFromType(string prefix, object element)
        {
            MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
            WriteIndent();
            Write(prefix);
            bool propWritten = false;
            foreach (MemberInfo m in members)
            {
                FieldInfo f = m as FieldInfo;
                PropertyInfo p = m as PropertyInfo;
                if (f != null || p != null)
                {
                    if (propWritten)
                    {
                        WriteTab();
                    }
                    else
                    {
                        propWritten = true;
                    }
                    Write(m.Name);
                    Write("=");
                    Type t = f != null ? f.FieldType : p.PropertyType;
                    if (t.IsValueType || t == typeof(string))
                    {
                        WriteValue(f != null ? f.GetValue(element) : p.GetValue(element, null));
                    }
                    else
                    {
                        if (typeof(IEnumerable).IsAssignableFrom(t))
                        {
                            Write("{\n");
                            
                            Write("}\n");
                        }
                        else
                        {
                            Write("{ }");
                        }
                    }
                }
            }
            if (propWritten) WriteLine();
            if (level < depth)
            {
                foreach (MemberInfo m in members)
                {
                    FieldInfo f = m as FieldInfo;
                    PropertyInfo p = m as PropertyInfo;
                    if (f != null || p != null)
                    {
                        Type t = f != null ? f.FieldType : p.PropertyType;
                        if (!(t.IsValueType || t == typeof(string)))
                        {
                            object value = f != null ? f.GetValue(element) : p.GetValue(element, null);
                            if (value != null)
                            {
                                level++;
                                WriteObject(m.Name + ": ", value);
                                level--;
                            }
                        }
                    }
                }
            }
        }

        private void WriteValue(object o)
        {
            if (o == null)
            {
                Write("null");
            }
            else if (o is DateTime)
            {
                Write(((DateTime)o).ToShortDateString());
            }
            else if (o is ValueType || o is string)
            {
                Write(o.ToString());
            }
            else if (o is IEnumerable)
            {
                Write("...");
            }
            else
            {
                Write("{ }");
            }
        }
    }

    public class ObjectDumper2
    {
        private int _level;
        private readonly int _indentSize;
        private readonly StringBuilder _stringBuilder;
        private readonly List<int> _hashListOfFoundElements;
        private List<string> filter;

        private ObjectDumper2(int indentSize, List<string> filter)
        {
            this.filter = filter;
            _indentSize = indentSize;
            _stringBuilder = new StringBuilder();
            _hashListOfFoundElements = new List<int>();
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
            var instance = new ObjectDumper2(indentSize, filter);
            return instance.DumpElement(element);
        }

        private string DumpElement(object element)
        {
            if (element == null || element is ValueType || element is string)
            {
                Write(FormatValue(element));
            }
            else if (filter.Contains(element.GetType().Name))
            {
            }
            else
            {
                var objectType = element.GetType();
                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    Write("{{{0}}}", objectType.FullName);
                    _hashListOfFoundElements.Add(element.GetHashCode());
                    _level++;
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
                                _level++;
                                DumpElement(item);
                                _level--;
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
                    MemberInfo[] members = element.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var memberInfo in members)
                    {
                        var fieldInfo = memberInfo as FieldInfo;
                        var propertyInfo = memberInfo as PropertyInfo;

                        if (fieldInfo == null && propertyInfo == null)
                            continue;

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
                                Write("null");
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
                            _level++;
                            if (!alreadyTouched)
                                DumpElement(value);
                            else
                                Write("{{{0}}} <-- bidirectional reference found", value.GetType().FullName);
                            _level--;
                        }
                    }
                }

                if (!typeof(IEnumerable).IsAssignableFrom(objectType))
                {
                    _level--;
                }
            }

            return _stringBuilder.ToString();
        }

        private bool AlreadyTouched(object value)
        {
            if (value == null)
            {
                return false;
            }
            var hash = value.GetHashCode();
            for (var i = 0; i < _hashListOfFoundElements.Count; i++)
            {
                if (_hashListOfFoundElements[i] == hash)
                    return true;
            }
            return false;
        }

        private void Write(string value, params object[] args)
        {
            var space = new string(' ', _level * _indentSize);

            if (args != null)
                value = string.Format(value, args);

            _stringBuilder.AppendLine(space + value);
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