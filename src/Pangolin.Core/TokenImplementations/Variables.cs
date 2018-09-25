using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class GetVariable : Token
    {
        public const string CHAR_LIST = "\u00E1\u0107\u00E9\u01F5\u1E31\u1E3F\u0144\u00F3\u015B\u00FA";

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
        public const string CHAR_LIST = "\u00C1\u0106\u00C9\u01F4\u1E30\u1E3E\u0143\u00D3\u015A\u00DA";

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
