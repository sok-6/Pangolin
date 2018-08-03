using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class LogicAnd : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get 1st operand
            var op1 = programState.DequeueAndEvaluate();

            // If falsey, no need to evaluate 2nd operand
            if (!op1.IsTruthy)
            {
                programState.StepOverNextTokenBlock();
                return DataValue.Falsey;
            }
            else
            {
                var op2 = programState.DequeueAndEvaluate();
                return DataValue.BoolToTruthiness(op2.IsTruthy);
            }
        }

        public override string ToString() => "&";
    }

    public class LogicOr : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get 1st operand
            var op1 = programState.DequeueAndEvaluate();

            // If truthy, no need to evaluate 2nd operand
            if (op1.IsTruthy)
            {
                programState.StepOverNextTokenBlock();
                return DataValue.Truthy;
            }
            else
            {
                var op2 = programState.DequeueAndEvaluate();
                return DataValue.BoolToTruthiness(op2.IsTruthy);
            }
        }

        public override string ToString() => "|";
    }

    public class LogicXor : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            var op1 = programState.DequeueAndEvaluate();
            var op2 = programState.DequeueAndEvaluate();

            return DataValue.BoolToTruthiness(op1.IsTruthy ^ op2.IsTruthy);
        }

        public override string ToString() => "X";
    }

    public class LogicXnor : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            var op1 = programState.DequeueAndEvaluate();
            var op2 = programState.DequeueAndEvaluate();

            return DataValue.BoolToTruthiness(!(op1.IsTruthy ^ op2.IsTruthy));
        }

        public override string ToString() => "~";
    }
}
