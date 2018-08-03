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
    public class DivisionTests
    {
        [Fact]
        public void Division_should_calculate_float_division_between_numerics()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(2);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(15);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

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
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(0);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(15);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

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
            var array1 = result1.ShouldBeOfType<ArrayValue>();
            array1.Value.Count.ShouldBe(3);
            array1.Value[0].ShouldBeArrayWhichStartsWith(1, 2, 3, 4);
            array1.Value[1].ShouldBeArrayWhichStartsWith(5, 6, 7, 8);
            array1.Value[2].ShouldBeArrayWhichStartsWith(9, 10, 11, 12);

            var array2 = result2.ShouldBeOfType<ArrayValue>();
            array2.Value.Count.ShouldBe(5);
            array2.Value[0].ShouldBeArrayWhichStartsWith(1, 2, 3);
            array2.Value[1].ShouldBeArrayWhichStartsWith(4, 5, 6);
            array2.Value[2].ShouldBeArrayWhichStartsWith(7, 8);
            array2.Value[3].ShouldBeArrayWhichStartsWith(9, 10);
            array2.Value[4].ShouldBeArrayWhichStartsWith(11, 12);
        }

        [Fact]
        public void Division_should_error_if_first_value_non_numeric()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);

            var mockString = new Mock<StringValue>();
            mockString.SetupGet(s => s.Type).Returns(DataValueType.String);

            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(a => a.Type).Returns(DataValueType.Array);
            
            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockString.Object).Returns(mockNumeric.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockString.Object).Returns(mockString.Object);

            var mockProgramState3 = new Mock<ProgramState>();
            mockProgramState3.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockString.Object).Returns(mockArray.Object);

            var mockProgramState4 = new Mock<ProgramState>();
            mockProgramState4.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockArray.Object).Returns(mockNumeric.Object);

            var mockProgramState5 = new Mock<ProgramState>();
            mockProgramState5.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockArray.Object).Returns(mockString.Object);

            var mockProgramState6 = new Mock<ProgramState>();
            mockProgramState6.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockArray.Object).Returns(mockArray.Object);

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
            var array1 = result1.ShouldBeOfType<ArrayValue>();
            array1.Value.Count.ShouldBe(2);
            array1.Value[0].ShouldBeArrayWhichStartsWith(1, 2, 3);
            array1.Value[1].ShouldBeArrayWhichStartsWith(4, 5, 6);

            var array2 = result2.ShouldBeOfType<ArrayValue>();
            array2.Value.Count.ShouldBe(2);
            array2.Value[0].ShouldBeArrayWhichStartsWith(1, 2, 3, 4);
            array2.Value[1].ShouldBeArrayWhichStartsWith(5, 6, 7);
        }
    }
}
