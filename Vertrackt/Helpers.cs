using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt
{
    public static class Helpers
    {
        public static TV GetValueOrCreateType<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TV> createValue)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary[key] = createValue();
            }

            return dictionary[key];
        }


        public static double DeltaAngle(double a, double b)
        {
            var delta = Math.Abs(a - b);
            return Math.Min(2 * Math.PI - delta, delta);
        }


        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> list, int parts)
        {
            return list.Select((item, index) => new { index, item })
                       .GroupBy(x => x.index % parts)
                       .Select(x => x.Select(y => y.item));
        }


        public static IEnumerable<IEnumerable<T>> SlicteToChunks<T>(this IEnumerable<T> list, int length)
        {
            return list
                .Select((item, index) => new { index, item })
                       .GroupBy(x => x.index / length)
                       .Select(x => x.Select(y => y.item));
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static bool IsApproxEqual(this double a, double b, double delta = 1e-5)
        {
            return Math.Abs(a - b) < delta;
        }

        public static T PeekCheckNull<T>(this Stack<T> stack)
        {
            return stack.Count > 0 ? stack.Peek() : default(T);
        }

    }

}
