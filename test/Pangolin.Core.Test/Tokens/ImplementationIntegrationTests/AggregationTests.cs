using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Core.TokenImplementations;
using Shouldly;
using Xunit;

namespace Pangolin.Core.Test.Tokens.ImplementationIntegrationTests
{
    public class AggregationTests
    {
        [Fact]
        public void AggregateFirst_should_aggregate_array()
        {
            // Arrange
            var programState = new ProgramState(
                new DataValue[]
                {
                    new NumericValue(2),
                    new StringValue("abc"),
                    new NumericValue(5),
                    new StringValue("d")
                },
                new Token[]
                {
                    new AggregateFirst(),
                    new Add(),
                    new TokenImplementations.Double(),
                    new AggregateFirstVariableConstantCurrent(),
                    new AggregateFirstVariableConstantNext(),
                    new ArgumentArray()
                });

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("4abc4abc54abc4abc5d");
        }

        [Fact]
        public void AggregateFirst_should_aggregate_string()
        {
            // Arrange
            var programState = new ProgramState(
                new DataValue[]
                {

                },
                new Token[]
                {
                    new AggregateFirst(),
                    new Add(),
                    new TokenImplementations.Double(),
                    new AggregateFirstVariableConstantCurrent(),
                    new AggregateFirstVariableConstantNext(),
                    new StringLiteral("abcd")
                });

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("aabaabcaabaabcd");
        }

        [Fact]
        public void AggregateFirst_should_aggregate_integral()
        {
            // Arrange
            var programState = new ProgramState(
                new DataValue[]
                {

                },
                new Token[]
                {
                    new AggregateFirst(),
                    new Add(),
                    new TokenImplementations.Double(),
                    new AggregateFirstVariableConstantCurrent(),
                    new AggregateFirstVariableConstantNext(),
                    new NumericLiteral(5)
                });

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(26);
        }

        [Fact]
        public void CollapseFunction_should_aggregate_array()
        {
            // Arrange
            var programState = new ProgramState(
                new DataValue[]
                {
                    new NumericValue(2),
                    new StringValue("abc"),
                    new NumericValue(5),
                    new StringValue("d")
                },
                new Token[]
                {
                    new CollapseFunction(),
                    new Add(),
                    new TokenImplementations.Double(),
                    new CollapseFunctionVariableConstantCurrent(),
                    new CollapseFunctionVariableConstantNext(),
                    new NumericLiteral(3),
                    new ArgumentArray()
                });

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("16abc16abc516abc16abc5d");
        }

        [Fact]
        public void CollapseFunction_should_aggregate_string()
        {
            // Arrange
            var programState = new ProgramState(
                new DataValue[]
                {

                },
                new Token[]
                {
                    new CollapseFunction(),
                    new Add(),
                    new TokenImplementations.Double(),
                    new CollapseFunctionVariableConstantCurrent(),
                    new CollapseFunctionVariableConstantNext(),
                    new NumericLiteral(3),
                    new StringLiteral("abcd")
                });

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("6a6ab6a6abc6a6ab6a6abcd");
        }

        [Fact]
        public void CollapseFunction_should_aggregate_integral()
        {
            // Arrange
            var programState = new ProgramState(
                new DataValue[]
                {

                },
                new Token[]
                {
                    new CollapseFunction(),
                    new Add(),
                    new TokenImplementations.Double(),
                    new CollapseFunctionVariableConstantCurrent(),
                    new CollapseFunctionVariableConstantNext(),
                    new NumericLiteral(3),
                    new NumericLiteral(5)
                });

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(122);
        }
    }
}
