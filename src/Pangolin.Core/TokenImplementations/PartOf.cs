using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class AllButFirst_ModTen : ArityOneIterableToken
    {
        public override string ToString() => "(";

        protected override DataValue EvaluateInner(DataValue arg)
        {
            // Numeric, mod 10 that bad boy
            if (arg.Type == DataValueType.Numeric)
            {
                return new NumericValue(((NumericValue)arg).Value % 10);
            }
            // Iterable, remove first
            else
            {
                var elements = arg.IterationValues;

                // If less than 2 elements, empty
                if (elements.Count < 2)
                {
                    return arg.Type == DataValueType.String ? (DataValue)(new StringValue()) : (DataValue)(new ArrayValue());
                }

                var trimmedElements = elements.Skip(1);

                return DataValueSetToStringOrArray(trimmedElements, arg.Type);
            }
        }
    }

    public class AllButLast_LogTen : ArityOneIterableToken
    {
        public override string ToString() => ")";

        protected override DataValue EvaluateInner(DataValue arg)
        {
            // Numeric, log 10
            if (arg.Type == DataValueType.Numeric)
            {
                return new NumericValue(Math.Log10(((NumericValue)arg).Value));
            }
            // Iterable, remove first
            else
            {
                var elements = arg.IterationValues;

                // If less than 2 elements, empty
                if (elements.Count < 2)
                {
                    return arg.Type == DataValueType.String ? (DataValue)(new StringValue()) : (DataValue)(new ArrayValue());
                }

                var trimmedElements = elements.Take(elements.Count - 1);

                return DataValueSetToStringOrArray(trimmedElements, arg.Type);
            }
        }
    }
}
