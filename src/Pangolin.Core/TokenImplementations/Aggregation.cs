using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Sum : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u03A3";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // Numeric, nth triagle number if integral
            if (arg.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)arg;

                if (!numericArg.IsIntegral)
                {
                    throw new Common.PangolinInvalidArgumentTypeException($"Invalid argument type passed to \u03A3 command - non-integral numeric: {arg}");
                }
                if (numericArg.Value < 0)
                {
                    throw new Common.PangolinInvalidArgumentTypeException($"Invalid argument type passed to \u03A3 command - negative numeric: {arg}");
                }

                // Calculate triagle number using (n*(n+1))/2
                return new NumericValue((numericArg.IntValue * (numericArg.IntValue + 1)) / 2);
            }
            // Array, reduce on +
            else if (arg.Type == DataValueType.Array)
            {
                var arrayArg = (ArrayValue)arg;

                // Figure out what type the output will be
                // Array - concatenate all elements into single array
                if (arrayArg.Value.Any(v => v.Type == DataValueType.Array))
                {
                    var newArrayContents = new List<DataValue>();

                    foreach (var dv in arrayArg.Value)
                    {
                        // Add arrays as set of elements, other types as individual elements
                        if (dv.Type == DataValueType.Array)
                        {
                            newArrayContents.AddRange(dv.IterationValues);
                        }
                        else
                        {
                            newArrayContents.Add(dv);
                        }
                    }

                    return new ArrayValue(newArrayContents);
                }
                // String - convert all elements to strings and add to single string value
                else if (arrayArg.Value.Any(v => v.Type == DataValueType.String))
                {
                    var sb = new StringBuilder();

                    foreach (var dv in arrayArg.Value)
                    {
                        sb.Append(dv.ToString());
                    }

                    return new StringValue(sb.ToString());
                }
                // Numeric or empty array - simple sum, starting at 0
                else
                {
                    return new NumericValue(arrayArg.Value.Count == 0 ? 0 : arrayArg.Value.Sum(v => ((NumericValue)v).Value));
                }
            }
            // String, not implemented yet
            else
            {
                throw GetInvalidArgumentTypeException(ToString(), DataValueType.String);
            }
        }
    }

    public class AggregateFirst : FunctionToken
    {
        public override int Arity => 2;
        public override string ToString() => "A";

        private IReadOnlyList<DataValue> _iterationValueSet = null;
        private DataValue _current;

        protected override void ClearIterationConstants(ProgramState programState, int nestingLevel)
        {
            programState.ClearIterationFunctionConstant("a");
            programState.ClearIterationFunctionConstant("b");
        }

        protected override string GetDefaultToken(int nestingLevel) => "b";

        protected override int GetNestedAmount(ProgramState programState) => programState.IterationFunctionConstants.ContainsKey("a") ? 1 : 0;

        protected override int GetNestingLimit() => 1;

        protected override void PostExecutionAction(DataValue executionResult)
        {
            _current = executionResult;
        }

        protected override DataValue ProcessResults(IReadOnlyList<IterationResultContainer> results)
        {
            return _current;
        }

        protected override int RetrieveFunctionArguments(ProgramState programState)
        {
            var arg = programState.DequeueAndEvaluate();
            IEnumerable<DataValue> tempSet;

            if (arg.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)arg).IntValue;

                tempSet = Enumerable.Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Select(i => new NumericValue(i));
            }
            else
            {
                tempSet = arg.IterationValues;
            }

            // First value is current, rest are queued to iterate over
            _current = tempSet.First();
            _iterationValueSet = tempSet.Skip(1).ToList();

            return _iterationValueSet.Count;
        }

        protected override void SetIterationConstants(ProgramState programState, int nestingLevel, int iterationIndex)
        {
            programState.SetIterationFunctionConstant("a", _current);
            programState.SetIterationFunctionConstant("b", _iterationValueSet.Skip(iterationIndex).First());
        }
    }

    public class AggregateFirstVariableConstantCurrent : IterationConstantToken { public override string ToString() => "a"; }
    public class AggregateFirstVariableConstantNext : IterationConstantToken { public override string ToString() => "b"; }

    public class CollapseFunction : FunctionToken
    {
        public override string ToString() => "C";
        public override int Arity => 3;

        private IReadOnlyList<DataValue> _iterationValueSet = null;
        private DataValue _current;
                
        protected override void ClearIterationConstants(ProgramState programState, int nestingLevel)
        {
            programState.ClearIterationFunctionConstant("c");
            programState.ClearIterationFunctionConstant("d");
        }

        protected override string GetDefaultToken(int nestingLevel) => "d";

        protected override int GetNestedAmount(ProgramState programState) => programState.IterationFunctionConstants.ContainsKey("c") ? 1 : 0;

        protected override int GetNestingLimit() => 1;

        protected override DataValue ProcessResults(IReadOnlyList<IterationResultContainer> results)
        {
            return _current;
        }
        protected override void PostExecutionAction(DataValue executionResult)
        {
            _current = executionResult;
        }

        protected override int RetrieveFunctionArguments(ProgramState programState)
        {
            _current = programState.DequeueAndEvaluate();
            var arg = programState.DequeueAndEvaluate();

            if (arg.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)arg).IntValue;

                _iterationValueSet = Enumerable.Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Select(i => new NumericValue(i)).ToList();
            }
            else
            {
                _iterationValueSet = arg.IterationValues;
            }
            
            return _iterationValueSet.Count;
        }

        protected override void SetIterationConstants(ProgramState programState, int nestingLevel, int iterationIndex)
        {
            programState.SetIterationFunctionConstant("c", _current);
            programState.SetIterationFunctionConstant("d", _iterationValueSet[iterationIndex]);
        }
    }

    public class CollapseFunctionVariableConstantCurrent : IterationConstantToken { public override string ToString() => "c"; }
    public class CollapseFunctionVariableConstantNext : IterationConstantToken { public override string ToString() => "d"; }
}
