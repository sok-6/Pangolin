
using System;
using System.Collections.Generic;
using System.Linq;
using Pangolin.Core.DataValueImplementations;

namespace Pangolin.Core.TokenImplementations
{
    public class Multiply : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            var a = programState.DequeueAndEvaluate();
            var b = programState.DequeueAndEvaluate();

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

                // Sort out negative repeat times
                var reverse = repeatTimes < 0;
                repeatTimes = Math.Abs(repeatTimes);

                // Get values to repeat
                var repeatValues = new List<object>();
                repeatValues.AddRange(
                    a.Type == DataValueType.Numeric
                        ? returnString
                            ? ((StringValue)b).Value.Cast<object>()
                            : ((ArrayValue)b).Value.Cast<object>()
                        : returnString
                            ? ((StringValue)a).Value.Cast<object>()
                            : ((ArrayValue)a).Value.Cast<object>());

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

            throw new NotImplementedException();
        }

        public override string ToString() => "*";
    }
}