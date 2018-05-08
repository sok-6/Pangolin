using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class GetVariable : Token
    {
        private const string CHAR_LIST = "\uDD52\uDD53\uDD54\uDD55\uDD56\uDD57\uDD58\uDD59\uDD5A\uDD5B";

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
        private const string CHAR_LIST = "\uDD38\uDD39\u2102\uDD3B\uDD3C\uDD3D\uDD3E\u210D\uDD40\uDD41";

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
