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

        [Fact]
        public void Transform_Transpose_should_convert_numeric_to_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(-4);
            var mockProgramState3 = MockFactory.MockProgramState(1.2);
            var mockProgramState4 = MockFactory.MockProgramState(0);

            var token = new Transform_Transpose();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("10");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("-4");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("1.2");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("0");
        }

        [Fact]
        public void Transform_Transpose_should_parse_string_as_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("10");
            var mockProgramState2 = MockFactory.MockProgramState("-4");
            var mockProgramState3 = MockFactory.MockProgramState("1.2");
            var mockProgramState4 = MockFactory.MockProgramState("1E6");
            var mockProgramState5 = MockFactory.MockProgramState("0");

            var token = new Transform_Transpose();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(10);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-4);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1.2);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1000000);
            result5.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Transform_Transpose_should_throw_exception_for_poorly_formatted_numeric_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abc");
            var mockProgramState2 = MockFactory.MockProgramState("123abc");
            var mockProgramState3 = MockFactory.MockProgramState("1   2");
            var mockProgramState4 = MockFactory.MockProgramState("");

            var token = new Transform_Transpose();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Failed to parse string \"abc\" as numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Failed to parse string \"123abc\" as numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Failed to parse string \"1   2\" as numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Failed to parse string \"\" as numeric");
        }

        [Fact]
        public void Transform_Transpose_should_transpose_array_of_strings()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abcd", "efg", "hijk").Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abcd", "", "hijk").Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abcd").Complete());

            var token = new Transform_Transpose();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith("aeh", "bfi", "cgj").End();
            result2.ShouldBeEmptyArray();
            result3.ShouldBeArrayWhichStartsWith("a", "b", "c", "d").End();
        }

        [Fact]
        public void Transform_Transpose_should_transpose_array_of_arrays()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4).Complete())
                .WithArray(MockFactory.MockArrayBuilder.StartingNumerics(5, 6, 7).Complete())
                .WithArray(MockFactory.MockArrayBuilder.StartingStrings("a", "b", "c", "d").Complete())
                .Complete());

            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4).Complete())
                .WithArray(MockFactory.MockArrayBuilder.Empty)
                .WithArray(MockFactory.MockArrayBuilder.StartingStrings("a", "b", "c", "d").Complete())
                .Complete());

            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4).Complete())
                .Complete());

            var token = new Transform_Transpose();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1
                .ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1, 5).ThenShouldContinueWith("a").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2, 6).ThenShouldContinueWith("b").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3, 7).ThenShouldContinueWith("c").End()).End();

            result2.ShouldBeEmptyArray();

            result3
                .ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(4).End()).End();
        }

        [Fact]
        public void Transform_Transpose_should_throw_exception_if_array_passed_with_numerics()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());

            var token = new Transform_Transpose();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Transform_Transpose can only be evaluated on array if none of the elements are non-numeric - arg=[1,2,3]");
        }
    }
}
