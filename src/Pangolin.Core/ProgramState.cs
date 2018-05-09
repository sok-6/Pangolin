using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core
{
    public class ProgramState
    {
        private const int VARIABLE_COUNT = 10;

        private List<Token> _tokenList;
        private List<DataValue> _argumentList;
        private DataValue[] _variables;

        public IReadOnlyList<Token> TokenList => _tokenList;
        public virtual IReadOnlyList<DataValue> ArgumentList => _argumentList;
        public IReadOnlyList<DataValue> Variables => _variables;

        public int CurrentTokenIndex { get; private set; }
        public bool ExecutionInProgress => CurrentTokenIndex < _tokenList.Count;

        public int? WhereIndex { get; private set; }

        public ProgramState()
        {
            CurrentTokenIndex = 0;
            _tokenList = new List<Token>();
            _argumentList = new List<DataValue>();

            _variables = new DataValue[VARIABLE_COUNT];
            for (int i = 0; i < VARIABLE_COUNT; i++) _variables[i] = DataValueImplementations.NumericValue.Zero;
        }

        public ProgramState(IReadOnlyList<DataValue> argumentList, params Token[] tokens)
        {
            CurrentTokenIndex = 0;
            _tokenList = new List<Token>(tokens);
            _argumentList = new List<DataValue>(argumentList ?? new DataValue[0]);

            _variables = new DataValue[VARIABLE_COUNT];
            for (int i = 0; i < VARIABLE_COUNT; i++) _variables[i] = DataValueImplementations.NumericValue.Zero;
        }

        public void EnqueueToken(Token item)
        {
            _tokenList.Add(item);
        }

        public virtual DataValue DequeueAndEvaluate()
        {
            // Check if run out of tokens
            if (CurrentTokenIndex == _tokenList.Count)
            {
                // Return 0th argument, or lit 0
                return _argumentList.Count > 0 ? _argumentList[0] : DataValueImplementations.NumericValue.Zero;
            }

            // 'Dequeue' and evaluate
            var currentToken = _tokenList[CurrentTokenIndex];
            CurrentTokenIndex++;
            return currentToken.Evaluate(this);
        }

        public virtual void SetVariable(int index, DataValue value)
        {
            _variables[index] = value;
        }

        public virtual DataValue GetVariable(int index)
        {
            return _variables[index];
        }

        public void SetWhereIndex(int whereIndex)
        {
            WhereIndex = whereIndex;
        }

        public void ClearWhereIndex()
        {
            WhereIndex = null;
        }

        public int FindEndOfBlock(int blockStartIndex)
        {
            var currentArity = _tokenList[blockStartIndex].Arity;
            var result = blockStartIndex;
            
            for (int i = 0; i < currentArity; i++)
            {
                result = FindEndOfBlock(result + 1);
            }

            return result;
        }
    }
}
