using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Range : ArityOneIterableToken
    {
        public override string ToString() => "\u2192";

        protected override DataValue EvaluateInner(DataValue argument)
        {
            if (argument.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)argument).IntValue;

                return new ArrayValue(
                    Enumerable
                        .Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Select(i => new NumericValue(i)));
            }
            // String/Array - get last element
            else
            {
                if (argument.IterationValues.Count == 0) throw new PangolinException($"Can't get last item of empty {argument.Type}");

                return argument.IterationValues[argument.IterationValues.Count - 1];
            }
        }
    }

    public class ReverseRange : ArityOneIterableToken
    {
        public override string ToString() => "\u2190";

        protected override DataValue EvaluateInner(DataValue argument)
        {
            if (argument.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)argument).IntValue;

                return new ArrayValue(
                    Enumerable
                        .Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Reverse()
                        .Select(i => new NumericValue(i)));
            }
            // String/Array - get first element
            else
            {
                if (argument.IterationValues.Count == 0) throw new PangolinException($"Can't get last item of empty {argument.Type}");

                return argument.IterationValues[0];
            }
        }
    }

    public class Range1 : ArityOneIterableToken
    {
        public override string ToString() => "\u0411";

        protected override DataValue EvaluateInner(DataValue argument)
        {
            if (argument.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)argument).IntValue;

                return new ArrayValue(
                    Enumerable
                        .Range(Math.Min(1, a), Math.Abs(a))
                        .Select(i => new NumericValue(i)));
            }
            else
            {
                throw GetInvalidArgumentTypeException(ToString(), argument.Type);
            }
        }
    }

    public class ReverseRange1 : ArityOneIterableToken
    {
        public override string ToString() => "\u042A";

        protected override DataValue EvaluateInner(DataValue argument)
        {
            if (argument.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)argument).IntValue;

                return new ArrayValue(
                    Enumerable
                        .Range(Math.Min(1, a), Math.Abs(a))
                        .Reverse()
                        .Select(i => new NumericValue(i)));
            }
            else
            {
                throw GetInvalidArgumentTypeException(ToString(), argument.Type);
            }
        }
    }
}
