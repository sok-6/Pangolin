using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Length : Token
    {
        public override int Arity => 1;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get argument
            var arg = programState.DequeueAndEvaluate();

            if (arg.Type == DataValueType.Numeric)
            {
                // Numeric, return absolute value
                var numericArg = arg as NumericValue;
                return new NumericValue(Math.Abs(numericArg.Value));
            }
            else if (arg.Type == DataValueType.String)
            {
                // String, get value's length
                var stringArg = arg as StringValue;
                return new NumericValue(stringArg.Value.Length);
            }
            else
            {
                // Array, get value's length
                var arrayArg = arg as ArrayValue;
                return new NumericValue(arrayArg.Value.Count);
            }
        }

        public override string ToString() => "L";
    }
}
