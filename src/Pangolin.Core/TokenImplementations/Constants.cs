using Pangolin.Core.DataValueImplementations;
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

        public override string ToString() => "\u25AF";
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

    public class ConstantLowercaseAlphabet : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState) => new StringValue("abcdefghijklmnopqrstuvwxyz");

        public override string ToString() => "\u1EA1";
    }

    public class ConstantUppercaseAlphabet : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState) => new StringValue("ABCDEFGHIJKLMNOPQRSTUVWXYZ");

        public override string ToString() => "\u1E05";
    }

    public class ConstantPi : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState) => new NumericValue(Math.PI);

        public override string ToString() => "\u03C0";
    }

    public class ConstantTau : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState) => new NumericValue(2 * Math.PI);

        public override string ToString() => "\u03C4";
    }

    public class ConstantSpace : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState) => new StringValue(" ");

        public override string ToString() => "\u1E63";
    }
}
