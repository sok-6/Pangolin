
using System;
using System.Collections.Generic;
using System.Linq;
using Pangolin.Core.DataValueImplementations;

namespace Pangolin.Core.TokenImplementations
{
    public class Multiply : IterableToken
    {
        public override int Arity => 2;
        public override string ToString() => "*";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg1 = arguments[0];
            var arg2 = arguments[1];

            return ProcessMultiply(arg1, arg2);
        }

        public static DataValue ProcessMultiply(DataValue arg1, DataValue arg2)
        {
            // Both numeric, regular multiplication
            if (arg1.Type == DataValueType.Numeric && arg2.Type == DataValueType.Numeric)
            {
                return new NumericValue(((NumericValue)arg1).Value * ((NumericValue)arg2).Value);
            }
            // Exactly 1 numeric, repeat
            else if (arg1.Type == DataValueType.Numeric || arg2.Type == DataValueType.Numeric)
            {
                var repeatTimes = (arg1.Type) == DataValueType.Numeric ? ((NumericValue)arg1).Value : ((NumericValue)arg2).Value;
                var returnString = (arg1.Type == DataValueType.String || arg2.Type == DataValueType.String);

                // If repeat 0 times, return empty value
                if (repeatTimes == 0)
                {
                    return returnString ? (DataValue)(new StringValue("")) : (DataValue)(new ArrayValue());
                }

                // Get values to repeat
                var repeatValues = new List<object>(
                    arg1.Type == DataValueType.Numeric
                        ? returnString
                            ? ((StringValue)arg2).Value.Cast<object>()
                            : ((ArrayValue)arg2).Value.Cast<object>()
                        : returnString
                            ? ((StringValue)arg1).Value.Cast<object>()
                            : ((ArrayValue)arg1).Value.Cast<object>());

                // If no values, return empty value
                if (repeatValues.Count == 0)
                {
                    return returnString ? (DataValue)(new StringValue("")) : (DataValue)(new ArrayValue());
                }

                // Sort out negative repeat times
                var reverse = repeatTimes < 0;
                repeatTimes = Math.Abs(repeatTimes);

                // Perform repetition
                var result = new List<object>();
                var index = 0;

                while (result.Count < repeatValues.Count * repeatTimes)
                {
                    result.Add(repeatValues[index % repeatValues.Count]);
                    index++;
                }

                // Reverse if required
                if (reverse)
                {
                    result.Reverse();
                }

                // Convert and return
                if (returnString)
                {
                    return new StringValue(String.Join("", result.ToArray()));
                }
                else
                {
                    return new ArrayValue(result.Cast<DataValue>());
                }
            }
            // No numerics, cartesian product
            else
            {
                // If either is falsey (i.e. is empty string or empty array), product is empty
                if (!arg1.IsTruthy || !arg2.IsTruthy)
                {
                    return new ArrayValue();
                }

                // Get elements of a and b - if string, treat each character as separate value
                var setA = new List<DataValue>(
                    arg1.Type == DataValueType.String
                        ? ((StringValue)arg1).Value.Select(c => new StringValue(c.ToString())).Cast<DataValue>()
                        : ((ArrayValue)arg1).Value);
                var setB = new List<DataValue>(
                    arg2.Type == DataValueType.String
                        ? ((StringValue)arg2).Value.Select(c => new StringValue(c.ToString())).Cast<DataValue>()
                        : ((ArrayValue)arg2).Value);

                // Return pairs, iterating over b first
                // TODO: iteration order arbitrary, could be revisited?
                var result = new List<DataValue>();
                for (int i = 0; i < setA.Count; i++)
                {
                    for (int j = 0; j < setB.Count; j++)
                    {
                        result.Add(new ArrayValue(setA[i], setB[j]));
                    }
                }

                return new ArrayValue(result);
            }
        }
    }

    public class Double : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "D";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            return Multiply.ProcessMultiply(new NumericValue(2), arg);
        }
    }
}