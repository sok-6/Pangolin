using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Range : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u2192";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            if (arg.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)arg).IntValue;

                return new ArrayValue(
                    Enumerable
                        .Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Select(i => new NumericValue(i)));
            }
            // String/Array - get last element
            else
            {
                if (arg.IterationValues.Count == 0) throw new PangolinException($"Can't get last item of empty {arg.Type}");

                return arg.IterationValues[arg.IterationValues.Count - 1];
            }
        }
    }

    public class ReverseRange : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u2190";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            if (arg.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)arg).IntValue;

                return new ArrayValue(
                    Enumerable
                        .Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Reverse()
                        .Select(i => new NumericValue(i)));
            }
            // String/Array - get first element
            else
            {
                if (arg.IterationValues.Count == 0) throw new PangolinException($"Can't get last item of empty {arg.Type}");

                return arg.IterationValues[0];
            }
        }
    }

    public class Range1 : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u0411";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            if (arg.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)arg).IntValue;

                return new ArrayValue(
                    Enumerable
                        .Range(Math.Min(1, a), Math.Abs(a))
                        .Select(i => new NumericValue(i)));
            }
            else
            {
                throw GetInvalidArgumentTypeException(ToString(), arg.Type);
            }
        }
    }

    public class ReverseRange1 : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u042A";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            if (arg.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)arg).IntValue;

                return new ArrayValue(
                    Enumerable
                        .Range(Math.Min(1, a), Math.Abs(a))
                        .Reverse()
                        .Select(i => new NumericValue(i)));
            }
            else
            {
                throw GetInvalidArgumentTypeException(ToString(), arg.Type);
            }
        }
    }
}
