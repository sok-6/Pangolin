using Moq;
using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pangolin.Core.Test.Tokens.Implementations
{
    public class RangeTests
    {
        [Fact]
        public void Range_should_return_positive_range_when_positive_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.Range();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(0, 1, 2, 3, 4);
        }

        [Fact]
        public void Range_should_return_negative_range_when_negative_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(-5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.Range();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(-4, -3, -2, -1, 0);
        }

        [Fact]
        public void Range_should_return_empty_array_when_0_passed()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(0);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.Range();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
        }

        [Fact]
        public void Range_should_return_last_element_of_string()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockStringValue("abc").Object);

            var token = new TokenImplementations.Range();

            // Act 
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("c");
        }

        [Fact]
        public void Range_should_return_last_element_of_array()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1,2,3).Complete());

            var token = new TokenImplementations.Range();

            // Act 
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(3);
        }

        [Fact]
        public void Range_should_throw_Exception_when_empty_non_numeric_passed()
        {
            // Arrange
            var mockStringTokenQueue = MockFactory.MockProgramState(MockFactory.MockStringValue("").Object);
            var mockArrayTokenQueue = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new TokenImplementations.Range();

            // Act / Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe("Can't get last item of empty String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe("Can't get last item of empty Array");
        }

        [Fact]
        public void ReverseRange_should_return_positive_range_when_positive_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.ReverseRange();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(4, 3, 2, 1, 0);
        }

        [Fact]
        public void ReverseRange_should_return_negative_range_when_negative_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(-5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.ReverseRange();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(0, -1, -2, -3, -4);
        }

        [Fact]
        public void ReverseRange_should_return_empty_array_when_0_passed()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(0);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.ReverseRange();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
        }

        [Fact]
        public void ReverseRange_should_return_first_element_of_string()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockStringValue("abc").Object);

            var token = new TokenImplementations.ReverseRange();

            // Act 
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a");
        }

        [Fact]
        public void ReverseRange_should_return_first_element_of_array()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());

            var token = new TokenImplementations.ReverseRange();

            // Act 
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void ReverseRange_should_throw_Exception_when_empty_non_numeric_passed()
        {
            // Arrange
            var mockStringTokenQueue = MockFactory.MockProgramState(MockFactory.MockStringValue("").Object);
            var mockArrayTokenQueue = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new TokenImplementations.ReverseRange();

            // Act / Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe("Can't get last item of empty String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe("Can't get last item of empty Array");
        }

        [Fact]
        public void Range1_should_return_positive_range_when_positive_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.Range1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(1, 2, 3, 4, 5);
        }

        [Fact]
        public void Range1_should_return_negative_range_when_negative_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(-5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.Range1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(-5, -4, -3, -2, -1);
        }

        [Fact]
        public void Range1_should_return_empty_array_when_0_passed()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(0);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.Range1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
        }

        [Fact]
        public void Range1_should_throw_InvalidArgumentTypeException_when_non_numeric_passed()
        {
            // Arrange
            var mockStringValue = new Mock<StringValue>();
            mockStringValue.Setup(m => m.Type).Returns(DataValueType.String);
            var mockStringTokenQueue = new Mock<ProgramState>();
            mockStringTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockStringValue.Object);
            var mockArrayValue = new Mock<ArrayValue>();
            mockArrayValue.Setup(m => m.Type).Returns(DataValueType.Array);
            var mockArrayTokenQueue = new Mock<ProgramState>();
            mockArrayTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockArrayValue.Object);

            var token = new TokenImplementations.Range1();

            // Act / Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u0411 command - String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u0411 command - Array");
        }

        [Fact]
        public void ReverseRange1_should_return_positive_range_when_positive_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.ReverseRange1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(5, 4, 3, 2, 1);
        }

        [Fact]
        public void ReverseRange1_should_return_negative_range_when_negative_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(-5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.ReverseRange1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(-1, -2, -3, -4, -5);
        }

        [Fact]
        public void ReverseRange1_should_return_empty_array_when_0_passed()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(0);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.ReverseRange1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
        }

        [Fact]
        public void ReverseRange1_should_throw_InvalidArgumentTypeException_when_non_numeric_passed()
        {
            // Arrange
            var mockStringValue = new Mock<StringValue>();
            mockStringValue.Setup(m => m.Type).Returns(DataValueType.String);
            var mockStringTokenQueue = new Mock<ProgramState>();
            mockStringTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockStringValue.Object);
            var mockArrayValue = new Mock<ArrayValue>();
            mockArrayValue.Setup(m => m.Type).Returns(DataValueType.Array);
            var mockArrayTokenQueue = new Mock<ProgramState>();
            mockArrayTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockArrayValue.Object);

            var token = new TokenImplementations.ReverseRange1();

            // Act / Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u042A command - String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u042A command - Array");
        }
    }
}
