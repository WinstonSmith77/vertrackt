using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertrackt
{
    public static class Helpers
    {
        public static TV GetValueOrCreateType<TK,TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TV> createValue )
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
    }
}
