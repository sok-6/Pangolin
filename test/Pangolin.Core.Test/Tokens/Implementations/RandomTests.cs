using Moq;
using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Core.TokenImplementations;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pangolin.Core.Test.Tokens.Implementations
{
    public class RandomTests
    {
        [Fact]
        public void GetRandomDecimal_should_return_numeric_less_than_1()
        {
            // Arrange
            var mockProgramState = new Mock<ProgramState>();

            var token = new GetRandomDecimal();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBeGreaterThanOrEqualTo(0);
            numericResult.Value.ShouldBeLessThan(1);
        }

        [Fact]
        public void ChooseRandom_should_choose_random_element_of_array()
        {
            // Arrange
            var mockArrayValue = MockFactory.MockArrayBuilder.StartingNumerics(0, 1, 2).Complete();
            var mockProgramState = MockFactory.MockProgramState(mockArrayValue);

            var token = new ChooseRandom();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOneOf(mockArrayValue.Value.ToArray());
        }

        [Fact]
        public void ChooseRandom_should_choose_random_character_from_string()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object);

            var token = new ChooseRandom();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBeOneOf("a", "b", "c");
        }

        [Fact]
        public void ChooseRandom_should_choose_random_integral_less_than_argument_when_positive_integral_provided()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockNumericValue(10).Object);

            var token = new ChooseRandom();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var resultValue = result.ShouldBeOfType<NumericValue>().Value;
            resultValue.ShouldBe((int)resultValue); // Check integral
            resultValue.ShouldBeGreaterThanOrEqualTo(0);
            resultValue.ShouldBeLessThan(10);
        }

        [Fact]
        public void ChooseRandom_should_choose_random_integral_greater_than_argument_when_negative_integral_provided()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockNumericValue(-10).Object);

            var token = new ChooseRandom();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var resultValue = result.ShouldBeOfType<NumericValue>().Value;
            resultValue.ShouldBe((int)resultValue); // Check integral
            resultValue.ShouldBeLessThanOrEqualTo(0);
            resultValue.ShouldBeGreaterThan(-10);
        }

        [Fact]
        public void ChooseRandom_should_choose_random_number_less_than_argument_when_positive_float_provided()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockNumericValue(5.5).Object);

            var token = new ChooseRandom();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var resultValue = result.ShouldBeOfType<NumericValue>().Value;
            resultValue.ShouldBeGreaterThanOrEqualTo(0);
            resultValue.ShouldBeLessThan(5.5);
        }

        [Fact]
        public void ChooseRandom_should_choose_random_number_greater_than_argument_when_negative_float_provided()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockNumericValue(-5.5).Object);

            var token = new ChooseRandom();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var resultValue = result.ShouldBeOfType<NumericValue>().Value;
            resultValue.ShouldBeLessThanOrEqualTo(0);
            resultValue.ShouldBeGreaterThan(-5.5);
        }
    }
}
