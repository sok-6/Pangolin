using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Core.TokenImplementations;

namespace Pangolin.Core.Test.Tokens.ImplementationUnitTests
{
    public class OrderTests
    {
        [Fact]
        public void Ascend_should_increment_numerics()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(0);
            var mockProgramState3 = MockFactory.MockProgramState(-10);
            var mockProgramState4 = MockFactory.MockProgramState(1.23);

            var token = new Ascend();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(11);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-9);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(2.23);
        }

        [Fact]
        public void Ascend_should_sort_characters_in_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("cebda");
            var mockProgramState2 = MockFactory.MockProgramState("aaAAaAAaaA");
            var mockProgramState3 = MockFactory.MockProgramState("abc123!$%");
            var mockProgramState4 = MockFactory.MockProgramState("");

            var token = new Ascend();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abcde");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("AAAAAaaaaa");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("!$%123abc");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Ascend_should_sort_elements_of_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(4, 2, 7).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(4, -1, -7).Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("xyz", "123", "abc").Complete());

            var token = new Ascend();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(2, 4, 7).End();
            result2.ShouldBeArrayWhichStartsWith(-7, -1, 4).End();
            result3.ShouldBeArrayWhichStartsWith("123", "abc", "xyz").End();
        }

        [Fact]
        public void Ascend_should_throw_exception_if_array_types_are_not_consistent()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(4, 2, 7).WithStrings("abc").Complete());

            var token = new Ascend();

            // Act/Assert
            Should.Throw<ArgumentException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Comparison failed between types Numeric and String");
        }

        [Fact]
        public void Descend_should_decrement_numerics()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(0);
            var mockProgramState3 = MockFactory.MockProgramState(-10);
            var mockProgramState4 = MockFactory.MockProgramState(1.23);

            var token = new Descend();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(9);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-1);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-11);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0.23, 0.01);
        }

        [Fact]
        public void Decend_should_sort_characters_in_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("cebda");
            var mockProgramState2 = MockFactory.MockProgramState("aaAAaAAaaA");
            var mockProgramState3 = MockFactory.MockProgramState("abc123!$%");
            var mockProgramState4 = MockFactory.MockProgramState("");

            var token = new Descend();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("edcba");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("aaaaaAAAAA");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("cba321%$!");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Descend_should_sort_elements_of_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(4, 2, 7).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(4, -1, -7).Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("xyz", "123", "abc").Complete());

            var token = new Descend();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(7, 4, 2).End();
            result2.ShouldBeArrayWhichStartsWith(4, -1, -7).End();
            result3.ShouldBeArrayWhichStartsWith("xyz", "abc", "123").End();
        }

        [Fact]
        public void Descend_should_throw_exception_if_array_types_are_not_consistent()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(4, 2, 7).WithStrings("abc").Complete());

            var token = new Descend();

            // Act/Assert
            Should.Throw<ArgumentException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Comparison failed between types Numeric and String");
        }

        [Fact]
        public void Floor_should_round_numeric_towards_negative_infinity()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(1.5);
            var mockProgramState2 = MockFactory.MockProgramState(-1.5);
            var mockProgramState3 = MockFactory.MockProgramState(10);
            var mockProgramState4 = MockFactory.MockProgramState(0);

            var token = new Floor();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-2);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(10);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Floor_should_return_smallest_character_of_string_by_unicode_code_point()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("cba");
            var mockProgramState2 = MockFactory.MockProgramState("Aa");
            var mockProgramState3 = MockFactory.MockProgramState("!%^123cba");
            var mockProgramState4 = MockFactory.MockProgramState("z");

            var token = new Floor();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("A");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("!");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("z");
        }

        [Fact]
        public void Floor_should_throw_exception_on_empty_string()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState("");

            var token = new Floor();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Floor unable to find minimum value of empty string");
        }

        [Fact]
        public void Floor_should_return_first_element_of_array_when_sorted()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(2, 4, 1).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abc", "aBc").Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1).Complete());

            var token = new Floor();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("aBc");
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Floor_should_throw_exception_on_empty_array()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new Floor();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Floor unable to find minimum value of empty array");
        }

        [Fact]
        public void Ceiling_should_round_numeric_towards_positive_infinity()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(1.5);
            var mockProgramState2 = MockFactory.MockProgramState(-1.5);
            var mockProgramState3 = MockFactory.MockProgramState(10);
            var mockProgramState4 = MockFactory.MockProgramState(0);

            var token = new Ceiling();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(2);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-1);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(10);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Ceiling_should_return_largest_character_of_string_by_unicode_code_point()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("cba");
            var mockProgramState2 = MockFactory.MockProgramState("Aa");
            var mockProgramState3 = MockFactory.MockProgramState("!%^123cba");
            var mockProgramState4 = MockFactory.MockProgramState("z");

            var token = new Ceiling();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("c");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("c");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("z");
        }

        [Fact]
        public void Ceiling_should_throw_exception_on_empty_string()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState("");

            var token = new Ceiling();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Ceiling unable to find minimum value of empty string");
        }

        [Fact]
        public void Ceiling_should_return_last_element_of_array_when_sorted()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(2, 4, 1).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abc", "aBc").Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1).Complete());

            var token = new Ceiling();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(4);
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abc");
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Ceiling_should_throw_exception_on_empty_array()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new Ceiling();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Ceiling unable to find minimum value of empty array");
        }
    }
}