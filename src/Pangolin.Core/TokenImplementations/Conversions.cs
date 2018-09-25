using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pangolin.Core.TokenImplementations
{
    public class Arrayify : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u1EA0";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            // Get argument, return it wrapped in an array
            return new ArrayValue(arguments);
        }
    }

    public class ArrayPair : IterableToken
    {
        public override int Arity => 2;
        public override string ToString() => "]";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            return new ArrayValue(arguments);
        }
    }

    public class ArrayTriple : Token
    {
        public override int Arity => 3;

        public override DataValue Evaluate(ProgramState programState)
        {
            return new ArrayValue(
                programState.DequeueAndEvaluate(),
                programState.DequeueAndEvaluate(),
                programState.DequeueAndEvaluate());
        }

        public override string ToString() => "\u039E";
    }

    public class Elements : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u03B4";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // Numeric, get base 10 digits of +ve integral
            if (arg.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)arg;

                if (numericArg.Value < 0)
                {
                    throw new PangolinInvalidArgumentTypeException($"Invalid argument type passed to \u03B4 command - negative value: {numericArg.Value}");
                }
                if (!numericArg.IsIntegral)
                {
                    throw new PangolinInvalidArgumentTypeException($"Invalid argument type passed to \u03B4 command - non-integral value: {numericArg.Value}");
                }

                var result = BaseConversion.ConvertToIntegerBase(10, numericArg.IntValue);

                // Special case 0
                if (result.Count() == 0)
                {
                    result = new int[] { 0 };
                }

                return new ArrayValue(result.Select(r => new NumericValue(r)));
            }
            // String, split into array of single character strings
            else if (arg.Type == DataValueType.String)
            {
                return new ArrayValue(arg.IterationValues);
            }
            // Array, no implemented outcome
            else
            {
                throw GetInvalidArgumentTypeException(ToString(), arg.Type);
            }
        }
    }

    public class Transform_Transpose : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u0393";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // Numeric, stringify
            if (arg.Type == DataValueType.Numeric)
            {
                return new StringValue(arg.ToString());
            }
            // String, parse as numeric
            else if (arg.Type == DataValueType.String)
            {
                var s = arg.ToString();
                if (!double.TryParse(s, out var value))
                {
                    throw new PangolinException($"Failed to parse string \"{s}\" as numeric");
                }

                return new NumericValue(value);
            }
            // Array, transpose with truncation
            else
            {
                var elements = ((ArrayValue)arg).Value;

                // If any numerics, can't transpose
                if (elements.Any(e => e.Type == DataValueType.Numeric))
                {
                    throw new PangolinInvalidArgumentTypeException($"{nameof(Transform_Transpose)} can only be evaluated on array if none of the elements are non-numeric - arg={arg.ToString()}");
                }

                var nestedElements = elements.Select(e => e.IterationValues);

                var result = new List<IEnumerable<DataValue>>();
                for (int i = 0; i < nestedElements.Min(n => n.Count); i++)
                {
                    result.Add(nestedElements.Select(n => n[i]).ToArray()); // Materialisation required as value of i changes :/
                }

                // If all elements were strings, cast back to strings
                if (elements.All(e => e.Type == DataValueType.String))
                {
                    return new ArrayValue(result.Select(r => new StringValue(String.Join("", r.Select(x => x.ToString())))));
                }
                else
                {
                    return new ArrayValue(result.Select(r => new ArrayValue(r)));
                }
            }
        }
    }

    public class BaseConversion : IterableToken
    {
        public override int Arity => 2;
        public override string ToString() => "B";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg1 = arguments[0];
            var arg2 = arguments[1];

            // Arguments are base, value

            // Numeric base
            if (arg1.Type == DataValueType.Numeric)
            {
                var numericArg1 = (NumericValue)arg1;

                if (!numericArg1.IsIntegral)
                {
                    // TODO: Implement non-integral bases
                    throw new PangolinInvalidArgumentTypeException($"Non-integral bases not implemented yet - base={arg1}, value={arg2}");
                }

                var baseValue = numericArg1.IntValue;

                if (baseValue == 0 || baseValue == -1)
                {
                    throw new PangolinException($"Conversion not possible with specified base - base={arg1}, value={arg2}");
                }

                // Numeric value, convert from decimal to new base
                if (arg2.Type == DataValueType.Numeric)
                {
                    var result = ConvertToIntegerBase(baseValue, ((NumericValue)arg2).Value);

                    return new ArrayValue(result.Select(r => new NumericValue(r)));
                }
                // Array value, convert to decimal from specified base
                else if (arg2.Type == DataValueType.Array)
                {
                    var arrayArg2 = (ArrayValue)arg2;

                    // All elements must be numeric, integral and less than abs base value
                    if (arrayArg2.Value.Any(a => a.Type != DataValueType.Numeric))
                    {
                        throw new PangolinInvalidArgumentTypeException($"When converting to decimal from numeric base, all digit values must be numeric - base={arg1}, value={arg2}");
                    }

                    var arrayNumericsArg2 = arrayArg2.Value.Select(a => (NumericValue)a);
                    if (!arrayNumericsArg2.All(n => n.IsIntegral))
                    {
                        throw new PangolinInvalidArgumentTypeException($"When converting to decimal from numeric base, all digit values must be integral - base={arg1}, value={arg2}");
                    }

                    if (arrayNumericsArg2.Any(n => n.IntValue >= Math.Abs(baseValue)))
                    {
                        throw new PangolinInvalidArgumentTypeException($"When converting to decimal from numeric base, all digit values must be less than base - base={arg1}, value={arg2}");
                    }

                    // Perform conversion back to decimal
                    var result = ConvertFromIntegerBase(baseValue, arrayNumericsArg2.Select(n => n.IntValue));
                    return new NumericValue(result);
                }
                // String value, not defined
                else
                {
                    throw GetInvalidArgumentTypeException(nameof(BaseConversion), arg1.Type, arg2.Type);
                }
            }
            // String base
            else if (arg1.Type == DataValueType.String)
            {
                var baseCharacters = ((StringValue)arg1).Value;

                // Check if base's characters are unique
                if (baseCharacters.Distinct().Count() != baseCharacters.Length)
                {
                    throw new PangolinInvalidArgumentTypeException($"For string base, all characters in base must be unique - base={arg1}, value={arg2}");
                }

                // Numeric value, convert from decimal to new base
                if (arg2.Type == DataValueType.Numeric)
                {
                    var result = ConvertToIntegerBase(baseCharacters.Length, ((NumericValue)arg2).Value);

                    // Convert indices to characters and concatenate
                    return new StringValue(String.Join("", result.Select(r => baseCharacters[r])));
                }
                // String value, convert to decimal from base
                else if (arg2.Type == DataValueType.String)
                {
                    var valueString = ((StringValue)arg2).Value;
                    if (!valueString.All(c => baseCharacters.Contains(c)))
                    {
                        throw new PangolinException($"For string base, value can't contain characters that aren't in base definition - base={arg1}, value={arg2}");
                    }

                    // Get indices from value string
                    var digitValues = valueString.Select(c => baseCharacters.IndexOf(c));

                    var result = ConvertFromIntegerBase(baseCharacters.Length, digitValues);
                    return new NumericValue(result);
                }
                // Array value, not defined
                else
                {
                    throw GetInvalidArgumentTypeException(nameof(BaseConversion), arg1.Type, arg2.Type);
                }
            }
            // Array, implies complex or higher dimensional base
            else
            {
                // TODO: Implement complex base conversions
                throw new PangolinInvalidArgumentTypeException($"Complex and higher dimensional bases not implemented yet - base={arg1}, value={arg2}");

                // arr, num -> real to complex base
                // arr, arr -> complex based number to decimal based complex
                // A different operator is required to convert a decimal base complex number to complex base
            }
        }

        public static IEnumerable<int> ConvertToIntegerBase(int newBase, double number)
        {
            var result = new List<int>();

            if (newBase == 0 || newBase == -1)
            {
                throw new PangolinException($"Conversion not possible with specified base {newBase}");
            }

            // Can't convert negative number into positive base
            // TODO: consider compliments?
            if (newBase > 0 && number < 0)
            {
                throw new PangolinException($"Can't convert negative number into positive base - newBase={newBase}, number={number}");
            }
            
            // Special case for unary
            if (newBase == 1)
            {
                if (((int)number) != number)
                {
                    throw new PangolinException($"Can't convert non-integral number into unary - newBase={newBase}, number={number}");
                }

                result.AddRange(Enumerable.Repeat(0, (int)number));
            }
            else
            {
                // Do the base conversion for the whole numbers part
                var intValue = (int)number;
                while (intValue != 0)
                {
                    var mod = intValue % newBase;
                    intValue /= newBase;

                    if (mod < 0) // Deal with negative base issues
                    {
                        mod -= newBase;
                        intValue++;
                    }

                    result.Add(mod);
                }

                var fractionalValue = number % 1;
                if (fractionalValue != 0)
                {
                    throw new PangolinException($"Base conversion of non-integral numbers not implemented yet - newBase={newBase}, number={number}");
                }
            }

            // Reverse, as base conversion builds from least -> most significant
            result.Reverse();

            return result;
        }

        public static double ConvertFromIntegerBase(int fromBase, IEnumerable<int> digits)
        {
            double result = 0;

            if (fromBase == 0 || fromBase == -1)
            {
                throw new PangolinException($"Can't convert from base {fromBase}");
            }

            if (fromBase == 1)
            {
                return digits.Count();
            }

            foreach (var d in digits)
            {
                result = (result * fromBase) + d;
            }

            return result;
        }
    }

    public class BinaryConversion : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u1E04";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            if (arg.Type == DataValueType.String)
            {
                throw GetInvalidArgumentTypeException(nameof(BinaryConversion), arg.Type);
            }
            // Numeric, convert to base 2
            else if (arg.Type == DataValueType.Numeric)
            {
                var result = BaseConversion.ConvertToIntegerBase(2, ((NumericValue)arg).Value);
                return new ArrayValue(result.Select(r => new NumericValue(r)));
            }
            // Array, convert from base 2
            else
            {
                var arrayArg = (ArrayValue)arg;

                if (arrayArg.Value.Any(a => a.Type != DataValueType.Numeric))
                {
                    throw new PangolinInvalidArgumentTypeException($"When converting to decimal from base 2, all digit values must be numeric - value={arg}");
                }

                var numericArgs = arrayArg.Value.Select(a => (NumericValue)a);

                if (!numericArgs.All(n => n.IsIntegral))
                {
                    throw new PangolinInvalidArgumentTypeException($"When converting to decimal from base 2, all digit values must be integral - value={arg}");
                }

                if (!numericArgs.All(n => n.IntValue == 0 || n.IntValue == 1))
                {
                    throw new PangolinInvalidArgumentTypeException($"When converting to decimal from base 2, all digit values must be less either 0 or 1 - value={arg}");
                }

                var result = BaseConversion.ConvertFromIntegerBase(2, numericArgs.Select(n => n.IntValue));
                return new NumericValue(result);
            }
        }
    }

    public class ArrayConstruction : BlockToken
    {
        public override bool BeginsBlock => true;

        public override bool EndsBlock => false;

        public override int Arity => 0;

        public ArrayConstruction() : base(0) { }

        public override DataValue Evaluate(ProgramState programState)
        {
            var elements = new List<DataValue>();

            // Elevate block level
            programState.IncreaseBlockLevel();

            // Dequeue until end of block or end of program reached
            while (programState.CurrentTokenIndex < programState.TokenList.Count &&
                !(programState.TokenList[programState.CurrentTokenIndex].Type == TokenType.Block && ((BlockToken)(programState.TokenList[programState.CurrentTokenIndex])).EndsBlock))
            {
                // Can't use block type token
                if (programState.TokenList[programState.CurrentTokenIndex].Type == TokenType.Block)
                {
                    throw new PangolinException($"Can't next block type token {programState.TokenList[programState.CurrentTokenIndex].ToString()} in {nameof(ArrayConstruction)}");
                }

                // Store the next value
                elements.Add(programState.DequeueAndEvaluate());
            }

            // Decrease block level
            programState.DecreaseBlockLevel();

            // Return array of elements
            return new ArrayValue(elements);
        }

        public override string ToString() => "[";
    }
}
