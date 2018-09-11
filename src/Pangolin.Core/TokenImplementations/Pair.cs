using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Pair : Token // TODO: Write tests
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get the number of selects already in progress
            var alreadyRunningSelectCount = programState.IterationFunctionConstants.ContainsKey("p");

            if (programState.IterationFunctionConstants.ContainsKey("p"))
            {
                throw new PangolinException($"Can't nest P tokens");
            }

            // Save current token index to return to once 2nd arg evaluated
            var firstArgTokenIndex = programState.CurrentTokenIndex;

            // Step over 1st arg, evaluate 2nd
            programState.StepOverNextFunction();
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
            programState.DefaultTokenStack.Push("p");

            // Execute iteration block once per value pair in set
            var results = new List<DataValue>();

            for (int i = 0; i < iterationValueSet.Count() - 1; i++)
            {
                // Return to the iteration block
                programState.SetCurrentTokenIndex(firstArgTokenIndex);

                // Set iteration variable and index
                programState.SetIterationFunctionConstant("p", iterationValueSet.Skip(i).First());
                programState.SetIterationFunctionConstant("q", iterationValueSet.Skip(i + 1).First());

                // Execute function block, add to result set
                results.Add(programState.DequeueAndEvaluate());
            }

            // Clear iteration variable and index
            programState.ClearIterationFunctionConstant("p");
            programState.ClearIterationFunctionConstant("q");

            // Remove variable token from stack of default values
            if (programState.DefaultTokenStack.Peek() != "p")
            {
                throw new PangolinException($"Top of default token stack in unexpected state - p expected, actually {programState.DefaultTokenStack.Peek()}");
            }
            programState.DefaultTokenStack.Pop();

            // Move to end of 2nd argument
            programState.SetCurrentTokenIndex(endTokenIndex);

            // All done, return
            return new ArrayValue(results);
        }

        public override string ToString() => "P";
    }

    public class PairIterationVariableLeft : IterationConstantToken { public override string ToString() => "p"; }
    public class PairIterationVariableRight : IterationConstantToken { public override string ToString() => "q"; }

    public class SimplePair : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => ",";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // Only implemented for non numerics
            if (arg.Type == DataValueType.Numeric)
            {
                throw GetInvalidArgumentTypeException(ToString(), arg.Type);
            }

            var elements = arg.IterationValues;
            var results = new List<DataValue>();

            for (int i = 0; i < elements.Count - 1; i++)
            {
                results.Add(new ArrayValue(elements[i], elements[i + 1]));
            }

            return new ArrayValue(results);
        }
    }
}
