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
    public class PartOfTests
    {
        [Fact]
        public void AllButFirst_ModTen_should_perform_mod_ten_on_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(123);
            var mockProgramState2 = MockFactory.MockProgramState(-15);
            var mockProgramState3 = MockFactory.MockProgramState(0);
            var mockProgramState4 = MockFactory.MockProgramState(12.3);

            var token = new AllButFirst_ModTen();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(3);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-5);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(2.3, 0.1);
        }

        [Fact]
        public void AllButFirst_ModTen_should_return_tail_for_iterable_longer_than_1()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abcde");
            var mockProgramState2 = MockFactory.MockProgramState("ab");
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2).Complete());

            var token = new AllButFirst_ModTen();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("bcde");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("b");
            result3.ShouldBeArrayWhichStartsWith(2, 3, 4, 5).End();
            result4.ShouldBeArrayWhichStartsWith(2).End();
        }

        [Fact]
        public void AllButFirst_ModTen_should_return_empty_for_iterable_shorter_than_2()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("a");
            var mockProgramState2 = MockFactory.MockProgramState("");
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new AllButFirst_ModTen();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
            result3.ShouldBeEmptyArray();
            result4.ShouldBeEmptyArray();
        }

        [Fact]
        public void AllButLast_LogTen_should_perform_log_ten_on_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(123);
            var mockProgramState2 = MockFactory.MockProgramState(15);

            var token = new AllButLast_LogTen();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(2.0899, 0.0001);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1.1761, 0.0001);
        }

        [Fact]
        public void AllButLast_LogTen_should_return_head_for_iterable_longer_than_1()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abcde");
            var mockProgramState2 = MockFactory.MockProgramState("ab");
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2).Complete());

            var token = new AllButLast_LogTen();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abcd");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a");
            result3.ShouldBeArrayWhichStartsWith(1, 2, 3, 4).End();
            result4.ShouldBeArrayWhichStartsWith(1).End();
        }

        [Fact]
        public void AllButLast_LogTen_should_return_empty_for_iterable_shorter_than_2()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("a");
            var mockProgramState2 = MockFactory.MockProgramState("");
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new AllButLast_LogTen();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
            result3.ShouldBeEmptyArray();
            result4.ShouldBeEmptyArray();
        }

        [Fact]
        public void Index_should_modular_index_into_set_for_integral_and_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(0).Object,
                MockFactory.MockStringValue("abcde").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abcde").Object,
                MockFactory.MockNumericValue(0).Object);
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(2).Object,
                MockFactory.MockStringValue("abcde").Object);
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abcde").Object,
                MockFactory.MockNumericValue(2).Object);
            var mockProgramState5 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(8).Object,
                MockFactory.MockStringValue("abcde").Object);
            var mockProgramState6 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abcde").Object,
                MockFactory.MockNumericValue(8).Object);
            var mockProgramState7 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(-1).Object,
                MockFactory.MockStringValue("abcde").Object);
            var mockProgramState8 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abcde").Object,
                MockFactory.MockNumericValue(-1).Object);
            var mockProgramState9 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(-12).Object,
                MockFactory.MockStringValue("abcde").Object);
            var mockProgramState10 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abcde").Object,
                MockFactory.MockNumericValue(-12).Object);

            var token = new Index();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);
            var result6 = token.Evaluate(mockProgramState6.Object);
            var result7 = token.Evaluate(mockProgramState7.Object);
            var result8 = token.Evaluate(mockProgramState8.Object);
            var result9 = token.Evaluate(mockProgramState9.Object);
            var result10 = token.Evaluate(mockProgramState10.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("c");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("c");
            result5.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("d");
            result6.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("d");
            result7.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("e");
            result8.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("e");
            result9.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("d");
            result10.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("d");
        }

        [Fact]
        public void Index_should_modular_index_into_set_for_integral_and_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(0).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete(),
                MockFactory.MockNumericValue(0).Object);
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(2).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete(),
                MockFactory.MockNumericValue(2).Object);
            var mockProgramState5 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(8).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete());
            var mockProgramState6 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete(),
                MockFactory.MockNumericValue(8).Object);
            var mockProgramState7 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(-1).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete());
            var mockProgramState8 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete(),
                MockFactory.MockNumericValue(-1).Object);
            var mockProgramState9 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(-12).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete());
            var mockProgramState10 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete(),
                MockFactory.MockNumericValue(-12).Object);

            var token = new Index();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);
            var result6 = token.Evaluate(mockProgramState6.Object);
            var result7 = token.Evaluate(mockProgramState7.Object);
            var result8 = token.Evaluate(mockProgramState8.Object);
            var result9 = token.Evaluate(mockProgramState9.Object);
            var result10 = token.Evaluate(mockProgramState10.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(3);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(3);
            result5.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(4);
            result6.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(4);
            result7.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(5);
            result8.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(5);
            result9.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(4);
            result10.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(4);
        }

        [Fact]
        public void Index_should_throw_exception_if_both_numeric()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.Zero, MockFactory.Zero);

            var token = new Index();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Invalid argument types passed to Index command - Numeric,Numeric");
        }

        [Fact]
        public void Index_should_throw_exception_if_neither_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.EmptyString, MockFactory.EmptyString);
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.EmptyString, MockFactory.MockArrayBuilder.Empty);
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty, MockFactory.EmptyString);
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty, MockFactory.MockArrayBuilder.Empty);

            var token = new Index();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to Index command - String,String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to Index command - String,Array");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to Index command - Array,String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to Index command - Array,Array");
        }
    }
}
