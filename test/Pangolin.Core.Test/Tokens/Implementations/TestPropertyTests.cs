using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Core.TokenImplementations;
using Shouldly;
using Xunit;

namespace Pangolin.Core.Test.Tokens.Implementations
{
    public class TestPropertyTests
    {
        [Fact]
        public void IsEven_should_correctly_identify_parity_of_integrals()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(5);
            var mockProgramState2 = MockFactory.MockProgramState(100000000000);
            var mockProgramState3 = MockFactory.MockProgramState(0);
            var mockProgramState4 = MockFactory.MockProgramState(-7);

            var token = new IsEven();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void IsEven_should_return_falsey_for_floats()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(1.2);
            var mockProgramState2 = MockFactory.MockProgramState(-100.1);

            var token = new IsEven();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void IsEven_should_correctly_identify_parity_of_string_length()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abc");
            var mockProgramState2 = MockFactory.MockProgramState("");

            var token = new IsEven();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void IsEven_should_correctly_identify_parity_of_array_length()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new IsEven();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void IsOdd_should_correctly_identify_parity_of_integrals()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(5);
            var mockProgramState2 = MockFactory.MockProgramState(100000000000);
            var mockProgramState3 = MockFactory.MockProgramState(0);
            var mockProgramState4 = MockFactory.MockProgramState(-7);

            var token = new IsOdd();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void IsOdd_should_return_falsey_for_floats()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(1.2);
            var mockProgramState2 = MockFactory.MockProgramState(-100.1);

            var token = new IsOdd();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void IsOdd_should_correctly_identify_parity_of_string_length()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abc");
            var mockProgramState2 = MockFactory.MockProgramState("");

            var token = new IsOdd();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void IsOdd_should_correctly_identify_parity_of_array_length()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new IsOdd();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }
    }
}
