using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp
{
    public static class ListExtensions
    {
        private static readonly Random Rng = new Random(Environment.TickCount + "111222333sblg".GetHashCode());

        public static T PickOne<T>(this List<T> collection)
        {
            return collection[Rng.Next(collection.Count)];
        }

        public static T PickOne<T>(this T[] collection)
        {
            return collection[Rng.Next(collection.Length)];
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1) {
                n--;
                var k = Rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void RemoveOne<T>(this List<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var child = list[i];
                if (predicate(child))
                {
                    list.Remove(child);
                    return;
                }
            }
        }

        public static void EachToNext<T>(this List<T> list)
        {
            var last = list.Last();
            var slist = new List<T>(list);
            for (var i = 0; i < list.Count - 1; i++)
            {
                var item = slist[i];
                list[i + 1] = item;
            }

            list[0] = last;
        }
    }
}
