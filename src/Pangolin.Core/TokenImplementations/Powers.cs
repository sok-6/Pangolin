using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class SquareRoot : ArityOneIterableToken
    {
        public override string ToString() => "\u221A";

        protected override DataValue EvaluateInner(DataValue value)
        {
            if (value.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)value;

                if (numericArg.Value < 0)
                {
                    throw new PangolinException($"Complex numbers not implemented yet, SquareRoot failed for negative value {numericArg.Value}");
                }
                else
                {
                    return new NumericValue(Math.Sqrt(numericArg.Value));
                }
            }
            else
            {
                throw GetInvalidArgumentTypeException(ToString(), value.Type);
            }
        }
    }
}
