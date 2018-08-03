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
        public static bool AreEqual(DataValue a, DataValue b)
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
            return DataValue.BoolToTruthiness(AreEqual(arg1, arg2));
        }
    }

    public class Inequality : EqualityBase
    {
        public override string ToString() => "\u2260";

        protected override DataValue EvaluateInner(DataValue arg1, DataValue arg2)
        {
            return DataValue.BoolToTruthiness(!AreEqual(arg1, arg2));
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

                return DataValue.BoolToTruthiness(numeric1 < numeric2);
            }
            else
            {
                throw GetInvalidArgumentTypeException(ToString(), arg1.Type, arg2.Type);
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

                return DataValue.BoolToTruthiness(numeric1 > numeric2);
            }
            else
            {
                throw GetInvalidArgumentTypeException(ToString(), arg1.Type, arg2.Type);
            }
        }
    }

    public class IteratedEquality : Equality
    {
        public override string ToString() => "\u229C";

        /* 
+=========+==================================+=============================================================+==================================================+
|  Type   |             Numeric              |                           String                            |                    Array                         |
+=========+==================================+=============================================================+==================================================+
| Numeric | Exception                        | Num-> string, iterate over string                           | iterate over array                               |
+---------+----------------------------------+-------------------------------------------------------------+--------------------------------------------------+
| String  | num->string, iterate over string | zip                                                         | zip                                              |
+---------+----------------------------------+-------------------------------------------------------------+--------------------------------------------------+
| Array   | iterate over array               | zip                                                         | zip                                              |
+---------+----------------------------------+-------------------------------------------------------------+--------------------------------------------------+
         */
        public override DataValue Evaluate(ProgramState tokenQueue)
        {
            // Get two arguments
            var arg1 = tokenQueue.DequeueAndEvaluate();
            var arg2 = tokenQueue.DequeueAndEvaluate();

            if (arg1.Type == DataValueType.Numeric)
            {
                // Numeric, string -> a1 to string, iterate over a2
                if (arg2.Type == DataValueType.String)
                {
                    var str1 = new StringValue(arg1.ToString());
                    return new ArrayValue(arg2.IterationValues.Select(s => EvaluateInner(str1, s)));
                }
                // Numeric, array -> iterate over a2
                else if (arg2.Type == DataValueType.Array)
                {
                    return new ArrayValue(arg2.IterationValues.Select(a => EvaluateInner(arg1, a)));
                }
                // Numeric, numeric -> nothing
                else
                {
                    throw GetInvalidArgumentTypeException(ToString(), arg1.Type, arg2.Type);
                }
            }
            else if (arg2.Type == DataValueType.Numeric)
            {
                // String, Numeric -> a2 to string, iterate over a1
                if (arg1.Type == DataValueType.String)
                {
                    var str2 = new StringValue(arg2.ToString());
                    return new ArrayValue(arg1.IterationValues.Select(s => EvaluateInner(s, str2)));
                }
                // Array, Numeric -> iterate over a1
                else if (arg1.Type == DataValueType.Array)
                {
                    return new ArrayValue(arg1.IterationValues.Select(a => EvaluateInner(a, arg2)));
                }
                // Numeric, numeric -> nothing
                else
                {
                    throw GetInvalidArgumentTypeException(ToString(), arg1.Type, arg2.Type);
                }
            }
            // Both a1 and a2 are iterable, zip them
            else
            {
                return new ArrayValue(arg1.IterationValues.Zip(arg2.IterationValues, (a1, a2) => EvaluateInner(a1, a2)));
            }
        }
    }
}
