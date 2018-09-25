using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pangolin.Core.TokenImplementations
{
    public class Order : FunctionToken
    {
        private IReadOnlyList<DataValue> _iterationValueSet;

        public Order() : base(2) { }

        public override int Arity => 2;
        
        public override string ToString() => "O";
        
        protected override int RetrieveFunctionArguments(ProgramState programState)
        {
            var iterationValue = programState.DequeueAndEvaluate();

            // Get iteration value set
            if (iterationValue.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)iterationValue).IntValue;

                _iterationValueSet = Enumerable.Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Select(i => new NumericValue(i))
                        .ToList();
            }
            else
            {
                _iterationValueSet = iterationValue.IterationValues;
            }

            return _iterationValueSet.Count;
        }

        protected override ProgramState.IterationConstantDetails GetDefaultToken(IReadOnlyList<ProgramState.IterationConstantDetails> allocatedTokens) => allocatedTokens[0];

        protected override void SetIterationConstants(ProgramState programState, IReadOnlyList<ProgramState.IterationConstantDetails> allocatedTokens, int iterationIndex)
        {
            programState.SetIterationFunctionConstant(allocatedTokens[0], _iterationValueSet[iterationIndex]);
            programState.SetIterationFunctionConstant(allocatedTokens[1], new NumericValue(iterationIndex));
        }

        protected override DataValue ProcessResults(IReadOnlyList<IterationResultContainer> results)
        {
            var elementsWithOrderKeys = results.Select(r => new ElementWithOrderingKey(_iterationValueSet[r.Index], r.IterationResult));
            var orderedElements = ExecuteOrdering(elementsWithOrderKeys);
            return new ArrayValue(orderedElements);
        }

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
