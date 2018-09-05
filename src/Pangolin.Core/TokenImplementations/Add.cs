using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Add : IterableToken
    {
        public override int Arity => 2;
        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg1 = arguments[0];
            var arg2 = arguments[1];

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

    public class IteratedAdd : Add
    {
        public override DataValue Evaluate(ProgramState tokenQueue)
        {
            // Get two arguments
            var arg1 = tokenQueue.DequeueAndEvaluate();
            var arg2 = tokenQueue.DequeueAndEvaluate();
            
            if (arg1.Type != DataValueType.Numeric)
            {
                if (arg2.Type != DataValueType.Numeric)
                {
                    // Zip them
                    return new ArrayValue(arg1.IterationValues.Zip(arg2.IterationValues, (a1, a2) => EvaluateInner(new List<DataValue>() { a1, a2 })));
                }
                else
                {
                    return new ArrayValue(arg1.IterationValues.Select(a1 => EvaluateInner(new List<DataValue>() { a1, arg2 })));
                }
            }
            else
            {
                if (arg2.Type != DataValueType.Numeric)
                {
                    return new ArrayValue(arg2.IterationValues.Select(a2 => EvaluateInner(new List<DataValue>() { arg1, a2 })));
                }
                else
                {
                    throw GetInvalidArgumentTypeException(ToString(), arg1.Type, arg2.Type);
                }
            }
        }

        public override string ToString() => "\u2295";
    }
}
