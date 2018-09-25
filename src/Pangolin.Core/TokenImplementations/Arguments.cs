using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class SingleArgument : Token
    {
        public const string CHAR_LIST = "\u2474\u2475\u2476\u2477\u2478\u2479\u247A\u247B\u247C\u247D";

        public override int Arity => 0;

        public int ArgumentIndex { get; private set; }

        public SingleArgument(char tokenCharacter)
        {
            ArgumentIndex = CHAR_LIST.IndexOf(tokenCharacter);
            if (ArgumentIndex < 0) throw new Pangolin.Common.PangolinException($"Unexpected token character {tokenCharacter} passed to SingleArgument");
        }

        public override DataValue Evaluate(ProgramState programState)
        {
            return ArgumentIndex < programState.ArgumentList.Count ? programState.ArgumentList[ArgumentIndex] : NumericValue.Zero;
        }

        public override string ToString() => CHAR_LIST[ArgumentIndex].ToString();
    }

    public class ArgumentArray : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState tokenQueue)
        {
            return new ArrayValue(tokenQueue.ArgumentList);
        }

        public override string ToString() => "\u00A5";
    }
}
