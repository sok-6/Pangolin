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

namespace Pangolin.Core.Test.Tokens.ImplementationUnitTests
{
    public class RangeTests
    {
        [Fact]
        public void Range_should_return_positive_range_when_positive_numeric_provided()
        {
            // Arrange
            var mockTokenQueue = MockFactory.MockProgramState(5);

            var token = new TokenImplementations.Range();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(0, 1, 2, 3, 4).End();
        }

        [Fact]
        public void Range_should_return_negative_range_when_negative_numeric_provided()
        {
            // Arrange
            var mockTokenQueue = MockFactory.MockProgramState(-5);

            var token = new TokenImplementations.Range();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(-4, -3, -2, -1, 0).End();
        }

        [Fact]
        public void Range_should_return_empty_array_when_0_passed()
        {
            // Arrange
            var mockTokenQueue = MockFactory.MockProgramState(0);

            var token = new TokenImplementations.Range();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeEmptyArray();
        }

        [Fact]
        public void Range_should_return_last_element_of_string()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState("abc");

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
            var mockStringTokenQueue = MockFactory.MockProgramState(MockFactory.EmptyString);
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
            var mockTokenQueue = MockFactory.MockProgramState(5);

            var token = new TokenImplementations.ReverseRange();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(4, 3, 2, 1, 0).End();
        }

        [Fact]
        public void ReverseRange_should_return_negative_range_when_negative_numeric_provided()
        {
            // Arrange
            var mockTokenQueue = MockFactory.MockProgramState(-5);

            var token = new TokenImplementations.ReverseRange();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(0, -1, -2, -3, -4).End();
        }

        [Fact]
        public void ReverseRange_should_return_empty_array_when_0_passed()
        {
            // Arrange
            var mockTokenQueue = MockFactory.MockProgramState(0);

            var token = new TokenImplementations.ReverseRange();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeEmptyArray();
        }

        [Fact]
        public void ReverseRange_should_return_first_element_of_string()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState("abc");

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
            var mockStringTokenQueue = MockFactory.MockProgramState("");
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
            var mockTokenQueue = MockFactory.MockProgramState(5);

            var token = new TokenImplementations.Range1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(1, 2, 3, 4, 5).End();
        }

        [Fact]
        public void Range1_should_return_negative_range_when_negative_numeric_provided()
        {
            // Arrange
            var mockTokenQueue = MockFactory.MockProgramState(-5);

            var token = new TokenImplementations.Range1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(-5, -4, -3, -2, -1).End();
        }

        [Fact]
        public void Range1_should_return_empty_array_when_0_passed()
        {
            // Arrange
            var mockTokenQueue = MockFactory.MockProgramState(0);

            var token = new TokenImplementations.Range1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeEmptyArray();
        }

        [Fact]
        public void Range1_should_throw_InvalidArgumentTypeException_when_non_numeric_passed()
        {
            // Arrange
            var mockStringTokenQueue = MockFactory.MockProgramState("");
            var mockArrayTokenQueue = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new TokenImplementations.Range1();

            // Act / Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u0411 command - String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u0411 command - Array");
        }

        [Fact]
        public void ReverseRange1_should_return_positive_range_when_positive_numeric_provided()
        {
            // Arrange
            var mockTokenQueue = MockFactory.MockProgramState(5);

            var token = new TokenImplementations.ReverseRange1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(5, 4, 3, 2, 1).End();
        }

        [Fact]
        public void ReverseRange1_should_return_negative_range_when_negative_numeric_provided()
        {
            // Arrange
            var mockTokenQueue = MockFactory.MockProgramState(-5);

            var token = new TokenImplementations.ReverseRange1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(-1, -2, -3, -4, -5).End();
        }

        [Fact]
        public void ReverseRange1_should_return_empty_array_when_0_passed()
        {
            // Arrange
            var mockTokenQueue = MockFactory.MockProgramState(0);

            var token = new TokenImplementations.ReverseRange1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            result.ShouldBeEmptyArray();
        }

        [Fact]
        public void ReverseRange1_should_throw_InvalidArgumentTypeException_when_non_numeric_passed()
        {
            // Arrange
            var mockStringTokenQueue = MockFactory.MockProgramState("");
            var mockArrayTokenQueue = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new TokenImplementations.ReverseRange1();

            // Act / Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u042A command - String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u042A command - Array");
        }
    }
}
