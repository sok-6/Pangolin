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
}
