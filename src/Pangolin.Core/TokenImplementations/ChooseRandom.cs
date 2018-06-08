﻿using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.TokenImplementations
{
    public class ChooseRandom : Token
    {
        public override int Arity => 1;

        public override DataValue Evaluate(ProgramState programState)
        {
            // Get argument
            var arg = programState.DequeueAndEvaluate();

            // Numeric, random number
            if (arg.Type == DataValueType.Numeric)
            {
                var numericArg = (NumericValue)arg;

                if (numericArg.IsIntegral)
                {
                    return new NumericValue(RandomSource.IntBetweenZeroAnd(numericArg.IntValue));
                }
                else
                {
                    return new NumericValue(numericArg.Value * RandomSource.RandomDecimal());
                }
            }
            // String, random character
            else if (arg.Type == DataValueType.String)
            {
                var stringArg = (StringValue)arg;

                var chosenCharacter = RandomSource.Choose(stringArg.Value);

                return new StringValue(chosenCharacter.ToString());
            }
            // Array, random element
            else
            {
                var arrayArg = (ArrayValue)arg;

                return RandomSource.Choose(arrayArg.Value);
            }
        }

        public override string ToString() => "R";
    }
}