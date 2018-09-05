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

    public class AggregateFirst : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Check if aggregate already in progress
            if (programState.IterationFunctionConstants.ContainsKey("a"))
            {
                throw new PangolinException($"Can't nest {nameof(AggregateFirst)} tokens");
            }

            // Save current token index to return to once 2nd arg evaluated
            var firstArgTokenIndex = programState.CurrentTokenIndex;

            // Step over 1st arg, evaluate 2nd
            programState.StepOverNextTokenBlock();
            var iterationValue = programState.DequeueAndEvaluate();

            // Save token index to return to once select token execution ended
            var endTokenIndex = programState.CurrentTokenIndex;

            // Get iteration value set
            IEnumerable<DataValue> iterationValueSet;
            if (iterationValue.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)iterationValue).IntValue;

                iterationValueSet = Enumerable.Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Select(i => new NumericValue(i));
            }
            else
            {
                iterationValueSet = iterationValue.IterationValues;
            }

            // Add variable token to stack of default values
            programState.DefaultTokenStack.Push("b");

            // Execute iteration block once per value in set
            var currentValue = iterationValueSet.First();
            for (int i = 1; i < iterationValueSet.Count(); i++)
            {
                // Return to the iteration block
                programState.SetCurrentTokenIndex(firstArgTokenIndex);

                // Set iteration variable and index
                programState.SetIterationFunctionConstant("a", currentValue);
                programState.SetIterationFunctionConstant("b", iterationValueSet.Skip(i).First());

                // Execute function block, add to result set
                currentValue = programState.DequeueAndEvaluate();
            }

            // Clear iteration variable and index
            programState.ClearIterationFunctionConstant("a");
            programState.ClearIterationFunctionConstant("b");

            // Remove variable token from stack of default values
            if (programState.DefaultTokenStack.Peek() != "b")
            {
                throw new PangolinException($"Top of default token stack in unexpected state - b expected, actually {programState.DefaultTokenStack.Peek()}");
            }
            programState.DefaultTokenStack.Pop();

            // Move to end of 2nd argument
            programState.SetCurrentTokenIndex(endTokenIndex);

            // All done, return
            return currentValue;
        }

        public override string ToString() => "A";
    }

    public class AggregateFirstVariableConstantCurrent : IterationConstantBase { public override string ToString() => "a"; }
    public class AggregateFirstVariableConstantNext : IterationConstantBase { public override string ToString() => "b"; }

    public class CollapseFunction : Token
    {
        public override int Arity => 3;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Check if aggregate already in progress
            if (programState.IterationFunctionConstants.ContainsKey("c"))
            {
                throw new PangolinException($"Can't nest {nameof(CollapseFunction)} tokens");
            }

            // Save current token index to return to once 2nd arg evaluated
            var firstArgTokenIndex = programState.CurrentTokenIndex;

            // Step over 1st arg, evaluate 2nd and 3rd
            programState.StepOverNextTokenBlock();
            var initialValue = programState.DequeueAndEvaluate();
            var iterationValue = programState.DequeueAndEvaluate();

            // Save token index to return to once select token execution ended
            var endTokenIndex = programState.CurrentTokenIndex;

            // Get iteration value set
            IEnumerable<DataValue> iterationValueSet;
            if (iterationValue.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)iterationValue).IntValue;

                iterationValueSet = Enumerable.Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Select(i => new NumericValue(i));
            }
            else
            {
                iterationValueSet = iterationValue.IterationValues;
            }

            // Add variable token to stack of default values
            programState.DefaultTokenStack.Push("d");

            // Execute iteration block once per value in set
            var currentValue = initialValue;
            for (int i = 0; i < iterationValueSet.Count(); i++)
            {
                // Return to the iteration block
                programState.SetCurrentTokenIndex(firstArgTokenIndex);

                // Set iteration variable and index
                programState.SetIterationFunctionConstant("c", currentValue);
                programState.SetIterationFunctionConstant("d", iterationValueSet.Skip(i).First());

                // Execute function block, add to result set
                currentValue = programState.DequeueAndEvaluate();
            }

            // Clear iteration variable and index
            programState.ClearIterationFunctionConstant("c");
            programState.ClearIterationFunctionConstant("d");

            // Remove variable token from stack of default values
            if (programState.DefaultTokenStack.Peek() != "d")
            {
                throw new PangolinException($"Top of default token stack in unexpected state - d expected, actually {programState.DefaultTokenStack.Peek()}");
            }
            programState.DefaultTokenStack.Pop();

            // Move to end of 2nd argument
            programState.SetCurrentTokenIndex(endTokenIndex);

            // All done, return
            return currentValue;
        }

        public override string ToString() => "C";
    }

    public class CollapseFunctionVariableConstantCurrent : IterationConstantBase { public override string ToString() => "c"; }
    public class CollapseFunctionVariableConstantNext : IterationConstantBase { public override string ToString() => "d"; }
}
