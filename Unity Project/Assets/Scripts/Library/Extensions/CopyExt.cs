using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace System
{
    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        public static Object Copy(this Object originalObject)
        {
            return InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
        }
        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            if (visited.ContainsKey(originalObject)) return visited[originalObject];
            var cloneObject = CloneMethod.Invoke(originalObject, null);

            visited.Add(originalObject, cloneObject);

            //this doesn't detect classes that extend dictionary (I tried other methods, and a couple worked but then broke other aspects)
            if ((typeToReflect.IsGenericType && typeToReflect.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
            {
                cloneObject = CopyDictionary(originalObject, visited, typeToReflect);
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
                return cloneObject;
            }
            else if (typeToReflect.IsSubclassOfGeneric(typeof(Dictionary<,>)))
            {
                cloneObject = CopyDictionary(originalObject, visited, typeToReflect);
                CopyFields(originalObject, visited, cloneObject, typeToReflect);
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
                return cloneObject;
            }
            if (typeToReflect.IsArray) // this grabs arrays and copies the elements into a new one
            {
                var arrayType = typeToReflect.GetElementType();
                if (IsPrimitive(arrayType) == false)
                {
                    Array arrayObject = (Array)(originalObject);
                    Array temp = Array.CreateInstance(arrayType, arrayObject.Length);
                    for (int i = 0; i < arrayObject.Length; ++i)
                    {
                        temp.SetValue(InternalCopy(arrayObject.GetValue(i), visited), i);
                    }
                    cloneObject = temp; //reroutes the visited reference to this and continues for base private fields
                }
            }
            CopyFields(originalObject, visited, cloneObject, typeToReflect);
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);

            return cloneObject;
        }

        private static object CopyDictionary(Object originalObject, IDictionary<Object, Object> visited, Type typeToReflect)
        {
            var dict = Activator.CreateInstance(typeToReflect);
            Type[] types = GetDictionaryTypes(typeToReflect);

            MethodInfo GetItem = typeToReflect.GetMethod("get_Item", new Type[] { types[0] });
            MethodInfo Add = typeToReflect.GetMethod("Add", new Type[] { types[0], types[1] });

            IEnumerable keys = (IEnumerable)typeToReflect.GetProperty("Keys").GetValue(originalObject, null);
            foreach (object key in keys)
            {
                object[] arguments = new object[] { key };
                object value = GetItem.Invoke(originalObject, arguments);
                if (value != null)
                {
                    Add.Invoke(dict, new object[] { InternalCopy(key, visited), InternalCopy(value, visited) });
                }
            }
            return dict;
        }

        private static Type[] GetDictionaryTypes(Type t)
        {
            if (!(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Dictionary<,>)) && t.BaseType != null)
            {
                return GetDictionaryTypes(t.BaseType);
            }
            else
            {
                return t.GetGenericArguments();
            }
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null && !(typeToReflect.BaseType.IsGenericType && typeToReflect.BaseType.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
            {
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect.BaseType);
                CopyFields(originalObject, visited, cloneObject, typeToReflect.BaseType, BindingFlags.Instance | BindingFlags.NonPublic, info => info.IsPrivate);
            }
        }

        private static void CopyFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy, Func<FieldInfo, bool> filter = null)
        {
            foreach (FieldInfo fieldInfo in typeToReflect.GetFields(bindingFlags))
            {
                if (filter != null && filter(fieldInfo) == false) continue;
                if (IsPrimitive(fieldInfo.FieldType)) continue;
                var originalFieldValue = fieldInfo.GetValue(originalObject);
                var clonedFieldValue = originalFieldValue == null ? null : InternalCopy(originalFieldValue, visited);
                fieldInfo.SetValue(cloneObject, clonedFieldValue);
            }
        }
        public static T Copy<T>(this T original)
        {
            return (T)Copy((Object)original);
        }
    }

    public class ReferenceEqualityComparer : EqualityComparer<Object>
    {
        public override bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }
        public override int GetHashCode(object obj)
        {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }
    }
}