using System.Collections.Generic;
using System.Reflection;
using System.ArrayExtensions;
using System.Collections;

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
            /*
            //this doesn't detect classes that extend dictionary (I tried other methods, and a couple worked but then broke other aspects)
            if (typeToReflect.IsGenericType && typeToReflect.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                //was test code creating a separate generic type
                //Type keyType = typeToReflect.GetGenericArguments()[0];
                //Type valueType = typeToReflect.GetGenericArguments()[1];
                //Type dictType = typeToReflect.MakeGenericType(keyType, valueType);

                var dict = Activator.CreateInstance(typeToReflect);

                MethodInfo GetItem = typeToReflect.GetMethod("get_Item");
                MethodInfo Add = typeToReflect.GetMethod("Add", BindingFlags.Public | BindingFlags.Instance);

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
                cloneObject = dict; //reroutes the visited reference to this and continues for base private fields
                
            }
            */
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
            else //generic method for copying data on non-dictionarys and non-arrays
            {
                CopyFields(originalObject, visited, cloneObject, typeToReflect);
            }
            RecursiveCopyBaseTypePrivateFields(originalObject, visited, cloneObject, typeToReflect);
            return cloneObject;
        }

        private static void RecursiveCopyBaseTypePrivateFields(object originalObject, IDictionary<object, object> visited, object cloneObject, Type typeToReflect)
        {
            if (typeToReflect.BaseType != null)
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

    namespace ArrayExtensions
    {
        public static class ArrayExtensions
        {
            public static void ForEach(this Array array, Action<Array, int[]> action)
            {
                if (array.LongLength == 0) return;
                ArrayTraverse walker = new ArrayTraverse(array);
                do action(array, walker.Position);
                while (walker.Step());
            }
        }

        internal class ArrayTraverse
        {
            public int[] Position;
            private int[] maxLengths;

            public ArrayTraverse(Array array)
            {
                maxLengths = new int[array.Rank];
                for (int i = 0; i < array.Rank; ++i)
                {
                    maxLengths[i] = array.GetLength(i) - 1;
                }
                Position = new int[array.Rank];
            }

            public bool Step()
            {
                for (int i = 0; i < Position.Length; ++i)
                {
                    if (Position[i] < maxLengths[i])
                    {
                        Position[i]++;
                        for (int j = 0; j < i; j++)
                        {
                            Position[j] = 0;
                        }
                        return true;
                    }
                }
                return false;
            }
        }
    }

}