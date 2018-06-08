using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pangolin.Core.TokenImplementations
{
    public class Arrayify : Token
    {
        public override int Arity => 1;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get argument, return it wrapped in an array
            return new ArrayValue(programState.DequeueAndEvaluate());
        }

        public override string ToString() => "A";
    }

    public class ArrayPair : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get 2 arguments, return them wrapped in an array
            var arg1 = programState.DequeueAndEvaluate();
            var arg2 = programState.DequeueAndEvaluate();

            return new ArrayValue(arg1, arg2);
        }

        public override string ToString() => "]";
    }
}
