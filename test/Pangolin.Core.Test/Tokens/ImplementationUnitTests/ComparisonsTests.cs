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
    public class ComparisonsTests
    {
        [Fact]
        public void Equality_should_return_truthy_for_equal_numerics()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1.4).Object,
                MockFactory.MockNumericValue(1.4).Object);

            var token = new Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Equality_should_return_falsey_for_different_numerics()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1.4).Object,
                MockFactory.MockNumericValue(1.5).Object);

            var token = new Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Equality_should_return_truthy_for_equal_strings()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockStringValue("abc").Object);

            var token = new Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Equality_should_return_falsey_for_different_strings()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockStringValue("xyz").Object);

            var token = new Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Equality_should_return_truthy_for_equal_arrays()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1.4).WithStrings("abc").Complete(),
                MockFactory.MockArrayBuilder.StartingNumerics(1.4).WithStrings("abc").Complete());

            var token = new Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Equality_should_return_falsey_for_equal_arrays_in_different_orders()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1.4).WithStrings("abc").Complete(),
                MockFactory.MockArrayBuilder.StartingStrings("abc").WithNumerics(1.4).Complete());

            var token = new Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Equality_should_return_falsey_for_unequal_arrays()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1.4).WithStrings("abc").Complete(),
                MockFactory.MockArrayBuilder.StartingNumerics(1.5).WithStrings("abc").Complete());

            var token = new Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Inequality_should_return_falsey_for_equal_numerics()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1.4).Object,
                MockFactory.MockNumericValue(1.4).Object);

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Inequality_should_return_truthy_for_different_numerics()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1.4).Object,
                MockFactory.MockNumericValue(1.5).Object);

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Inequality_should_return_falsey_for_equal_strings()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockStringValue("abc").Object);

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Inequality_should_return_truthy_for_different_strings()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockStringValue("xyz").Object);

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Inequality_should_return_falsey_for_equal_arrays()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1.4).WithStrings("abc").Complete(),
                MockFactory.MockArrayBuilder.StartingNumerics(1.4).WithStrings("abc").Complete());

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Inequality_should_return_truthy_for_equal_arrays_in_different_orders()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1.4).WithStrings("abc").Complete(),
                MockFactory.MockArrayBuilder.StartingStrings("abc").WithNumerics(1.4).Complete());

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Inequality_should_return_truthy_for_unequal_arrays()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1.4).WithStrings("abc").Complete(),
                MockFactory.MockArrayBuilder.StartingNumerics(1.5).WithStrings("abc").Complete());

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void LessThan_should_evaluate_inequality_between_numerics()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(7).Object,
                MockFactory.MockNumericValue(18).Object);

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(18).Object,
                MockFactory.MockNumericValue(7).Object);

            var token = new LessThan();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void LessThan_should_error_if_non_numerics_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);

            var mockString = new Mock<StringValue>();
            mockString.SetupGet(s => s.Type).Returns(DataValueType.String);

            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(a => a.Type).Returns(DataValueType.Array);

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockNumeric.Object).Returns(mockString.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockNumeric.Object).Returns(mockArray.Object);

            var mockProgramState3 = new Mock<ProgramState>();
            mockProgramState3.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockString.Object).Returns(mockNumeric.Object);

            var mockProgramState4 = new Mock<ProgramState>();
            mockProgramState4.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockArray.Object).Returns(mockNumeric.Object);

            var token = new LessThan();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to < command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to < command - Numeric,Array");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to < command - String,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to < command - Array,Numeric");
        }

        [Fact]
        public void GreaterThan_should_evaluate_inequality_between_numerics()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(7).Object,
                MockFactory.MockNumericValue(18).Object);

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(18).Object,
                MockFactory.MockNumericValue(7).Object);

            var token = new GreaterThan();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void GreaterThan_should_error_if_non_numerics_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);

            var mockString = new Mock<StringValue>();
            mockString.SetupGet(s => s.Type).Returns(DataValueType.String);

            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(a => a.Type).Returns(DataValueType.Array);

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockNumeric.Object).Returns(mockString.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockNumeric.Object).Returns(mockArray.Object);

            var mockProgramState3 = new Mock<ProgramState>();
            mockProgramState3.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockString.Object).Returns(mockNumeric.Object);

            var mockProgramState4 = new Mock<ProgramState>();
            mockProgramState4.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockArray.Object).Returns(mockNumeric.Object);

            var token = new GreaterThan();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to > command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to > command - Numeric,Array");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to > command - String,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to > command - Array,Numeric");
        }

        [Fact]
        public void IteratedEquality_should_compare_each_character_of_two_strings_individually()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockStringValue("xbz").Object);

            var token = new IteratedEquality();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(0, 1, 0);
        }

        [Fact]
        public void IteratedEquality_should_compare_each_character_of_string_to_numeric()
        {
            // Arrange
            var ps1 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc123").Object,
                MockFactory.MockNumericValue(1).Object);
            var ps2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1).Object,
                MockFactory.MockStringValue("abc123").Object);

            var token = new IteratedEquality();

            // Act
            var result1 = token.Evaluate(ps1.Object);
            var result2 = token.Evaluate(ps2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(0, 0, 0, 1, 0, 0);
            result2.ShouldBeArrayWhichStartsWith(0, 0, 0, 1, 0, 0);
        }

        [Fact]
        public void IteratedEquality_should_compare_each_character_of_string_to_each_element_of_array()
        {
            // Arrange
            var ps1 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockArrayBuilder.StartingStrings("c","b","a").Complete());
            var ps2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingStrings("c", "b", "a").Complete(),
                MockFactory.MockStringValue("abc").Object);

            var token = new IteratedEquality();

            // Act
            var result1 = token.Evaluate(ps1.Object);
            var result2 = token.Evaluate(ps2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(0, 1, 0);
            result2.ShouldBeArrayWhichStartsWith(0, 1, 0);
        }

        [Fact]
        public void IteratedEquality_should_compare_each_element_of_array_to_numeric()
        {
            // Arrange
            var ps1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(3).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var ps2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockNumericValue(3).Object);

            var token = new IteratedEquality();

            // Act
            var result1 = token.Evaluate(ps1.Object);
            var result2 = token.Evaluate(ps2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(0, 0, 1);
            result2.ShouldBeArrayWhichStartsWith(0, 0, 1);
        }

        [Fact]
        public void IteratedEquality_should_compare_two_arrays_element_wise()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockArrayBuilder.StartingNumerics(5, 4, 3).Complete());

            var token = new IteratedEquality();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(0, 0, 1);
        }

        [Fact]
        public void IteratedEquality_should_throw_exception_if_two_numerics_passed()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(1, 2);

            var token = new IteratedEquality();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps.Object)).Message.ShouldBe("Invalid argument types passed to \u229C command - Numeric,Numeric");
        }
    }
}
