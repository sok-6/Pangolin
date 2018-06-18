
using System;
using System.Collections.Generic;
using System.Linq;
using Pangolin.Core.DataValueImplementations;

namespace Pangolin.Core.TokenImplementations
{
    public class Multiply : ArityTwoIterableToken
    {
        public override string ToString() => "*";

        protected override DataValue EvaluateInner(DataValue a, DataValue b)
        {
            // Both numeric, regular multiplication
            if (a.Type == DataValueType.Numeric && b.Type == DataValueType.Numeric)
            {
                return new NumericValue(((NumericValue)a).Value * ((NumericValue)b).Value);
            }
            // Exactly 1 numeric, repeat
            else if (a.Type == DataValueType.Numeric || b.Type == DataValueType.Numeric)
            {
                var repeatTimes = (a.Type) == DataValueType.Numeric ? ((NumericValue)a).Value : ((NumericValue)b).Value;
                var returnString = (a.Type == DataValueType.String || b.Type == DataValueType.String);

                // If repeat 0 times, return empty value
                if (repeatTimes == 0)
                {
                    return returnString ? (DataValue)(new StringValue("")) : (DataValue)(new ArrayValue());
                }

                // Get values to repeat
                var repeatValues = new List<object>(
                    a.Type == DataValueType.Numeric
                        ? returnString
                            ? ((StringValue)b).Value.Cast<object>()
                            : ((ArrayValue)b).Value.Cast<object>()
                        : returnString
                            ? ((StringValue)a).Value.Cast<object>()
                            : ((ArrayValue)a).Value.Cast<object>());

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
                if (!a.IsTruthy || !b.IsTruthy)
                {
                    return new ArrayValue();
                }

                // Get elements of a and b - if string, treat each character as separate value
                var setA = new List<DataValue>(
                    a.Type == DataValueType.String
                        ? ((StringValue)a).Value.Select(c => new StringValue(c.ToString())).Cast<DataValue>()
                        : ((ArrayValue)a).Value);
                var setB = new List<DataValue>(
                    b.Type == DataValueType.String
                        ? ((StringValue)b).Value.Select(c => new StringValue(c.ToString())).Cast<DataValue>()
                        : ((ArrayValue)b).Value);

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
}