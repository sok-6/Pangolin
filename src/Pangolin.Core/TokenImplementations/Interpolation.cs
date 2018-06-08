using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Interpolation : Token
    {
        private const string PLACEHOLDER_CHARS = "\u24EA\u2460\u2461\u2462\u2463\u2464\u2465\u2466\u2467\u2468";

        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get two arguments
            var arg1 = programState.DequeueAndEvaluate();
            var arg2 = programState.DequeueAndEvaluate();

            // String interpolation
            if (arg1.Type == DataValueType.String)
            {
                var formatString = ((StringValue)arg1).Value;

                // If arg2 is not an array, arrayify it first
                IReadOnlyList<DataValue> interpolationValues =
                    arg2.Type == DataValueType.Array
                        ? ((ArrayValue)arg2).Value
                        : new List<DataValue>() { arg2 };

                // Replace each index in turn
                for (int i = 0; i < PLACEHOLDER_CHARS.Length; i++)
                {
                    formatString = formatString.Replace(
                        PLACEHOLDER_CHARS.Substring(i, 1),
                        interpolationValues[i % interpolationValues.Count].ToString());
                }

                return new StringValue(formatString);
            }
            else
            {
                // Interpolation not defined for other data types yet
                throw new PangolinException($"Interpolation not defined in case where 1st argument is not a string - arg1.Type={arg1.Type}");
            }
        }

        public override string ToString() => "$";
    }
}
