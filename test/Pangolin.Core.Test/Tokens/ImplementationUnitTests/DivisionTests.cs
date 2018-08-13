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
    public class DivisionTests
    {
        [Fact]
        public void Division_should_calculate_float_division_between_numerics()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(2, 15);

            var token = new Division();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(7.5);
        }

        [Fact]
        public void Division_should_throw_exception_if_1st_argument_0()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(0, 15);

            var token = new Division();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Division by zero attempted");
        }

        [Fact]
        public void Division_should_split_string_into_correct_number_of_pieces()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(3).Object,
                MockFactory.MockStringValue("abcdefghijkl").Object);
            var mockProgramState2 = MockFactory.MockProgramState( 
                MockFactory.MockNumericValue(5).Object,
                MockFactory.MockStringValue("abcdefghijkl").Object);

            var token = new Division();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith("abcd", "efgh", "ijkl");
            result2.ShouldBeArrayWhichStartsWith("abc", "def", "gh", "ij", "kl");
        }

        [Fact]
        public void Division_should_split_array_into_correct_number_of_pieces()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(3).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(5).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12).Complete());

            var token = new Division();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1, 2, 3, 4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(5, 6, 7, 8).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(9, 10, 11, 12).End())
                .End();

            result2.ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1, 2, 3).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(4, 5, 6).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(7, 8).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(9, 10).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(11, 12).End())
                .End();
        }

        [Fact]
        public void Division_should_error_if_first_value_non_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.EmptyString, MockFactory.Zero);
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.EmptyString, MockFactory.EmptyString);
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.EmptyString, MockFactory.MockArrayBuilder.Empty);
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty, MockFactory.Zero);
            var mockProgramState5 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty, MockFactory.EmptyString);
            var mockProgramState6 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty, MockFactory.MockArrayBuilder.Empty);

            var token = new Division();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to / command - String,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to / command - String,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to / command - String,Array");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to / command - Array,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState5.Object)).Message.ShouldBe("Invalid argument types passed to / command - Array,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState6.Object)).Message.ShouldBe("Invalid argument types passed to / command - Array,Array");
        }

        [Fact]
        public void Half_should_calculate_correctly_with_numerics()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(0);
            var mockProgramState3 = MockFactory.MockProgramState(19);
            var mockProgramState4 = MockFactory.MockProgramState(-6);

            var token = new Half();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(5);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(9.5);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(-3);
        }

        [Fact]
        public void Half_should_split_string_in_two()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockStringValue("abcdef").Object);
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockStringValue("abcdefg").Object);

            var token = new Half();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith("abc", "def");
            result2.ShouldBeArrayWhichStartsWith("abcd", "efg");
        }

        [Fact]
        public void Half_should_split_array_in_two()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5, 6).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5, 6, 7).Complete());

            var token = new Half();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1, 2, 3).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(4, 5, 6).End())
                .End();

            result2.ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1, 2, 3, 4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(5, 6, 7).End())
                .End();
        }

        [Fact]
        public void IntegerDivision_should_calculate_truncated_division_between_two_numerics()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(4, 9);
            var mockProgramState2 = MockFactory.MockProgramState(1.2, 7.5);
            var mockProgramState3 = MockFactory.MockProgramState(3, -20);

            var token = new IntegerDivision();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(2);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(6);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(-6);
        }

        [Fact]
        public void IntegerDivision_should_split_iterable_into_x_sized_pieces()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(3).Object,
                MockFactory.MockStringValue("abcdefghij").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(3).Object,
                MockFactory.MockStringValue("").Object);
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(3).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1,2,3,4,5,6,7,8,9,10).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(3).Object,
                MockFactory.MockArrayBuilder.Empty);

            var token = new IntegerDivision();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith("abc", "def", "ghi", "j");
            result2.ShouldBeEmptyArray();
            result3.ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1, 2, 3).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(4, 5, 6).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(7, 8, 9).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(10).End())
                .End();
            result4.ShouldBeEmptyArray();
        }

        [Fact]
        public void IntegerDivision_should_split_iterable_into_x_sized_pieces_reversed_if_negative()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(-3).Object,
                MockFactory.MockStringValue("abcdefghij").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(-3).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5, 6, 7, 8, 9, 10).Complete());

            var token = new IntegerDivision();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith("cba", "fed", "ihg", "j");
            result2.ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(3, 2, 1).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(6, 5, 4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(9, 8, 7).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(10).End())
                .End();
        }

        [Fact]
        public void IntegerDivision_should_error_if_splitting_iterable_and_zero_or_non_integral_size_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(0).Object,
                MockFactory.MockStringValue("abcdefghij").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(2.5).Object,
                MockFactory.MockStringValue("abcdefghij").Object);
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(0).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5, 6, 7, 8, 9, 10).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(2.5).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5, 6, 7, 8, 9, 10).Complete());

            var token = new IntegerDivision();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("For \u00F7, if 1st argument is numeric, it must be non-zero - arg1=0, arg2=abcdefghij");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("For \u00F7, if 1st argument is numeric, it must be integral - arg1=2.5, arg2=abcdefghij");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("For \u00F7, if 1st argument is numeric, it must be non-zero - arg1=0, arg2=[1,2,3,4,5,6,7,8,9,10]");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("For \u00F7, if 1st argument is numeric, it must be integral - arg1=2.5, arg2=[1,2,3,4,5,6,7,8,9,10]");
        }

        [Fact]
        public void IntegerDivision_should_split_iterable_into_pieces_with_sizes_described_by_int_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockStringValue("abcdefghij").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 0, -3).Complete(),
                MockFactory.MockStringValue("abcdefghij").Object);
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockArrayBuilder.StartingNumerics(1,2,3,4,5,6,7,8,9,10).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 0, -3).Complete(),
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5, 6, 7, 8, 9, 10).Complete());

            var token = new IntegerDivision();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith("a", "bc", "def", "g", "hi", "j");
            result2.ShouldBeArrayWhichStartsWith("a", "", "dcb", "e", "", "hgf", "i", "", "j");
            result3.ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2, 3).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(4, 5, 6).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(7).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(8, 9).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(10).End())
                .End();
            result4.ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1).End())
                .ThenShouldContinueWith(v => v.ShouldBeEmptyArray())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(4, 3, 2).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(5).End())
                .ThenShouldContinueWith(v => v.ShouldBeEmptyArray())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(8, 7, 6).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(9).End())
                .ThenShouldContinueWith(v => v.ShouldBeEmptyArray())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(10).End())
                .End();
        }

        [Fact]
        public void IntegerDivision_should_error_if_splitting_iterable_and_empty_array_passed_as_piece_sizes()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.Empty,
                MockFactory.MockStringValue("abcdefghij").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.Empty,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5, 6, 7, 8, 9, 10).Complete());

            var token = new IntegerDivision();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("For \u00F7, if 1st argument is array, it must be populated - arg1=[], arg2=abcdefghij");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("For \u00F7, if 1st argument is array, it must be populated - arg1=[], arg2=[1,2,3,4,5,6,7,8,9,10]");
        }

        [Fact]
        public void IntegerDivision_should_error_if_splitting_iterable_and_array_of_zeros_or_non_integrals_passed_as_piece_sizes()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(0,0,0).Complete(),
                MockFactory.MockStringValue("abcdefghij").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1.5,10).Complete(),
                MockFactory.MockStringValue("abcdefghij").Object);

            var token = new IntegerDivision();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("For \u00F7, if 1st argument is array, at least one element must be non-zero - arg1=[0,0,0], arg2=abcdefghij");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("For \u00F7, if 1st argument is array, all elements of that array must be integral - arg1=[1.5,10], arg2=abcdefghij");
        }

        [Fact]
        public void IntegerDivision_should_error_if_splitting_iterable_and_array_of_non_numerics_passed_as_piece_sizes()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(3, 4).WithStrings("abc").Complete(),
                MockFactory.MockStringValue("abcdefghij").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(3, 4).WithArray(MockFactory.MockArrayBuilder.StartingNumerics(1,2,3).Complete()).Complete(),
                MockFactory.MockStringValue("abcdefghij").Object);

            var token = new IntegerDivision();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("For \u00F7, if 1st argument is array, all elements of that array must be integral - arg1=[3,4,abc], arg2=abcdefghij");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("For \u00F7, if 1st argument is array, all elements of that array must be integral - arg1=[3,4,[1,2,3]], arg2=abcdefghij");
        }
    }
}
