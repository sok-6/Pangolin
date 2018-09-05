using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class AllButFirst_ModTen : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "(";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

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

    public class AllButLast_LogTen : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => ")";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

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

    public class Index : IterableToken
    {
        public override int Arity => 2;
        public override string ToString() => "@";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg1 = arguments[0];
            var arg2 = arguments[1];

            // One must be numeric, one must be iterable
            if ((arg1.Type != DataValueType.Numeric && arg2.Type != DataValueType.Numeric) 
                || (arg1.Type == DataValueType.Numeric && arg2.Type == DataValueType.Numeric))
            {
                throw GetInvalidArgumentTypeException(nameof(Index), arg1.Type, arg2.Type);
            }
            
            var numericArg = (arg1 as NumericValue) ?? (NumericValue)arg2;
            var set = (arg1.Type == DataValueType.Numeric ? arg2 : arg1).IterationValues;

            if (!numericArg.IsIntegral)
            {
                throw new PangolinException($"{nameof(Index)} numeric argument must be integral - arg1={arg1}, arg2={arg2}");
            }

            if (set.Count == 0)
            {
                throw new PangolinException($"{nameof(Index)} iterable argument must not be empty - arg1={arg1}, arg2={arg2}");
            }

            return set[((numericArg.IntValue % set.Count) + set.Count) % set.Count];
        }
    }
}
