using Pangolin.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core
{
    public abstract class DataValue
    {
        public abstract DataValueType Type { get; }
        public abstract bool IsTruthy { get; }
        public abstract override string ToString();

        public DataValue Truthify()
        {
            return new DataValueImplementations.NumericValue(this.IsTruthy ? 1 : 0);
        }

        public virtual bool IterationRequired { get; protected set; } = false;

        public virtual void SetIterationRequired(bool iterationRequired)
        {
            throw new PangolinException($"Iteration not implemented on data type {Type}");
        }

        public virtual IReadOnlyList<DataValue> IterationValues { get { throw new PangolinException($"Iteration not implemented on data type {Type}"); } }
        
        public static DataValue Truthy => new DataValueImplementations.NumericValue(1);
        public static DataValue Falsey => new DataValueImplementations.NumericValue(0);
        public static DataValue BoolToTruthiness(bool b) => new DataValueImplementations.NumericValue(b ? 1 : 0);
    }
}
