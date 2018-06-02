using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class GetVariable : Token
    {
        public const string CHAR_LIST = "\u2825\u2845\u28A1\u2885\u2861\u28E1\u28C5\u28A5\u2865\u28E5";

        public override int Arity => 0;

        public int VariableIndex { get; private set; }

        public GetVariable(char tokenCharacter)
        {
            VariableIndex = CHAR_LIST.IndexOf(tokenCharacter);
            if (VariableIndex < 0) throw new Pangolin.Common.PangolinException($"Unexpected token character {tokenCharacter} passed to GetVariable");
        }

        public override DataValue Evaluate(ProgramState programState)
        {
            return programState.GetVariable(VariableIndex);
        }

        public override string ToString()
        {
            return CHAR_LIST.Substring(VariableIndex, 1);
        }
    }

    public class SetVariable : Token
    {
        public const string CHAR_LIST = "\u2849\u2843\u2858\u2851\u284A\u285A\u2853\u2859\u284B\u285B";

        public int VariableIndex { get; private set; }

        public override int Arity => 1;

        public SetVariable(char tokenCharacter)
        {
            VariableIndex = CHAR_LIST.IndexOf(tokenCharacter);
            if (VariableIndex < 0) throw new Pangolin.Common.PangolinException($"Unexpected token character {tokenCharacter} passed to SetVariable");
        }

        public override DataValue Evaluate(ProgramState programState)
        {
            var argument = programState.DequeueAndEvaluate();
            programState.SetVariable(VariableIndex, argument);
            return argument;
        }

        public override string ToString()
        {
            return CHAR_LIST.Substring(VariableIndex, 1);
        }
    }
}
