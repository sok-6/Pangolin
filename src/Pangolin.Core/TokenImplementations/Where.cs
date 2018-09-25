using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pangolin.Core.TokenImplementations
{
    public class Where : FunctionToken
    {
        public override int Arity => 2;
        public override string ToString() => "W";

        private IReadOnlyList<DataValue> _iterationValues = null;

        public Where() : base(2) { }

        protected override DataValue ProcessResults(IReadOnlyList<IterationResultContainer> results)
        {
            return new ArrayValue(results.Where(r => r.IterationResult.IsTruthy).Select(r => _iterationValues[r.Index]));
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
}
