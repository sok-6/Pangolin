using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pangolin.Core.TokenImplementations
{
    public abstract class IterationTokenBase : Token
    {
        public override DataValue Evaluate(ProgramState programState)
        {
            // Check if block is already executing
            if (IsExecuting(programState))
            {
                throw new PangolinException($"{GetType().Name} token encountered during execution of same");
            }

            Initialise();

            // Make note of current index to return to 
            var startIndex = programState.CurrentTokenIndex;

            // Find index of end of function block
            var blockEnd = programState.FindEndOfBlock(startIndex);

            // Set execution index to end of function block and get values to execute where over
            programState.SetCurrentTokenIndex(blockEnd + 1);
            var iterationCount = GetArguments(programState);

            // Make note of end of arguments index
            var endIndex = programState.CurrentTokenIndex;

            // Iterative execution
            SetExecuting(programState, true);

            for (int i = 0; i < iterationCount; i++)
            {
                programState.SetCurrentTokenIndex(startIndex);
                SetIterationVariables(programState, i);

                HandleIterationResult(i, programState.DequeueAndEvaluate());
            }

            // Finished execution, set execution index to end, finalise
            SetExecuting(programState, false);
            programState.SetCurrentTokenIndex(endIndex);
            return CompleteResult();
        }

        protected abstract void Initialise();
        protected abstract bool IsExecuting(ProgramState programState);
        protected abstract void SetExecuting(ProgramState programState, bool isExecuting);
        protected abstract int GetArguments(ProgramState programState);
        protected abstract void SetIterationVariables(ProgramState programState, int iterationCount);
        protected abstract void HandleIterationResult(int iterationCount, DataValue iterationResult);
        protected abstract DataValue CompleteResult();

        protected IReadOnlyList<DataValue> ConvertToValueList(DataValue value)
        {
            var innerValues = new List<DataValue>();

            // Numeric, create range
            if (value.Type == DataValueType.Numeric)
            {
                var numValue = value as NumericValue;

                // Can only do it if integral
                if (!numValue.IsIntegral)
                {
                    throw new PangolinInvalidArgumentTypeException("Where cannot accept non-integral numeric as 2nd argument");
                }

                innerValues.AddRange(
                    Enumerable
                        .Range(Math.Min(0, numValue.IntValue + 1), Math.Abs(numValue.IntValue))
                        .Select(i => new NumericValue(i)));
            }
            // String, separate into single length strings
            else if (value.Type == DataValueType.String)
            {
                innerValues.AddRange(
                    ((StringValue)value).Value
                    .Select(c => new StringValue(c.ToString())));
            }
            // Array, just take values
            else
            {
                innerValues.AddRange(((ArrayValue)value).Value);
            }

            return innerValues;
        }
    }

    public class Where : IterationTokenBase
    {
        private IReadOnlyList<DataValue> _iterationValues;
        private List<DataValue> _filteredValues;

        public override int Arity => 2;
        
        public override string ToString() => "W";

        protected override void Initialise()
        {
            _filteredValues = new List<DataValue>();
        }

        protected override DataValue CompleteResult()
        {
            return new ArrayValue(_filteredValues);
        }

        protected override int GetArguments(ProgramState programState)
        {
            // One argument required
            _iterationValues = ConvertToValueList(programState.DequeueAndEvaluate());

            return _iterationValues.Count;
        }

        protected override void HandleIterationResult(int iterationCount, DataValue iterationResult)
        {
            // Only add v to result set if r is truthy
            if (iterationResult.IsTruthy)
            {
                _filteredValues.Add(_iterationValues[iterationCount]);
            }
        }

        protected override bool IsExecuting(ProgramState programState) => programState.IsWhereBlockExecuting;

        protected override void SetExecuting(ProgramState programState, bool isExecuting)
        {
            programState.IsWhereBlockExecuting = isExecuting;
        }

        protected override void SetIterationVariables(ProgramState programState, int iterationCount)
        {
            programState.WhereValue = _iterationValues[iterationCount];
        }
    }

    public class WhereValue : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState)
        {
            if (programState.WhereValue == null)
            {
                throw new PangolinInvalidTokenException("WhereValue token encountered outside of where block");
            }

            return programState.WhereValue;
        }

        public override string ToString() => "w";
    }

    public class Select : IterationTokenBase
    {
        private IReadOnlyList<DataValue> _iterationValues;
        private List<DataValue> _mappedValues;

        public override int Arity => 2;

        public override string ToString() => "S";

        protected override void Initialise()
        {
            _mappedValues = new List<DataValue>();
        }

        protected override DataValue CompleteResult()
        {
            return new ArrayValue(_mappedValues);
        }

        protected override int GetArguments(ProgramState programState)
        {
            _iterationValues = ConvertToValueList(programState.DequeueAndEvaluate());
            return _iterationValues.Count;
        }

        protected override void HandleIterationResult(int iterationCount, DataValue iterationResult)
        {
            _mappedValues.Add(iterationResult);
        }

        protected override bool IsExecuting(ProgramState programState) => programState.IsSelectBlockExecuting;

        protected override void SetExecuting(ProgramState programState, bool isExecuting)
        {
            programState.IsSelectBlockExecuting = isExecuting;
        }

        protected override void SetIterationVariables(ProgramState programState, int iterationCount)
        {
            programState.SelectValue = _iterationValues[iterationCount];
        }
    }

    public class SelectValue : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState)
        {
            if (programState.SelectValue == null)
            {
                throw new PangolinInvalidTokenException("SelectValue token encountered outside of select block");
            }

            return programState.SelectValue;
        }

        public override string ToString() => "s";
    }
}
