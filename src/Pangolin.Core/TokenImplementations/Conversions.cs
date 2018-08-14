using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pangolin.Core.TokenImplementations
{
    public class Arrayify : ArityOneIterableToken
    {
        public override string ToString() => "A";

        protected override DataValue EvaluateInner(DataValue value)
        {
            // Get argument, return it wrapped in an array
            return new ArrayValue(value);
        }
    }

    public class ArrayPair : ArityTwoIterableToken
    {
        public override string ToString() => "]";

        protected override DataValue EvaluateInner(DataValue arg1, DataValue arg2)
        {
            return new ArrayValue(arg1, arg2);
        }
    }

    public class ArrayTriple : Token
    {
        public override int Arity => 3;

        public override DataValue Evaluate(ProgramState programState)
        {
            return new ArrayValue(
                programState.DequeueAndEvaluate(),
                programState.DequeueAndEvaluate(),
                programState.DequeueAndEvaluate());
        }

        public override string ToString() => "\u039E";
    }

    public class Elements : ArityOneIterableToken
    {
        public override string ToString() => "\u03B4";

        protected override DataValue EvaluateInner(DataValue arg)
        {
            // Numeric, get base 10 digits of +ve integral
            if (arg.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)arg;

                if (numericArg.Value < 0)
                {
                    throw new PangolinInvalidArgumentTypeException($"Invalid argument type passed to \u03B4 command - negative value: {numericArg.Value}");
                }
                if (!numericArg.IsIntegral)
                {
                    throw new PangolinInvalidArgumentTypeException($"Invalid argument type passed to \u03B4 command - non-integral value: {numericArg.Value}");
                }

                var result = new List<int>();
                var valueCopy = numericArg.IntValue;

                while (valueCopy > 0)
                {
                    result.Add(valueCopy % 10);
                    valueCopy /= 10;
                }

                result.Reverse();

                return new ArrayValue(result.Select(r => new NumericValue(r)));
            }
            // String, split into array of single character strings
            else if (arg.Type == DataValueType.String)
            {
                return new ArrayValue(arg.IterationValues);
            }
            // Array, no implemented outcome
            else
            {
                throw GetInvalidArgumentTypeException(ToString(), arg.Type);
            }
        }
    }

    public class Transform_Transpose : ArityOneIterableToken
    {
        public override string ToString() => "\u0393";

        protected override DataValue EvaluateInner(DataValue arg)
        {
            // Numeric, stringify
            if (arg.Type == DataValueType.Numeric)
            {
                return new StringValue(arg.ToString());
            }
            // String, parse as numeric
            else if (arg.Type == DataValueType.String)
            {
                var s = arg.ToString();
                if (!double.TryParse(s, out var value))
                {
                    throw new PangolinException($"Failed to parse string \"{s}\" as numeric");
                }

                return new NumericValue(value);
            }
            // Array, transpose with truncation
            else
            {
                var elements = ((ArrayValue)arg).Value;

                // If any numerics, can't transpose
                if (elements.Any(e => e.Type == DataValueType.Numeric))
                {
                    throw new PangolinInvalidArgumentTypeException($"{nameof(Transform_Transpose)} can only be evaluated on array if none of the elements are non-numeric - arg={arg.ToString()}");
                }

                var nestedElements = elements.Select(e => e.IterationValues);

                var result = new List<IEnumerable<DataValue>>();
                for (int i = 0; i < nestedElements.Min(n => n.Count); i++)
                {
                    result.Add(nestedElements.Select(n => n[i]).ToArray()); // Materialisation required as value of i changes :/
                }

                // If all elements were strings, cast back to strings
                if (elements.All(e => e.Type == DataValueType.String))
                {
                    return new ArrayValue(result.Select(r => new StringValue(String.Join("", r.Select(x => x.ToString())))));
                }
                else
                {
                    return new ArrayValue(result.Select(r => new ArrayValue(r)));
                }
            }
        }
    }
}
