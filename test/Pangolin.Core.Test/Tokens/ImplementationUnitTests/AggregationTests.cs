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
    public class AggregationTests
    {
        [Fact]
        public void Sum_should_calculate_nth_triangle_number_for_positive_integral()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(5);

            var token = new Sum();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(15);
        }

        [Fact]
        public void Sum_should_throw_exceptions_for_negative_integrals_and_floats()
        {
            // Arrange
            var ps1 = MockFactory.MockProgramState(-5);
            var ps2 = MockFactory.MockProgramState(5.5);

            var token = new Sum();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps1.Object)).Message.ShouldBe("Invalid argument type passed to \u03A3 command - negative numeric: -5");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps2.Object)).Message.ShouldBe("Invalid argument type passed to \u03A3 command - non-integral numeric: 5.5");
        }

        [Fact]
        public void Sum_should_throw_exception_for_string()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(MockFactory.MockStringValue("abc").Object);

            var token = new Sum();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps.Object)).Message.ShouldBe("Invalid argument type passed to \u03A3 command - String");
        }

        [Fact]
        public void Sum_should_return_sum_of_numeric_array()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(10, -3, 6.7).Complete());

            var token = new Sum();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(13.7);
        }

        [Fact]
        public void Sum_should_return_concatenated_string_of_array_with_strings()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder
                    .StartingStrings("abc", "ijk", "", "xyz").Complete());

            var token = new Sum();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("abcijkxyz");
        }

        [Fact]
        public void Sum_should_return_concatenated_string_of_array_with_strings_and_numerics()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder
                    .StartingNumerics(10, -3, 6.7)
                    .WithStrings("abc").Complete());

            var token = new Sum();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("10-36.7abc");
        }

        [Fact]
        public void Sum_should_return_nested_array_flattened_by_one_layer()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder
                    .StartingArray(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete())
                    .WithArray(MockFactory.MockArrayBuilder.StartingStrings("abc", "xyz").Complete())
                    .WithArray(MockFactory.MockArrayBuilder.StartingArray(
                        MockFactory.MockArrayBuilder.StartingNumerics(7, 8, 9).Complete()).Complete()).Complete()); // Should look something like [[1,2,3],["abc","xyz"],[[7,8,9]]]

            var token = new Sum();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(1, 2, 3)
                .ThenShouldContinueWith("abc", "xyz")
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(7, 8, 9).End())
                .End();
        }
    }
}
