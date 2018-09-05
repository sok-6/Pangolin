using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Join : IterableToken
    {
        public override int Arity => 2;
        public override string ToString() => "J";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg1 = arguments[0];
            var arg2 = arguments[1];

            // Not defined yet if set is numeric
            if (arg2.Type == DataValueType.Numeric)
            {
                throw GetInvalidArgumentTypeException(nameof(Join), arg1.Type, arg2.Type);
            }

            IEnumerable<string> joinOn;

            // Joining on array, join on each in turn
            if (arg1.Type == DataValueType.Array)
            {
                joinOn = arg1.IterationValues.Select(v => v.ToString());
            }
            // Joining on numeric or array, get string representation
            else
            {
                joinOn = new String[] { arg1.ToString() };
            }

            return new StringValue(ExecuteJoin(joinOn, arg2.IterationValues.Select(v => v.ToString())));
        }

        public static string ExecuteJoin(IEnumerable<string> joinOn, IEnumerable<string> joiningSet)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < joiningSet.Count(); i++)
            {
                // If not the 1st element in the set, add if there are any values to join on
                if (joinOn.Count() > 0 && i > 0)
                {
                    sb.Append(joinOn.Skip((i - 1) % joinOn.Count()).First());
                }

                sb.Append(joiningSet.Skip(i).First());
            }

            return sb.ToString();
        }
    }

    public class JoinOnSpaces : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u1E62";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            if (arg.Type == DataValueType.Numeric)
            {
                throw GetInvalidArgumentTypeException(nameof(JoinOnSpaces), arg.Type);
            }

            return new StringValue(Join.ExecuteJoin(new string[] { " " }, arg.IterationValues.Select(v => v.ToString())));
        }
    }

    public class JoinOnNewlines : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => "\u1E46";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            if (arg.Type == DataValueType.Numeric)
            {
                throw GetInvalidArgumentTypeException(nameof(JoinOnNewlines), arg.Type);
            }

            return new StringValue(Join.ExecuteJoin(new string[] { "\n" }, arg.IterationValues.Select(v => v.ToString())));
        }
    }
}
