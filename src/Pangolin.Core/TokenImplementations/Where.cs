using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pangolin.Core.TokenImplementations
{
    public class Where : FunctionToken
    {
        private const string ITERATION_VARIABLE_TOKENS = "wx";
        private const string ITERATION_INDEX_TOKENS = "gh";

        public override int Arity => 2;
        public override string ToString() => "W";

        private IReadOnlyList<DataValue> _iterationValues = null;

        protected override void ClearIterationConstants(ProgramState programState, int nestingLevel)
        {
            programState.ClearIterationFunctionConstant(ITERATION_VARIABLE_TOKENS.Substring(nestingLevel, 1));
            programState.ClearIterationFunctionConstant(ITERATION_INDEX_TOKENS.Substring(nestingLevel, 1));
        }

        protected override string GetDefaultToken(int nestingLevel) => ITERATION_VARIABLE_TOKENS.Substring(nestingLevel, 1);
        
        protected override int GetNestedAmount(ProgramState programState) => ITERATION_VARIABLE_TOKENS.Count(c => programState.IterationFunctionConstants.ContainsKey(c.ToString()));

        protected override int GetNestingLimit() => ITERATION_VARIABLE_TOKENS.Length;

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

        protected override void SetIterationConstants(ProgramState programState, int nestingLevel, int iterationIndex)
        {
            programState.SetIterationFunctionConstant(ITERATION_VARIABLE_TOKENS.Substring(nestingLevel, 1), _iterationValues[iterationIndex]);
            programState.SetIterationFunctionConstant(ITERATION_INDEX_TOKENS.Substring(nestingLevel, 1), new NumericValue(iterationIndex));
        }
    }
    
    public class WhereIterationVariable0 : IterationConstantToken { public override string ToString() => "w"; }
    public class WhereIterationVariable1 : IterationConstantToken { public override string ToString() => "x"; }
    public class WhereIterationIndex0 : IterationConstantToken { public override string ToString() => "g"; }
    public class WhereIterationIndex1 : IterationConstantToken { public override string ToString() => "h"; }
}
