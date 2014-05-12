using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace System
{
    public static class ObjectExtensions
    {
        private static readonly MethodInfo CloneMethod = typeof(Object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly Type CopyType = typeof(Copyable);
        private static readonly Type DictionaryType = typeof(Dictionary<,>);
        private static readonly Type ActionType = typeof(Action<>);
        private static readonly Type FuncType = typeof(Func<,>);

        public static bool IsPrimitive(this Type type)
        {
            if (type == typeof(String)) return true;
            return (type.IsValueType & type.IsPrimitive);
        }

        public static Object Copy(this Object originalObject)
        {
            Object o = InternalCopy(originalObject, new Dictionary<Object, Object>(new ReferenceEqualityComparer()));
            return o;
        }
        private static Object InternalCopy(Object originalObject, IDictionary<Object, Object> visited)
        {
            if (originalObject == null) return null;
            var typeToReflect = originalObject.GetType();
            if (IsPrimitive(typeToReflect)) return originalObject;
            Object visitedObj;
            if (visited.TryGetValue(originalObject, out visitedObj))
            {
                return visitedObj;
            }

            object cloneObject = null;
            if ((typeToReflect.IsGenericType && typeToReflect.GetGenericTypeDefinition() == DictionaryType))
            {
                cloneObject = CopyDictionary(originalObject, visited, typeToReflect);
                visited.Add(originalObject, cloneObject);
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            }
            else if (typeToReflect.IsSubclassOfGeneric(DictionaryType))
            {
                cloneObject = CopyDictionary(originalObject, visited, typeToReflect); ;
                visited.Add(originalObject, cloneObject);
                CopyFields(originalObject, visited, cloneObject, typeToReflect);
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            }
            else if (typeToReflect.IsArray) // this grabs arrays and copies the elements into a new one
            {
                var arrayType = typeToReflect.GetElementType();
                Array arrayObject = (Array)(originalObject);
                Array temp = Array.CreateInstance(arrayType, arrayObject.Length);
                for (int i = 0; i < arrayObject.Length; ++i)
                {
                    temp.SetValue(InternalCopy(arrayObject.GetValue(i), visited), i);
                }
                cloneObject = temp;
                visited.Add(originalObject, cloneObject);
                CopyFields(originalObject, visited, cloneObject, typeToReflect);
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            }
            else
            {
                cloneObject = CloneMethod.Invoke(originalObject, null);
                ICopyable copyable = cloneObject as ICopyable;
                if (copyable != null)
                {
                    copyable.PostPrimitiveCopy();
                }
                visited.Add(originalObject, cloneObject);
                CopyFields(originalObject, visited, cloneObject, typeToReflect);
                RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
                if (copyable != null)
                {
                    copyable.PostObjectCopy();
                }
            }
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
            if (!(t.IsGenericType && t.GetGenericTypeDefinition() == DictionaryType) && t.BaseType != null)
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
            if (typeToReflect.BaseType != null && !(typeToReflect.BaseType.IsGenericType && typeToReflect.BaseType.GetGenericTypeDefinition() == DictionaryType))
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
                if (fieldInfo.IsDefined(CopyType, false))
                {
                    if (fieldInfo.FieldType.IsGenericType)
                    {
                        Type genericType = fieldInfo.FieldType.GetGenericTypeDefinition();
                        if (genericType == ActionType || genericType == FuncType) continue;
                    }
                    var originalFieldValue = fieldInfo.GetValue(originalObject);
                    var clonedFieldValue = originalFieldValue == null ? null : InternalCopy(originalFieldValue, visited);
                    fieldInfo.SetValue(cloneObject, clonedFieldValue);
                }
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