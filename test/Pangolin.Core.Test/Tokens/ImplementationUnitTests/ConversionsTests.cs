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

namespace Pangolin.Core.Test.Tokens.ImplementationUnitTests
{
    public class ConversionsTests
    {
        [Fact]
        public void Arrayify_should_wrap_any_value_in_an_array()
        {
            // Arrange
            var mockDataValue = MockFactory.MockDataValue().Object;

            var mockProgramState = MockFactory.MockProgramState(mockDataValue);

            var token = new Arrayify();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(mockDataValue);
        }

        [Fact]
        public void ArrayPair_should_wrap_any_two_values_in_an_array()
        {
            // Arrange
            var mockDataValue1 = MockFactory.MockDataValue().Object;
            var mockDataValue2 = MockFactory.MockDataValue().Object;

            var mockProgramState = MockFactory.MockProgramState(mockDataValue1, mockDataValue2);

            var token = new ArrayPair();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(mockDataValue1, mockDataValue2);
        }

        [Fact]
        public void ArrayTriple_should_wrap_any_three_values_in_an_array()
        {
            // Arrange
            var mockDataValue1 = MockFactory.MockDataValue().Object;
            var mockDataValue2 = MockFactory.MockDataValue().Object;
            var mockDataValue3 = MockFactory.MockDataValue().Object;

            var mockProgramState = MockFactory.MockProgramState(mockDataValue1, mockDataValue2, mockDataValue3);

            var token = new ArrayTriple();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(mockDataValue1, mockDataValue2, mockDataValue3);
        }

        [Fact]
        public void Elements_should_get_digits_of_positive_integral()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(1234);

            var token = new Elements();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(1, 2, 3, 4);
        }

        [Fact]
        public void Elements_should_throw_exception_for_negatvie_or_non_integral_numerics()
        {
            // Arrange
            var ps1 = MockFactory.MockProgramState(-5);
            var ps2 = MockFactory.MockProgramState(1.2);

            var token = new Elements();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps1.Object)).Message.ShouldBe("Invalid argument type passed to \u03B4 command - negative value: -5");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps2.Object)).Message.ShouldBe("Invalid argument type passed to \u03B4 command - non-integral value: 1.2");
        }

        [Fact]
        public void Elements_should_get_characters_from_string()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(MockFactory.MockStringValue("abc").Object);

            var token = new Elements();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith("a", "b", "c");
        }

        [Fact]
        public void Elements_should_throw_exception_for_array_value()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new Elements();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps.Object)).Message.ShouldBe("Invalid argument type passed to \u03B4 command - Array");
        }
    }
}
