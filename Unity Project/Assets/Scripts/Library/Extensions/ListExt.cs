using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class ListExt
    {
        public static List<T> Populate<T>(this List<T> list, int num) where T : new()
        {
            for (int i = 0; i < num; i++)
            {
                list.Add(new T());
            }
            return list;
        }

        public static T Take<T>(this List<T> list)
        {
            T item = list[0];
            list.RemoveAt(0);
            return item;
        }

        public static T RandomTake<T>(this List<T> list, System.Random rand)
        {
            int r = rand.Next(list.Count);
            T item = list[r];
            list.RemoveAt(r);
            return item;
        }

        public static T Random<T>(this List<T> list, System.Random rand)
        {
            if (list.Count > 0)
                return list[rand.Next(list.Count)];
            return default(T);
        }
    }
}