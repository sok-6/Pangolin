using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pangolin.Core.TokenImplementations
{
    public class Select : FunctionToken // TODO: Write tests
    {
        private const string ITERATION_VARIABLE_TOKENS = "stuv";
        private const string ITERATION_INDEX_TOKENS = "ijkl";

        public override int Arity => 2;
        public override string ToString() => "S";

        private IReadOnlyList<DataValue> _iterationValues = null;

        public Select() : base(2) { }
        
        protected override DataValue ProcessResults(IReadOnlyList<IterationResultContainer> results)
        {
            return new ArrayValue(results.Select(r => r.IterationResult));
        }

        protected override int RetrieveFunctionArguments(ProgramState programState)
        {
            // Single argument required
            var iterationValue = programState.DequeueAndEvaluate();

            // Convert numeric to range
            if (iterationValue.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)iterationValue).IntValue;

                _iterationValues = Enumerable.Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Select(i => new NumericValue(i))
                        .ToList();
            }
            else
            {
                _iterationValues = iterationValue.IterationValues;
            }

            return _iterationValues.Count;
        }
        
        protected override ProgramState.IterationConstantDetails GetDefaultToken(IReadOnlyList<ProgramState.IterationConstantDetails> allocatedTokens) => allocatedTokens[0];

        protected override void SetIterationConstants(ProgramState programState, IReadOnlyList<ProgramState.IterationConstantDetails> allocatedTokens, int iterationIndex)
        {
            programState.SetIterationFunctionConstant(allocatedTokens[0], _iterationValues[iterationIndex]);
            programState.SetIterationFunctionConstant(allocatedTokens[1], new NumericValue(iterationIndex));
        }
    }

    public class SelectMany : FunctionToken
    {
        private IReadOnlyList<DataValue> _iterationValues = null;

        public SelectMany() : base(2) { }

        public override int Arity => 2;

        public override string ToString() => "M";
        
        protected override ProgramState.IterationConstantDetails GetDefaultToken(IReadOnlyList<ProgramState.IterationConstantDetails> allocatedTokens) => allocatedTokens[0];
        
        protected override DataValue ProcessResults(IReadOnlyList<IterationResultContainer> results)
        {
            var resultSet = new List<DataValue>();

            foreach (var r in results)
            {
                // Flatten arrays
                if (r.IterationResult.Type == DataValueType.Array)
                {
                    resultSet.AddRange(r.IterationResult.IterationValues);
                }
                else
                {
                    resultSet.Add(r.IterationResult);
                }
            }

            return new ArrayValue(resultSet);
        }

        protected override int RetrieveFunctionArguments(ProgramState programState)
        {
            // Single argument required
            var iterationValue = programState.DequeueAndEvaluate();

            // Convert numeric to range
            if (iterationValue.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)iterationValue).IntValue;

                _iterationValues = Enumerable.Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Select(i => new NumericValue(i))
                        .ToList();
            }
            else
            {
                _iterationValues = iterationValue.IterationValues;
            }

            return _iterationValues.Count;
        }

        protected override void SetIterationConstants(ProgramState programState, IReadOnlyList<ProgramState.IterationConstantDetails> allocatedTokens, int iterationIndex)
        {
            programState.SetIterationFunctionConstant(allocatedTokens[0], _iterationValues[iterationIndex]);
            programState.SetIterationFunctionConstant(allocatedTokens[1], new NumericValue(iterationIndex));
        }
    }
}
