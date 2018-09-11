using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Core.TokenImplementations;
using Xunit;

namespace Pangolin.Core.Test.Tokens.ImplementationIntegrationTests
{
    public class FlowControlIntegrationTests
    {
        [Fact]
        public void ConditionalApply_should_return_argument_if_conditional_falsey()
        {
            // Arrange
            var programState = new ProgramState(
                new DataValue[]
                {

                },
                new Token[]
                {
                    new ConditionalApply(),
                    new TokenImplementations.Double(),
                    new NumericLiteral(0),
                    new NumericLiteral(2)
                });

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(2);
        }

        [Fact]
        public void ConditionalApply_should_return_token_result_if_conditional_truthy()
        {
            // Arrange
            var programState = new ProgramState(
                new DataValue[]
                {

                },
                new Token[]
                {
                    new ConditionalApply(),
                    new TokenImplementations.Double(),
                    new NumericLiteral(1),
                    new NumericLiteral(2)
                });

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(4);
        }

        [Fact]
        public void ConditionalApply_should_throw_exception_if_argument_token_not_arity_1()
        {
            // Arrange
            var programState1 = new ProgramState(
                new DataValue[]
                {

                },
                new Token[]
                {
                    new ConditionalApply(),
                    new TokenImplementations.Add(),
                    new NumericLiteral(1),
                    new NumericLiteral(2)
                });
            var programState2 = new ProgramState(
                new DataValue[]
                {

                },
                new Token[]
                {
                    new ConditionalApply(),
                    new TokenImplementations.ConstantEmptyString(),
                    new NumericLiteral(1),
                    new NumericLiteral(2)
                });

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => programState1.DequeueAndEvaluate()).Message.ShouldBe("Token \u00BF first argument must be arity 1 - + is arity 2");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => programState2.DequeueAndEvaluate()).Message.ShouldBe("Token \u00BF first argument must be arity 1 - e is arity 0");
        }

        [Fact(Skip = "Need at least 1 implementation of types block, function led, token led")]
        public void ConditionalApply_should_throw_exception_if_argument_token_of_incorrect_base_type()
        {
            
        }
    }
}
