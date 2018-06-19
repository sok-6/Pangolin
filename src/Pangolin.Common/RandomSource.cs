using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Common
{
    public static class RandomSource
    {
        private static Random _rnd = new Random();

        public static int IntBetweenZeroAnd(int x)
        {
            if (x == 0)
            {
                return 0;
            }
            else
            {
                return Math.Sign(x) * _rnd.Next(Math.Abs(x));
            }
        }

        public static double RandomDouble()
        {
            return _rnd.NextDouble();
        }

        public static T Choose<T>(IEnumerable<T> collection)
        {
            if (collection.Count() == 0)
            {
                throw new PangolinException("Can't choose element from empty collection");
            }

            return collection.Skip(_rnd.Next(collection.Count())).First();
        }
    }
}
