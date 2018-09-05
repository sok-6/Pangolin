using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class LogicAnd : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get 1st operand
            var op1 = programState.DequeueAndEvaluate();

            // If falsey, no need to evaluate 2nd operand
            if (!op1.IsTruthy)
            {
                programState.StepOverNextTokenBlock();
                return DataValue.Falsey;
            }
            else
            {
                var op2 = programState.DequeueAndEvaluate();
                return DataValue.BoolToTruthiness(op2.IsTruthy);
            }
        }

        public override string ToString() => "&";
    }

    public class LogicOr : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get 1st operand
            var op1 = programState.DequeueAndEvaluate();

            // If truthy, no need to evaluate 2nd operand
            if (op1.IsTruthy)
            {
                programState.StepOverNextTokenBlock();
                return DataValue.Truthy;
            }
            else
            {
                var op2 = programState.DequeueAndEvaluate();
                return DataValue.BoolToTruthiness(op2.IsTruthy);
            }
        }

        public override string ToString() => "|";
    }

    public class LogicXor : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            var op1 = programState.DequeueAndEvaluate();
            var op2 = programState.DequeueAndEvaluate();

            return DataValue.BoolToTruthiness(op1.IsTruthy ^ op2.IsTruthy);
        }

        public override string ToString() => "X";
    }

    public class LogicXnor : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            var op1 = programState.DequeueAndEvaluate();
            var op2 = programState.DequeueAndEvaluate();

            return DataValue.BoolToTruthiness(!(op1.IsTruthy ^ op2.IsTruthy));
        }

        public override string ToString() => "~";
    }

    public class Disjunction_ToLower : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "V";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // String, tolower
            if (arg.Type == DataValueType.String)
            {
                return new StringValue(((StringValue)arg).Value.ToLower());
            }
            // Numeric, check if any are nonzero
            else if (arg.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)arg;

                if (!numericArg.IsIntegral)
                {
                    throw new PangolinInvalidArgumentTypeException($"{nameof(Disjunction_ToLower)} not defined for float values - arg={arg}");
                }

                if (numericArg.Value < 0)
                {
                    throw new PangolinInvalidArgumentTypeException($"{nameof(Disjunction_ToLower)} not defined for negative values - arg={arg}");
                }

                // If 0, that's the only digit
                if (numericArg.Value == 0)
                {
                    return DataValue.Falsey;
                }
                else
                {
                    return DataValue.BoolToTruthiness(BaseConversion.ConvertToIntegerBase(10, numericArg.Value).Any(d => d != 0));
                }
            }
            // Array, just get elements
            else
            {
                // Empty array is always falsey
                if (arg.IterationValues.Count == 0)
                {
                    return DataValue.Falsey;
                }

                return DataValue.BoolToTruthiness(((ArrayValue)arg).Value.Any(e => e.IsTruthy));
            }
        }
    }

    public class Conjunction_ToUpper : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u039B";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // String, tolower
            if (arg.Type == DataValueType.String)
            {
                return new StringValue(((StringValue)arg).Value.ToUpper());
            }
            // Numeric, check if any are nonzero
            else if (arg.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)arg;

                if (!numericArg.IsIntegral)
                {
                    throw new PangolinInvalidArgumentTypeException($"{nameof(Conjunction_ToUpper)} not defined for float values - arg={arg}");
                }

                if (numericArg.Value < 0)
                {
                    throw new PangolinInvalidArgumentTypeException($"{nameof(Conjunction_ToUpper)} not defined for negative values - arg={arg}");
                }

                // If 0, that's the only digit
                if (numericArg.Value == 0)
                {
                    return DataValue.Falsey;
                }
                else
                {
                    return DataValue.BoolToTruthiness(BaseConversion.ConvertToIntegerBase(10, numericArg.Value).All(d => d != 0));
                }
            }
            // Array, just get elements
            else
            {
                // Empty array is always falsey
                if (arg.IterationValues.Count == 0)
                {
                    return DataValue.Falsey;
                }

                return DataValue.BoolToTruthiness(((ArrayValue)arg).Value.All(e => e.IsTruthy));
            }
        }
    }

    public class InverseDisjunction : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u22BC";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // String, tolower
            if (arg.Type == DataValueType.String)
            {
                throw GetInvalidArgumentTypeException(nameof(InverseDisjunction), arg.Type);
            }
            // Numeric, check if any are nonzero
            else if (arg.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)arg;

                if (!numericArg.IsIntegral)
                {
                    throw new PangolinInvalidArgumentTypeException($"{nameof(InverseDisjunction)} not defined for float values - arg={arg}");
                }

                if (numericArg.Value < 0)
                {
                    throw new PangolinInvalidArgumentTypeException($"{nameof(InverseDisjunction)} not defined for negative values - arg={arg}");
                }

                // If 0, that's the only digit
                if (numericArg.Value == 0)
                {
                    return DataValue.Truthy;
                }
                else
                {
                    return DataValue.BoolToTruthiness(BaseConversion.ConvertToIntegerBase(10, numericArg.Value).Any(d => d == 0));
                }
            }
            // Array, just get elements
            else
            {
                // Empty array is always falsey
                if (arg.IterationValues.Count == 0)
                {
                    return DataValue.Falsey;
                }

                return DataValue.BoolToTruthiness(((ArrayValue)arg).Value.Any(e => !e.IsTruthy));
            }
        }
    }

    public class InverseConjunction : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u22BD";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // String, tolower
            if (arg.Type == DataValueType.String)
            {
                throw GetInvalidArgumentTypeException(nameof(InverseConjunction), arg.Type);
            }
            // Numeric, check if any are nonzero
            else if (arg.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)arg;

                if (!numericArg.IsIntegral)
                {
                    throw new PangolinInvalidArgumentTypeException($"{nameof(InverseConjunction)} not defined for float values - arg={arg}");
                }

                if (numericArg.Value < 0)
                {
                    throw new PangolinInvalidArgumentTypeException($"{nameof(InverseConjunction)} not defined for negative values - arg={arg}");
                }

                // If 0, that's the only digit
                if (numericArg.Value == 0)
                {
                    return DataValue.Truthy;
                }
                else
                {
                    return DataValue.BoolToTruthiness(BaseConversion.ConvertToIntegerBase(10, numericArg.Value).All(d => d == 0));
                }
            }
            // Array, just get elements
            else
            {
                // Empty array is always falsey
                if (arg.IterationValues.Count == 0)
                {
                    return DataValue.Falsey;
                }

                return DataValue.BoolToTruthiness(((ArrayValue)arg).Value.All(e => !e.IsTruthy));
            }
        }
    }
}
