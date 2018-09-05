using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class SquareRoot : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u221A";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            if (arg.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)arg;

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
                throw GetInvalidArgumentTypeException(ToString(), arg.Type);
            }
        }
    }

    public class Root : IterableToken
    {
        public override int Arity => 2;
        public override string ToString() => "\u1E5A";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg1 = arguments[0];
            var arg2 = arguments[1];

            // Only defined for numerics
            if (arg1.Type != DataValueType.Numeric || arg2.Type != DataValueType.Numeric)
            {
                throw GetInvalidArgumentTypeException(nameof(Root), arg1.Type, arg2.Type);
            }

            var exponent = 1 / (((NumericValue)arg1).Value);
            var theBase = ((NumericValue)arg2).Value;

            return new NumericValue(Math.Pow(theBase, exponent));
        }
    }

    public class Power_RepeatedCartesianProduct : IterableToken
    {
        public override int Arity => 2;
        public override string ToString() => "^";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg1 = arguments[0];
            var arg2 = arguments[1];

            // First argument (exponent) must be numeric
            if (arg1.Type != DataValueType.Numeric)
            {
                throw GetInvalidArgumentTypeException(nameof(Power_RepeatedCartesianProduct), arg1.Type, arg2.Type);
            }

            var numericArg1 = (NumericValue)arg1;

            // 2nd argument numeric, exponentiation
            if (arg2.Type == DataValueType.Numeric)
            {
                var numericArg2 = (NumericValue)arg2;

                if (numericArg1.Value == 0 && numericArg2.Value == 0)
                {
                    throw new PangolinException("Can't raise 0 to the power 0");
                }

                return new NumericValue(Math.Pow(((NumericValue)arg2).Value, numericArg1.Value));
            }
            else
            {
                if (!numericArg1.IsIntegral)
                {
                    throw new PangolinException($"Can't perform repeated cartesian product a non-integral number of times - repeat={arg1}, set={arg2}");
                }
                if (numericArg1.Value < 0)
                {
                    throw new PangolinException($"Can't perform repeated cartesian product a negative number of times - repeat={arg1}, set={arg2}");
                }

                var repeatCount = numericArg1.IntValue;

                // If repeating 0 times, empty version of correct type
                if (repeatCount == 0)
                {
                    return new ArrayValue(arg2.Type == DataValueType.String 
                        ? (DataValue)(new StringValue()) 
                        : (DataValue)(new ArrayValue()));
                }

                IEnumerable<IEnumerable<DataValue>> result = null;

                var elements = arg2.IterationValues;

                for (int i = 0; i < numericArg1.IntValue; i++)
                {
                    result = (i == 0) 
                        ? elements.Select(e => new DataValue[] { e }) 
                        : result.SelectMany(r => elements.Select(e => r.Append(e)));
                }

                return new ArrayValue(result.Select(r => DataValueSetToStringOrArray(r, arg2.Type)));
            }
        }
    }
}
