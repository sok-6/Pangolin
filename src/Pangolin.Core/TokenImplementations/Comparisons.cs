﻿using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public abstract class EqualityBase : Token
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
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            var a = programState.DequeueAndEvaluate();
            var b = programState.DequeueAndEvaluate();

            return AreEqual(a, b) ? DataValue.Truthy : DataValue.Falsey;
        }

        public override string ToString() => "=";
    }

    public class Inequality : EqualityBase
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            var a = programState.DequeueAndEvaluate();
            var b = programState.DequeueAndEvaluate();

            return AreEqual(a, b) ? DataValue.Falsey : DataValue.Truthy;
        }

        public override string ToString() => "\u2260";
    }
}