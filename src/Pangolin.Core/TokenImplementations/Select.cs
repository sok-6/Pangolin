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

        protected override void ClearIterationConstants(ProgramState programState, int nestingLevel)
        {
            // Clear variable and index constants
            programState.ClearIterationFunctionConstant(ITERATION_VARIABLE_TOKENS.Substring(nestingLevel, 1));
            programState.ClearIterationFunctionConstant(ITERATION_INDEX_TOKENS.Substring(nestingLevel, 1));
        }

        protected override string GetDefaultToken(int nestingLevel)
        {
            return ITERATION_VARIABLE_TOKENS.Substring(nestingLevel, 1);
        }

        protected override int GetNestedAmount(ProgramState programState)
        {
            return ITERATION_VARIABLE_TOKENS.Count(c => programState.IterationFunctionConstants.ContainsKey(c.ToString()));
        }

        protected override int GetNestingLimit()
        {
            return ITERATION_VARIABLE_TOKENS.Length;
        }

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

        protected override void SetIterationConstants(ProgramState programState, int nestingLevel, int iterationIndex)
        {
            programState.SetIterationFunctionConstant(ITERATION_VARIABLE_TOKENS.Substring(nestingLevel, 1), _iterationValues[iterationIndex]);
            programState.SetIterationFunctionConstant(ITERATION_INDEX_TOKENS.Substring(nestingLevel, 1), new NumericValue(iterationIndex));
        }
    }

    public class SelectMany : FunctionToken
    {
        private const string ITERATION_VARIABLE_TOKENS = "m";
        private const string ITERATION_INDEX_TOKENS = "n";

        public override int Arity => 2;

        //public override DataValue Evaluate(ProgramState programState)
        //{
        //    // Get the number of selects already in progress
        //    var alreadyRunningSelectCount = ITERATION_VARIABLE_TOKENS.Count(c => programState.IterationFunctionConstants.ContainsKey(c.ToString()));

        //    if (alreadyRunningSelectCount == ITERATION_VARIABLE_TOKENS.Length)
        //    {
        //        throw new PangolinException($"Can't nest {ToString()} tokens more than {ITERATION_VARIABLE_TOKENS.Length} deep");
        //    }

        //    // Get variable and index tokens
        //    var variableToken = ITERATION_VARIABLE_TOKENS.Substring(alreadyRunningSelectCount, 1);
        //    var indexToken = ITERATION_INDEX_TOKENS.Substring(alreadyRunningSelectCount, 1);

        //    // Save current token index to return to once 2nd arg evaluated
        //    var firstArgTokenIndex = programState.CurrentTokenIndex;

        //    // Step over 1st arg, evaluate 2nd
        //    programState.StepOverNextTokenBlock();
        //    var iterationValue = programState.DequeueAndEvaluate();

        //    // Save token index to return to once select token execution ended
        //    var endTokenIndex = programState.CurrentTokenIndex;

        //    // Get iteration value set
        //    IEnumerable<DataValue> iterationValueSet;
        //    if (iterationValue.Type == DataValueType.Numeric)
        //    {
        //        var a = ((NumericValue)iterationValue).IntValue;

        //        iterationValueSet = Enumerable.Range(Math.Min(0, a + 1), Math.Abs(a))
        //                .Select(i => new NumericValue(i));
        //    }
        //    else
        //    {
        //        iterationValueSet = iterationValue.IterationValues;
        //    }

        //    // Add variable token to stack of default values
        //    programState.DefaultTokenStack.Push(variableToken);

        //    // Execute iteration block once per value in set
        //    var results = new List<DataValue>();

        //    for (int i = 0; i < iterationValueSet.Count(); i++)
        //    {
        //        // Return to the iteration block
        //        programState.SetCurrentTokenIndex(firstArgTokenIndex);

        //        // Set iteration variable and index
        //        programState.SetIterationFunctionConstant(variableToken, iterationValueSet.Skip(i).First());
        //        programState.SetIterationFunctionConstant(indexToken, new NumericValue(i));

        //        // Execute function block, add to result set
        //        var r = programState.DequeueAndEvaluate();
        //        if (r.Type == DataValueType.Array)
        //        {
        //            // Flatten if array
        //            results.AddRange(r.IterationValues);
        //        }
        //        else
        //        {
        //            results.Add(r);
        //        }
        //    }

        //    // Clear iteration variable and index
        //    programState.ClearIterationFunctionConstant(variableToken);
        //    programState.ClearIterationFunctionConstant(indexToken);

        //    // Remove variable token from stack of default values
        //    if (programState.DefaultTokenStack.Peek() != variableToken)
        //    {
        //        throw new PangolinException($"Top of default token stack in unexpected state - {variableToken} expected, actually {programState.DefaultTokenStack.Peek()}");
        //    }
        //    programState.DefaultTokenStack.Pop();

        //    // Move to end of 2nd argument
        //    programState.SetCurrentTokenIndex(endTokenIndex);

        //    // All done, return
        //    return new ArrayValue(results);
        //}

        public override string ToString() => "M";

        private IReadOnlyList<DataValue> _iterationValues = null;

        protected override void ClearIterationConstants(ProgramState programState, int nestingLevel)
        {
            programState.ClearIterationFunctionConstant("m");
            programState.ClearIterationFunctionConstant("n");
        }

        protected override string GetDefaultToken(int nestingLevel) => "m";

        protected override int GetNestedAmount(ProgramState programState) => programState.IterationFunctionConstants.ContainsKey("m") ? 1 : 0;

        protected override int GetNestingLimit() => 1;

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

        protected override void SetIterationConstants(ProgramState programState, int nestingLevel, int iterationIndex)
        {
            programState.SetIterationFunctionConstant("m", _iterationValues[iterationIndex]);
            programState.SetIterationFunctionConstant("n", new NumericValue(iterationIndex));
        }
    }

    public class SelectIterationVariable0 : IterationConstantToken { public override string ToString() => "s"; }
    public class SelectIterationVariable1 : IterationConstantToken { public override string ToString() => "t"; }
    public class SelectIterationVariable2 : IterationConstantToken { public override string ToString() => "u"; }
    public class SelectIterationVariable3 : IterationConstantToken { public override string ToString() => "v"; }
    public class SelectIterationIndex0 : IterationConstantToken { public override string ToString() => "i"; }
    public class SelectIterationIndex1 : IterationConstantToken { public override string ToString() => "j"; }
    public class SelectIterationIndex2 : IterationConstantToken { public override string ToString() => "k"; }
    public class SelectIterationIndex3 : IterationConstantToken { public override string ToString() => "l"; }
    public class SelectManyIterationVariable : IterationConstantToken { public override string ToString() => "m"; }
    public class SelectManyIterationIndex : IterationConstantToken { public override string ToString() => "n"; }
}
