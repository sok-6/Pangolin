using Moq;
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
    public class MultiplyTests
    {
        [Fact]
        public void Multiply_should_multiply_two_numerics()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(-4, 2.5);

            var token = new TokenImplementations.Multiply();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(-10);
        }

        [Fact]
        public void Multiply_should_repeat_string_if_numeric_and_string_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(3).Object,
                MockFactory.MockStringValue("abc").Object);

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockNumericValue(3).Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("abcabcabc");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("abcabcabc");
        }

        [Fact]
        public void Multiply_should_return_empty_string_if_0_and_string_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(0).Object,
                MockFactory.MockStringValue("abc").Object);

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockNumericValue(0).Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Multiply_should_return_empty_string_if_numeric_and_empty_string_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(3).Object,
                MockFactory.MockStringValue("").Object);

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("").Object,
                MockFactory.MockNumericValue(3).Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Multiply_should_repeat_string_partial_times_if_non_integral_numeric_and_string_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(2.2).Object,
                MockFactory.MockStringValue("abc").Object);

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockNumericValue(2.2).Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("abcabca");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("abcabca");
        }

        [Fact]
        public void Multiply_should_repeat_string_and_reverse_if_negative_numeric_and_string_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(-3).Object,
                MockFactory.MockStringValue("abc").Object);

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockNumericValue(-3).Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("cbacbacba");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("cbacbacba");
        }

        [Fact]
        public void Multiply_should_repeat_string_partial_times_and_reverse_if_negative_non_integral_numeric_and_string_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(-1.2).Object,
                MockFactory.MockStringValue("abc").Object);

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockNumericValue(-1.2).Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("acba");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("acba");
        }

        [Fact]
        public void Multiply_should_repeat_array_if_numeric_and_array_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(2).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockNumericValue(2).Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(1, 2, 3, 1, 2, 3).End();
            result2.ShouldBeArrayWhichStartsWith(1, 2, 3, 1, 2, 3).End();
        }

        [Fact]
        public void Multiply_should_return_empty_array_if_0_and_array_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(0).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockNumericValue(0).Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeEmptyArray();
            result2.ShouldBeEmptyArray();
        }

        [Fact]
        public void Multiply_should_return_empty_array_if_numeric_and_empty_array_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(3).Object,
                MockFactory.MockArrayBuilder.Empty);

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.Empty,
                MockFactory.MockNumericValue(3).Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeEmptyArray();
            result2.ShouldBeEmptyArray();
        }

        [Fact]
        public void Multiply_should_repeat_array_partial_times_if_non_integral_numeric_and_array_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(2.2).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockNumericValue(2.2).Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(1, 2, 3, 1, 2, 3, 1).End();
            result2.ShouldBeArrayWhichStartsWith(1, 2, 3, 1, 2, 3, 1).End();
        }

        [Fact]
        public void Multiply_should_repeat_array_and_reverse_if_negative_numeric_and_array_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(-3).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockNumericValue(-3).Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(3, 2, 1, 3, 2, 1, 3, 2, 1).End();
            result2.ShouldBeArrayWhichStartsWith(3, 2, 1, 3, 2, 1, 3, 2, 1).End();
        }

        [Fact]
        public void Multiply_should_repeat_array_partial_times_and_reverse_if_negative_non_integral_numeric_and_array_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(-1.2).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());

            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockNumericValue(-1.2).Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(1, 3, 2, 1).End();
            result2.ShouldBeArrayWhichStartsWith(1, 3, 2, 1).End();
        }

        [Fact]
        public void Multiply_should_return_cartesian_product_of_two_strings()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState("abc", "xyz");

            var token = new TokenImplementations.Multiply();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith("a", "x").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith("a", "y").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith("a", "z").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith("b", "x").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith("b", "y").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith("b", "z").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith("c", "x").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith("c", "y").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith("c", "z").End())
                .End();
        }

        [Fact]
        public void Multiply_should_return_empty_array_for_empty_string_and_string()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState("abc", "");

            var token = new TokenImplementations.Multiply();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeEmptyArray();
        }

        [Fact]
        public void Multiply_should_return_cartesian_product_of_two_arrays()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockArrayBuilder.StartingNumerics(4, 5, 6).Complete());

            var token = new TokenImplementations.Multiply();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1, 4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1, 5).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1, 6).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2, 4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2, 5).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2, 6).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3, 4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3, 5).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3, 6).End())
                .End();
        }

        [Fact]
        public void Multiply_should_return_empty_array_for_empty_array_and_array()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockArrayBuilder.Empty);
            
            var token = new TokenImplementations.Multiply();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeEmptyArray();
        }

        [Fact]
        public void Multiply_should_return_cartesian_product_of_string_and_array()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockStringValue("abc").Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1).ThenShouldContinueWith("a").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1).ThenShouldContinueWith("b").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1).ThenShouldContinueWith("c").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2).ThenShouldContinueWith("a").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2).ThenShouldContinueWith("b").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2).ThenShouldContinueWith("c").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3).ThenShouldContinueWith("a").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3).ThenShouldContinueWith("b").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3).ThenShouldContinueWith("c").End())
                .End();
        }

        [Fact]
        public void Double_should_double_value_of_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(5);
            var mockProgramState2 = MockFactory.MockProgramState(-1.2);
            var mockProgramState3 = MockFactory.MockProgramState(0);

            var token = new TokenImplementations.Double();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(10);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(-2.4);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Double_should_duplicate_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abc");
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.EmptyString);

            var token = new TokenImplementations.Double();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("abcabc");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Double_should_duplicate_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1,2,3).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new TokenImplementations.Double();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(1, 2, 3, 1, 2, 3);
            result2.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
        }
    }
}
