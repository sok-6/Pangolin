using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Range : Token
    {
        public override int Arity => 1;

        public override DataValue Evaluate(TokenQueue tokenQueue)
        {
            var argument = tokenQueue.DequeueAndEvaluate();

            if (argument.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)argument).IntValue;

                return new ArrayValue(
                    Enumerable
                        .Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Select(i => new NumericValue(i)));
            }
            else
            {
                throw GetInvalidArgumentTypeException(argument.Type);
            }
        }

        public override string ToString() => "\u2192";
    }
}
