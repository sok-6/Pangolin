using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;

namespace Pangolin.Core.TokenImplementations
{
    public class UnTruthify : Token
    {
        public override int Arity => 1;

        public override DataValue Evaluate(ProgramState tokenQueue)
        {
            // Get argument
            var argument = tokenQueue.DequeueAndEvaluate();

            return argument.IsTruthy ? DataValue.Falsey : DataValue.Truthy;
        }

        public override string ToString() => "\u00A1";
    }
}
