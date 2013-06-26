using System;
using System.Collections.Generic;

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
}