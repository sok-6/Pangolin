using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Add : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(TokenQueue tokenQueue)
        {
            // Get two arguments
            var arg1 = tokenQueue.DequeueAndEvaluate();
            var arg2 = tokenQueue.DequeueAndEvaluate();

            if (arg1.Type == DataValueType.Array) // Array concatenate
            {
                var newArrayContents = new List<DataValue>(((ArrayValue)arg1).Value);

                if (arg2.Type == DataValueType.Array)
                {
                    newArrayContents.AddRange(((ArrayValue)arg2).Value);
                }
                else
                {
                    newArrayContents.Add(arg2);
                }

                return new ArrayValue(newArrayContents);
            }
            else if (arg2.Type == DataValueType.Array) // Prepend to 2nd array
            {
                var newArrayContents = new List<DataValue>();
                newArrayContents.Add(arg1);
                newArrayContents.AddRange(((ArrayValue)arg2).Value);
                return new ArrayValue(newArrayContents);
            }
            else if (arg1.Type == DataValueType.String || arg2.Type == DataValueType.String) // String concatenation
            {
                return new StringValue($"{arg1}{arg2}");
            }
            else
            {
                var value1 = ((NumericValue)arg1).Value;
                var value2 = ((NumericValue)arg2).Value;

                return new NumericValue(value1 + value2);
            }
        }

        public override string ToString() => "+";
    }
}
