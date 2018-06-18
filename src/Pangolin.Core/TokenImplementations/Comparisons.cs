using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public abstract class EqualityBase : ArityTwoIterableToken
    {
        protected static bool AreEqual(DataValue a, DataValue b)
        {
            if (a.Type != b.Type)
            {
                return false;
            }
            else if (a.Type == DataValueType.Numeric)
            {
                return ((NumericValue)a).Value == ((NumericValue)b).Value;
            }
            else if (a.Type == DataValueType.String)
            {
                return ((StringValue)a).Value == ((StringValue)b).Value;
            }
            else
            {
                var arrayA = ((ArrayValue)a);
                var arrayB = ((ArrayValue)b);

                if (arrayA.Value.Count != arrayB.Value.Count)
                {
                    return false;
                }
                else
                {
                    for (int i = 0; i < arrayA.Value.Count; i++)
                    {
                        if (!AreEqual(arrayA.Value[i], arrayB.Value[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }
        }
    }

    public class Equality : EqualityBase
    {
        public override string ToString() => "=";

        protected override DataValue EvaluateInner(DataValue arg1, DataValue arg2)
        {
            return AreEqual(arg1, arg2) ? DataValue.Truthy : DataValue.Falsey;
        }
    }

    public class Inequality : EqualityBase
    {
        public override string ToString() => "\u2260";

        protected override DataValue EvaluateInner(DataValue arg1, DataValue arg2)
        {
            return AreEqual(arg1, arg2) ? DataValue.Falsey : DataValue.Truthy;
        }
    }

    public class LessThan : ArityTwoIterableToken
    {
        public override string ToString() => "<";

        protected override DataValue EvaluateInner(DataValue arg1, DataValue arg2)
        {
            // Numeric comparison
            if (arg1.Type == DataValueType.Numeric && arg2.Type == DataValueType.Numeric)
            {
                var numeric1 = ((NumericValue)arg1).Value;
                var numeric2 = ((NumericValue)arg2).Value;

                return numeric1 < numeric2 ? DataValue.Truthy : DataValue.Falsey;
            }
            else
            {
                throw GetInvalidArgumentTypeException(arg1.Type, arg2.Type);
            }
        }
    }

    public class GreaterThan : ArityTwoIterableToken
    {
        public override string ToString() => ">";

        protected override DataValue EvaluateInner(DataValue arg1, DataValue arg2)
        {
            // Numeric comparison
            if (arg1.Type == DataValueType.Numeric && arg2.Type == DataValueType.Numeric)
            {
                var numeric1 = ((NumericValue)arg1).Value;
                var numeric2 = ((NumericValue)arg2).Value;

                return numeric1 > numeric2 ? DataValue.Truthy : DataValue.Falsey;
            }
            else
            {
                throw GetInvalidArgumentTypeException(arg1.Type, arg2.Type);
            }
        }
    }
}
