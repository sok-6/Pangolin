using Pangolin.Core.DataValueImplementations;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.Test
{
    public static class Extensions
    {
        public static void CompareTo(this ArrayValue arrayValue, params DataValue[] comparison)
        {
            arrayValue.Value.Count.ShouldBe(comparison.Length);

            for (int i = 0; i < arrayValue.Value.Count; i++)
            {
                arrayValue.Value[i].ShouldBe(comparison[i]);
            }
        }

        public static void CompareTo(this ArrayValue arrayValue, params decimal[] comparison)
        {
            arrayValue.Value.Count.ShouldBe(comparison.Length);

            for (int i = 0; i < arrayValue.Value.Count; i++)
            {
                arrayValue.Value[i].ShouldBeOfType<NumericValue>().Value.ShouldBe(comparison[i]);
            }
        }

        public static void CompareTo(this ArrayValue arrayValue, params string[] comparison)
        {
            arrayValue.Value.Count.ShouldBe(comparison.Length);

            for (int i = 0; i < arrayValue.Value.Count; i++)
            {
                arrayValue.Value[i].ShouldBeOfType<StringValue>().Value.ShouldBe(comparison[i]);
            }
        }
    }
}
