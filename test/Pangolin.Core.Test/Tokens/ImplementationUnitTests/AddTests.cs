//using Moq;
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
    public class AddTests
    {
        [Fact]
        public void Add_should_add_two_integers()
        {
            // Arrange
            var mockQueue = MockFactory.MockProgramState(1, 2);

            var addToken = new Add();

            // Act
            var result = addToken.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(3);
        }

        [Fact]
        public void Add_should_add_two_floats()
        {
            // Arrange
            var mockQueue = MockFactory.MockProgramState(1.5, 2.3);

            var addToken = new Add();

            // Act
            var result = addToken.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(3.8);
        }

        [Fact]
        public void Add_should_concatenate_two_strings()
        {
            // Arrange
            var mockQueue = MockFactory.MockProgramState("ab", "cd");

            var addToken = new Add();

            // Act
            var result = addToken.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("abcd");
        }

        [Fact]
        public void Add_should_concatenate_string_and_numeric()
        {
            // Arrange
            var mockQueue1 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("ab").Object,
                MockFactory.MockNumericValue(1).Object);

            var mockQueue2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1).Object,
                MockFactory.MockStringValue("ab").Object);

            var addToken = new Add();

            // Act
            var result1 = addToken.Evaluate(mockQueue1.Object);
            var result2 = addToken.Evaluate(mockQueue2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("ab1");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("1ab");
        }

        [Fact]
        public void Add_should_concatenate_two_arrays()
        {
            // Arrange
            var mockQueue = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2).Complete(),
                MockFactory.MockArrayBuilder.StartingNumerics(3, 4).Complete());

            var token = new Add();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(1, 2, 3, 4);
        }

        [Fact]
        public void Add_should_concatenate_array_and_string()
        {
            // Arrange
            var mockQueue1 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2).Complete(),
                MockFactory.MockStringValue("abc").Object);

            var mockQueue2 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2).Complete());

            var token = new Add();

            // Act
            var result1 = token.Evaluate(mockQueue1.Object);
            var result2 = token.Evaluate(mockQueue2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(1, 2).ThenShouldContinueWith("abc");
            result2.ShouldBeArrayWhichStartsWith("abc").ThenShouldContinueWith(1, 2);
        }

        [Fact]
        public void Add_should_concatenate_array_and_numeric()
        {
            // Arrange
            var mockQueue1 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2).Complete(),
                MockFactory.MockNumericValue(3).Object);

            var mockQueue2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(3).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2).Complete());

            var token = new Add();

            // Act
            var result1 = token.Evaluate(mockQueue1.Object);
            var result2 = token.Evaluate(mockQueue2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(1, 2, 3);
            result2.ShouldBeArrayWhichStartsWith(3, 1, 2);
        }

        [Fact]
        public void Add_should_evaluate_over_iteratable_array_elements_and_numeric()
        {
            // Arrange
            var mockQueue1 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder
                    .StartingNumerics(5)
                    .WithStrings("abc")
                    .WithNumerics(5).CompleteIterationRequired(),
                MockFactory.MockNumericValue(5).Object);

            var mockQueue2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(5).Object,
                MockFactory.MockArrayBuilder
                    .StartingNumerics(5)
                    .WithStrings("abc")
                    .WithNumerics(5).CompleteIterationRequired());

            var token = new Add();

            // Act
            var result1 = token.Evaluate(mockQueue1.Object);
            var result2 = token.Evaluate(mockQueue2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(10).ThenShouldContinueWith("abc5").ThenShouldContinueWith(10);
            result2.ShouldBeArrayWhichStartsWith(10).ThenShouldContinueWith("5abc").ThenShouldContinueWith(10);
        }

        [Fact]
        public void IteratedAdd_should_throw_argument_exception_if_two_numerics_passed()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1).Object,
                MockFactory.MockNumericValue(2).Object);

            var token = new IteratedAdd();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Invalid argument types passed to \u2295 command - Numeric,Numeric");
        }

        [Fact]
        public void IteratedAdd_should_add_over_an_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(10, 20, 30).WithStrings("abc").Complete(),
                MockFactory.MockNumericValue(5).Object);

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(5).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(10, 20, 30).WithStrings("abc").Complete());

            var token = new IteratedAdd();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(15, 25, 35).ThenShouldContinueWith("abc5");
            result2.ShouldBeArrayWhichStartsWith(15, 25, 35).ThenShouldContinueWith("5abc");
        }

        [Fact]
        public void IteratedAdd_should_zip_add_two_iterable_values()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockStringValue("abcd").Object);

            var token = new IteratedAdd();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith("1a", "2b", "3c");
        }
    }
}
