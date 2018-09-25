using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class Pair : FunctionToken // TODO: Write tests
    {
        public override int Arity => 2;

        private IReadOnlyList<DataValue> _iterationValueSet;

        public Pair() : base(4) { }
        
        public override string ToString() => "P";

        protected override ProgramState.IterationConstantDetails GetDefaultToken(IReadOnlyList<ProgramState.IterationConstantDetails> allocatedTokens) => allocatedTokens[0];

        protected override DataValue ProcessResults(IReadOnlyList<IterationResultContainer> results)
        {
            return new ArrayValue(results.Select(r => r.IterationResult));
        }

        protected override int RetrieveFunctionArguments(ProgramState programState)
        {
            var iterationValue = programState.DequeueAndEvaluate();

            // Get iteration value set
            if (iterationValue.Type == DataValueType.Numeric)
            {
                var a = ((NumericValue)iterationValue).IntValue;

                _iterationValueSet = Enumerable.Range(Math.Min(0, a + 1), Math.Abs(a))
                        .Select(i => new NumericValue(i))
                        .ToList();
            }
            else
            {
                _iterationValueSet = iterationValue.IterationValues;
            }

            return _iterationValueSet.Count - 1;
        }

        protected override void SetIterationConstants(ProgramState programState, IReadOnlyList<ProgramState.IterationConstantDetails> allocatedTokens, int iterationIndex)
        {
            programState.SetIterationFunctionConstant(allocatedTokens[0], _iterationValueSet[iterationIndex]);
            programState.SetIterationFunctionConstant(allocatedTokens[1], _iterationValueSet[iterationIndex + 1]);
            programState.SetIterationFunctionConstant(allocatedTokens[2], new NumericValue(iterationIndex));
            programState.SetIterationFunctionConstant(allocatedTokens[3], new NumericValue(iterationIndex));
        }
    }

    public class SimplePair : IterableToken
    {
        public override int Arity => 1;
        public override string ToString() => ",";

        protected override DataValue EvaluateInner(IReadOnlyList<DataValue> arguments)
        {
            var arg = arguments[0];

            // Only implemented for non numerics
            if (arg.Type == DataValueType.Numeric)
            {
                throw GetInvalidArgumentTypeException(ToString(), arg.Type);
            }

            var elements = arg.IterationValues;
            var results = new List<DataValue>();

            for (int i = 0; i < elements.Count - 1; i++)
            {
                results.Add(new ArrayValue(elements[i], elements[i + 1]));
            }

            return new ArrayValue(results);
        }
    }
}
