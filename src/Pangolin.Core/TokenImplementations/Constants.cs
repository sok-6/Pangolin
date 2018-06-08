using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class ConstantNewline : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState)
        {
            return new DataValueImplementations.StringValue("\n");
        }

        public override string ToString() => "\u00B6";
    }

    public class ConstantEmptyArray : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState)
        {
            return new DataValueImplementations.ArrayValue();
        }

        public override string ToString() => "a";
    }

    public class ConstantEmptyString : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState)
        {
            return new DataValueImplementations.StringValue();
        }

        public override string ToString() => "e";
    }
}
