using Pangolin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Iterate : Token
    {
        public override int Arity => 1;

        public override DataValue Evaluate(ProgramState programState)
        {
            var arg = programState.DequeueAndEvaluate();

            if (arg.Type == DataValueType.Numeric)
            {
                throw new PangolinException("Iterate is not defined for Numeric");
            }
            else
            {
                arg.SetIterationRequired(true);

                return arg;
            }
        }

        public override string ToString() => "I";
    }
}
