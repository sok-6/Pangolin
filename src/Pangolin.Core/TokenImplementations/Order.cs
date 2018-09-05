using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pangolin.Core.TokenImplementations
{
    public class Order : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Check if ordering already in progress
            if (programState.IterationFunctionConstants.ContainsKey("o"))
            {
                throw new PangolinException($"Can't nest {ToString()} tokens");
            }

            // Get variable and index tokens
            var variableToken = "o";
            var indexToken = "\u1ECD";

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
            programState.DefaultTokenStack.Push(variableToken);

            // Execute iteration block once per value in set
            var elementsWithOrderKeys = new List<Order.ElementWithOrderingKey>();

            for (int i = 0; i < iterationValueSet.Count(); i++)
            {
                // Return to the iteration block
                programState.SetCurrentTokenIndex(firstArgTokenIndex);

                // Set iteration variable and index
                var currentElement = iterationValueSet.Skip(i).First();
                programState.SetIterationFunctionConstant(variableToken, currentElement);
                programState.SetIterationFunctionConstant(indexToken, new NumericValue(i));

                // Execute function block, add to result set
                elementsWithOrderKeys.Add(new ElementWithOrderingKey(currentElement, programState.DequeueAndEvaluate()));
            }

            // Clear iteration variable and index
            programState.ClearIterationFunctionConstant(variableToken);
            programState.ClearIterationFunctionConstant(indexToken);

            // Remove variable token from stack of default values
            if (programState.DefaultTokenStack.Peek() != variableToken)
            {
                throw new PangolinException($"Top of default token stack in unexpected state - {variableToken} expected, actually {programState.DefaultTokenStack.Peek()}");
            }
            programState.DefaultTokenStack.Pop();

            // Move to end of 2nd argument
            programState.SetCurrentTokenIndex(endTokenIndex);

            // Sort elements by their respective keys and return
            var orderedElements = ExecuteOrdering(elementsWithOrderKeys);
            return new ArrayValue(orderedElements);
        }

        public override string ToString() => "O";

        #region Ordering execution whatnots
        public class ElementWithOrderingKey
        {
            public DataValue Key { get; private set; }
            public DataValue Element { get; private set; }

            public ElementWithOrderingKey(DataValue element, DataValue key = null)
            {
                Element = element;
                Key = key ?? element;
            }
        }

        public static IEnumerable<DataValue> ExecuteOrdering(IEnumerable<DataValue> elements)
        {
            var result = elements.Select(e => new ElementWithOrderingKey(e)).ToArray();
            MergeSort(result);
            return result.Select(r => r.Element);
        }

        public static IEnumerable<DataValue> ExecuteOrdering(IEnumerable<ElementWithOrderingKey> elements)
        {
            var result = elements.ToArray();
            MergeSort(result);
            return result.Select(r => r.Element);
        }

        private static void MergeSort(ElementWithOrderingKey[] elements)
        {
            if (elements.Length < 2)
            {
                return;
            }

            // Split into left and right - ties split to left
            var left = elements.Take((elements.Length + 1) / 2).ToArray();
            var right = elements.TakeLast(elements.Length / 2).ToArray();

            // Sort each sublist
            MergeSort(left);
            MergeSort(right);

            // Perform merge
            var lIndex = 0;
            var rIndex = 0;

            while (lIndex + rIndex < elements.Length)
            {
                var comparisonResult =
                    lIndex == left.Length
                        ? 1
                        : rIndex == right.Length
                            ? -1
                            : RunComparison(left[lIndex].Key, right[rIndex].Key);

                // If tied, take from left (stable sort)
                if (comparisonResult <= 0)
                {
                    elements[lIndex + rIndex] = left[lIndex];
                    lIndex++;
                }
                else
                {
                    elements[lIndex + rIndex] = right[rIndex];
                    rIndex++;
                }
            }
        }

        public static int RunComparison(DataValue a, DataValue b)
        {
            if (a.Type != b.Type)
            {
                throw new ArgumentException($"Comparison failed between types {a.Type} and {b.Type}");
            }

            if (a.Type == DataValueType.Numeric)
            {
                return ((NumericValue)a).Value.CompareTo(((NumericValue)b).Value);
            }
            else if (a.Type == DataValueType.String)
            {
                var aValue = ((StringValue)a).Value;
                var bValue = ((StringValue)b).Value;

                // Compare unicode code points of each char
                for (int i = 0; i < Math.Min(aValue.Length, bValue.Length); i++)
                {
                    var comparisonResult = aValue[i].CompareTo(bValue[i]);
                    if (comparisonResult != 0)
                    {
                        return comparisonResult;
                    }
                }

                // Both equal so far, shorter string should be sorted before longer
                return aValue.Length.CompareTo(bValue.Length);
            }
            else
            {
                var aValue = ((ArrayValue)a).Value;
                var bValue = ((ArrayValue)b).Value;

                // Check each element in turn
                for (int i = 0; i < Math.Min(aValue.Count, bValue.Count); i++)
                {
                    var comparisonResult = RunComparison(aValue[i], bValue[i]);
                    if (comparisonResult != 0)
                    {
                        return comparisonResult;
                    }
                }

                // All elements equal, shorter array should be sorted before longer
                return aValue.Count.CompareTo(bValue.Count);
            }
        }
        #endregion
    }

    public class OrderIterationVariable : IterationConstantBase { public override string ToString() => "o"; }
    public class OrderIterationIndex : IterationConstantBase { public override string ToString() => "\u1ECD"; }

    public class Ascend : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u2191";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // If numeric, increment
            if (arg.Type == DataValueType.Numeric)
            {
                return new NumericValue(((NumericValue)arg).Value + 1);
            }
            // String or array, sort ascending
            else
            {
                var elements = arg.IterationValues;

                var sortedElements = Order.ExecuteOrdering(elements);

                return DataValueSetToStringOrArray(sortedElements, arg.Type);
            }
        }
    }

    public class Descend : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u2193";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // If numeric, decrement
            if (arg.Type == DataValueType.Numeric)
            {
                return new NumericValue(((NumericValue)arg).Value - 1);
            }
            // String or array, sort ascending
            else
            {
                var elements = arg.IterationValues;

                var sortedElements = Order.ExecuteOrdering(elements).Reverse();

                return DataValueSetToStringOrArray(sortedElements, arg.Type);
            }
        }
    }

    public class Floor : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "_";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // Numeric, round towards -inf
            if (arg.Type == DataValueType.Numeric)
            {
                return new NumericValue(Math.Floor(((NumericValue)arg).Value));
            }
            // String, smallest character by unicode code point
            else if (arg.Type == DataValueType.String)
            {
                var stringValue = ((StringValue)arg).Value;

                if (stringValue.Length == 0)
                {
                    throw new PangolinException($"{nameof(Floor)} unable to find minimum value of empty string");
                }

                // Order by unicode, take first
                return new StringValue(stringValue.OrderBy(c => (int)c).First().ToString());
            }
            // Array, order and take first
            else
            {
                var elements = ((ArrayValue)arg).Value;

                if (elements.Count == 0)
                {
                    throw new PangolinException($"{nameof(Floor)} unable to find minimum value of empty array");
                }

                return Order.ExecuteOrdering(elements).First();
            }
        }
    }

    public class Ceiling : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u00AF";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // Numeric, round towards +inf
            if (arg.Type == DataValueType.Numeric)
            {
                return new NumericValue(Math.Ceiling(((NumericValue)arg).Value));
            }
            // String, smallest character by unicode code point
            else if (arg.Type == DataValueType.String)
            {
                var stringValue = ((StringValue)arg).Value;

                if (stringValue.Length == 0)
                {
                    throw new PangolinException($"{nameof(Ceiling)} unable to find minimum value of empty string");
                }

                // Order by unicode, take first
                return new StringValue(stringValue.OrderBy(c => (int)c).Last().ToString());
            }
            // Array, order and take first
            else
            {
                var elements = ((ArrayValue)arg).Value;

                if (elements.Count == 0)
                {
                    throw new PangolinException($"{nameof(Ceiling)} unable to find minimum value of empty array");
                }

                return Order.ExecuteOrdering(elements).Last();
            }
        }
    }
}
