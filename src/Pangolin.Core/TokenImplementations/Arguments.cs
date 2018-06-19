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
        public const string CHAR_LIST = "\u24EA\u2460\u2461\u2462\u2463\u2464\u2465\u2466\u2467\u2468";

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
