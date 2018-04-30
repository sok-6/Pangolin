using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Truthify : Token
    {
        public override int Arity => 1;

        public override DataValue Evaluate(ProgramState tokenQueue)
        {
            // Get argument
            var argument = tokenQueue.DequeueAndEvaluate();

            return argument.Truthify();
        }

        public override string ToString() => "\u00A1";
    }
}
