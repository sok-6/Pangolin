using Pangolin.Common;
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
        private List<DataValue> _argumentList;
        private DataValue[] _variables;

        public IReadOnlyList<Token> TokenList { get; private set; }

        public virtual IReadOnlyList<DataValue> ArgumentList => _argumentList;
        public IReadOnlyList<DataValue> Variables => _variables;

        public int CurrentTokenIndex { get; private set; }
        public bool ExecutionInProgress => CurrentTokenIndex < TokenList.Count;

        private Dictionary<string, DataValue> _iterationFunctionConstants;
        public IReadOnlyDictionary<string, DataValue> IterationFunctionConstants => _iterationFunctionConstants;
        public Stack<string> DefaultTokenStack { get; private set; }

        public bool IsInPartialTokenApplication { get; private set; }
        private Queue<DataValue> _partialApplicationTokenQueue;

        public ProgramState()
        {
            CurrentTokenIndex = 0;
            TokenList = new List<Token>();
            _argumentList = new List<DataValue>();
            _iterationFunctionConstants = new Dictionary<string, DataValue>();
            DefaultTokenStack = new Stack<string>();

            _variables = new DataValue[VARIABLE_COUNT];
            for (int i = 0; i < VARIABLE_COUNT; i++) _variables[i] = DataValueImplementations.NumericValue.Zero;

            IsInPartialTokenApplication = false;
            _partialApplicationTokenQueue = new Queue<DataValue>();
        }

        public ProgramState(IReadOnlyList<DataValue> argumentList, IReadOnlyList<Token> tokens)
        {
            CurrentTokenIndex = 0;
            TokenList = tokens;
            _argumentList = new List<DataValue>(argumentList ?? new DataValue[0]);
            _iterationFunctionConstants = new Dictionary<string, DataValue>();
            DefaultTokenStack = new Stack<string>();

            _variables = new DataValue[VARIABLE_COUNT];
            for (int i = 0; i < VARIABLE_COUNT; i++) _variables[i] = DataValueImplementations.NumericValue.Zero;

            IsInPartialTokenApplication = false;
            _partialApplicationTokenQueue = new Queue<DataValue>();
        }
        
        public virtual DataValue DequeueAndEvaluate()
        {
            // If in partial token application, return token from partial queue
            if (IsInPartialTokenApplication)
            {
                if (_partialApplicationTokenQueue.Any())
                {
                    return _partialApplicationTokenQueue.Dequeue();
                }
                else
                {
                    throw new PangolinException("Attempted to dequeue from empty partial application queue");
                }
            }
            else
	        {
                // Check if run out of tokens
                if (CurrentTokenIndex >= TokenList.Count)
                {
                    // Get default token if any
                    if (DefaultTokenStack.Count > 0)
                    {
                        return _iterationFunctionConstants[DefaultTokenStack.Peek()];
                    }
                    else
                    {
                        // Return 0th argument, or lit 0
                        return _argumentList.Count > 0 ? _argumentList[0] : DataValueImplementations.NumericValue.Zero;
                    }
                }

                // 'Dequeue' and evaluate
                var currentToken = TokenList[CurrentTokenIndex];
                CurrentTokenIndex++;
                return currentToken.Evaluate(this); 
            }
        }

        public void EnqueuePartialApplicationValue(DataValue value)
        {
            IsInPartialTokenApplication = true;
            _partialApplicationTokenQueue.Enqueue(value);
        }

        public void ClearPartialApplicationQueue()
        {
            IsInPartialTokenApplication = false;
            _partialApplicationTokenQueue.Clear();
        }

        public virtual void SetVariable(int index, DataValue value)
        {
            _variables[index] = value;
        }

        public virtual DataValue GetVariable(int index)
        {
            return _variables[index];
        }

        public virtual void SetIterationFunctionConstant(string token, DataValue value)
        {
            _iterationFunctionConstants[token] = value;
        }

        public virtual void ClearIterationFunctionConstant(string token)
        {
            _iterationFunctionConstants.Remove(token);
        }

        /// <summary>
        /// Finds the end of a block of execution from a given starting index
        /// </summary>
        /// <param name="blockStartIndex">The index of the start of the block</param>
        /// <returns>The index of the last token in the block - i.e. the start of the next block is return value + 1</returns>
        public int FindEndOfBlock(int blockStartIndex)
        {
            var currentArity = blockStartIndex < TokenList.Count ? TokenList[blockStartIndex].Arity : 0;
            var result = blockStartIndex;
            
            for (int i = 0; i < currentArity; i++)
            {
                result = FindEndOfBlock(result + 1);
            }

            return result;
        }

        public void SetCurrentTokenIndex(int newIndex)
        {
            CurrentTokenIndex = newIndex;
        }

        public virtual void StepOverNextTokenBlock()
        {
            SetCurrentTokenIndex(FindEndOfBlock(CurrentTokenIndex) + 1);
        }
    }
}
