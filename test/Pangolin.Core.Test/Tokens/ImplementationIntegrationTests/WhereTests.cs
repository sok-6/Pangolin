using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Pangolin.Core.TokenImplementations;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Common;

namespace Pangolin.Core.Test.Tokens.ImplementationIntegrationTests
{
    public class WhereTests
    {
        [Fact]
        public void Where_should_filter_array()
        {
            // Arrange
            var programState = new ProgramState(
                new List<DataValue>()
                {
                    new NumericValue(5),
                    new NumericValue(20),
                    new NumericValue(9),
                    new NumericValue(15)
                },
                new List<Token>()
                {
                    new Where(),
                    new LessThan(),
                    new NumericLiteral(10),
                    new WhereIterationVariable0(),
                    new ArgumentArray()
                });

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBeArrayWhichStartsWith(20, 15);
        }

        [Fact]
        public void Where_should_filter_string()
        {
            // Arrange
            var programState = new ProgramState(
                new List<DataValue>(),
                new List<Token>()
                {
                    new Where(),
                    new Membership(),
                    new WhereIterationVariable0(),
                    new StringLiteral("aeiou"),
                    new StringLiteral("it was the best of times, it was the worst of times")
                });

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBeArrayWhichStartsWith("i", "a", "e", "e", "o", "i", "e", "i", "a", "e", "o", "o", "i", "e");
        }

        [Fact]
        public void Where_should_filter_range_up_to_numeric_value()
        {
            // Arrange
            var programState = new ProgramState(
                new List<DataValue>(),
                new List<Token>()
                {
                    new Where(),
                    new Modulo(),
                    new NumericLiteral(3),
                    new WhereIterationVariable0(),
                    new NumericLiteral(10)
                });

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBeArrayWhichStartsWith(1, 2, 4, 5, 7, 8);
        }

        [Fact(Skip = "Need to come up with good test")]
        public void Where_should_be_allowed_to_be_nested()
        {
            
        }
    }
}
