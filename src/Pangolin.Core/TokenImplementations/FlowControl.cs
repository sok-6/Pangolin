using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class IfThenElse : Token
    {
        public override int Arity => 3;

        public override DataValue Evaluate(ProgramState programState)
        {
            // If a is truthy, evaluate b, else evaluate c
            if (programState.DequeueAndEvaluate().IsTruthy)
            {
                var result = programState.DequeueAndEvaluate();
                programState.StepOverNextTokenBlock();

                return result;
            }
            else
            {
                programState.StepOverNextTokenBlock();
                return programState.DequeueAndEvaluate();
            }
        }

        public override string ToString() => "?";
    }

    public class ConditionalApply : TokenLedToken
    {
        public override int Arity => 3;

        public override string ToString() => "\u00BF";

        protected override DataValue EvaluateInner(ProgramState programState)
        {
            var conditional = programState.DequeueAndEvaluate();
            var argument = programState.DequeueAndEvaluate();

            // Only call token if conditional is truthy
            DataValue result = argument;
            if (conditional.IsTruthy)
            {
                // Set up partial application queue
                programState.EnqueuePartialApplicationValue(argument);

                result = _argumentToken.Evaluate(programState);

                // Clear partial application queue
                programState.ClearPartialApplicationQueue();
            }

            return result;
        }
    }
}
