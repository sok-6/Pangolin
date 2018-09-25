using Pangolin.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core
{
    public class ProgramState
    {
        public const string AVAILABLE_ITERATION_CONSTANTS = "abcdefghijklmnopqrstuvwxyz";

        private const int VARIABLE_COUNT = 10;
        private List<DataValue> _argumentList;
        private DataValue[] _variables;

        public IReadOnlyList<Token> TokenList { get; private set; }

        public virtual IReadOnlyList<DataValue> ArgumentList => _argumentList;
        public IReadOnlyList<DataValue> Variables => _variables;

        public int CurrentTokenIndex { get; private set; }
        public bool ExecutionInProgress => CurrentTokenIndex < TokenList.Count;

        private Dictionary<IterationConstantDetails, DataValue> _iterationFunctionConstants;
        public IReadOnlyDictionary<IterationConstantDetails, DataValue> IterationFunctionConstants => _iterationFunctionConstants;
        public Stack<IterationConstantDetails> DefaultTokenStack { get; private set; }

        public bool IsInPartialTokenApplication { get; private set; }
        private Queue<DataValue> _partialApplicationTokenQueue;

        public int CurrentBlockLevel { get; private set; }

        public int AssignedIterationConstantsCount { get; private set; } = 0;

        public ProgramState()
        {
            CurrentTokenIndex = 0;
            TokenList = new List<Token>();
            _argumentList = new List<DataValue>();
            _iterationFunctionConstants = new Dictionary<IterationConstantDetails, DataValue>();
            DefaultTokenStack = new Stack<IterationConstantDetails>();

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
            _iterationFunctionConstants = new Dictionary<IterationConstantDetails, DataValue>();
            DefaultTokenStack = new Stack<IterationConstantDetails>();

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

        public virtual void SetIterationFunctionConstant(IterationConstantDetails token, DataValue value)
        {
            _iterationFunctionConstants[token] = value;
        }

        public virtual void ClearIterationFunctionConstant(IterationConstantDetails token)
        {
            _iterationFunctionConstants.Remove(token);
        }

        public IReadOnlyList<IterationConstantDetails> GetIterationConstants(int numConstantsRequired)
        {
            return Enumerable
                .Range(AssignedIterationConstantsCount, numConstantsRequired)
                .Select(i => new IterationConstantDetails(
                    AVAILABLE_ITERATION_CONSTANTS.Substring(i % AVAILABLE_ITERATION_CONSTANTS.Length, 1),
                    i / AVAILABLE_ITERATION_CONSTANTS.Length))
                .ToList();
        }

        /// <summary>
        /// Finds the end of a function from a given starting index
        /// </summary>
        /// <param name="functionStartIndex">The index of the start of the function</param>
        /// <returns>The index of the last token in the function - i.e. the start of the next function is return value + 1</returns>
        public int FindEndOfFunction(int functionStartIndex)
        {
            // TODO: Block/TokenLed tokens?

            var currentArity = functionStartIndex < TokenList.Count ? TokenList[functionStartIndex].Arity : 0;
            var result = functionStartIndex;
            
            for (int i = 0; i < currentArity; i++)
            {
                result = FindEndOfFunction(result + 1);
            }

            return result;
        }

        public void SetCurrentTokenIndex(int newIndex)
        {
            CurrentTokenIndex = newIndex;
        }

        public virtual void StepOverNextFunction()
        {
            SetCurrentTokenIndex(FindEndOfFunction(CurrentTokenIndex) + 1);
        }

        public void IncreaseBlockLevel()
        {
            CurrentBlockLevel++;
        }

        public void DecreaseBlockLevel()
        {
            CurrentBlockLevel--;
        }

        [DebuggerDisplay("{Depth}{TokenString}")]
        public class IterationConstantDetails
        {
            public string TokenString { get; private set; }
            public int Depth { get; private set; }

            public IterationConstantDetails(string tokenString, int depth)
            {
                TokenString = tokenString;
                Depth = depth;
            }

            public override int GetHashCode() => $"{Depth}{TokenString}".GetHashCode();

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(obj, null))
                {
                    return false;
                }

                var that = obj as IterationConstantDetails;

                if (ReferenceEquals((object)that, null))
                {
                    return false;
                }

                return this.Depth == that.Depth && this.TokenString == that.TokenString;
            }
        }
    }
}
