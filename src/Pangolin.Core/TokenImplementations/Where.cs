using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pangolin.Core.TokenImplementations
{
    public class Where : Token
    {
        public override int Arity => 2;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Check if already in where block
            if (programState.IsWhereBlockExecuting)
            {
                throw new PangolinException("Where token encountered in where block");
            }

            // Make note of current index to return to 
            var whereIndex = programState.CurrentTokenIndex;

            // Find index of end of function block
            var blockEnd = programState.FindEndOfBlock(whereIndex);

            // Set execution index to end of function block and get values to execute where over
            programState.SetCurrentTokenIndex(blockEnd + 1);
            var whereValues = programState.DequeueAndEvaluate();

            // Make note of end of 2nd argument end index
            var endIndex = programState.CurrentTokenIndex;

            // Interpret whereValues, get individual values where required
            var innerValues = new List<DataValue>();

            // Numeric, create range first
            if (whereValues.Type == DataValueType.Numeric)
            {
                var numValue = whereValues as NumericValue;

                // Can only do it if integral
                if (!numValue.IsIntegral)
                {
                    throw new PangolinInvalidArgumentTypeException("Where cannot accept non-integral numeric as 2nd argument");
                }

                innerValues.AddRange(
                    Enumerable
                        .Range(Math.Min(0, numValue.IntValue + 1), Math.Abs(numValue.IntValue))
                        .Select(i => new NumericValue(i)));
            }
            // String, separate into single length strings
            else if (whereValues.Type == DataValueType.String)
            {
                innerValues.AddRange(
                    ((StringValue)whereValues).Value
                    .Select(c => new StringValue(c.ToString())));
            }
            // Array, just take values
            else
            {
                innerValues.AddRange(((ArrayValue)whereValues).Value);
            }

            // Execute where block for each value in turn
            var result = new List<DataValue>();
            programState.IsWhereBlockExecuting = true;

            innerValues.ForEach(v =>
            {
                programState.SetCurrentTokenIndex(whereIndex);
                programState.WhereValue = v;

                var r = programState.DequeueAndEvaluate();

                // Only add v to result set if r is truthy
                if (r.IsTruthy)
                {
                    result.Add(v);
                }
            });

            // Set token index to end of 2nd argument, wrap filtered results in array value and return
            programState.IsWhereBlockExecuting = false;
            programState.SetCurrentTokenIndex(endIndex);
            return new ArrayValue(result);
        }

        public override string ToString() => "W";
    }

    public class WhereValue : Token
    {
        public override int Arity => 0;

        public override DataValue Evaluate(ProgramState programState)
        {
            if (programState.WhereValue == null)
            {
                throw new PangolinInvalidTokenException("WhereValue token encountered outside of where block");
            }

            return programState.WhereValue;
        }

        public override string ToString() => "w";
    }
}
