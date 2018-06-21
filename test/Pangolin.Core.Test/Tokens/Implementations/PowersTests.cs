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
    public class PowersTests
    {
        [Fact]
        public void SquareRoot_should_throw_exception_if_non_numeric_passed()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object);

            var token = new SquareRoot();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps.Object)).Message.ShouldBe("Invalid argument type passed to \u221A command - String");
        }

        [Fact]
        public void SquareRoot_should_throw_exception_if_negative_value_passed()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(-25);

            var token = new SquareRoot();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(ps.Object)).Message.ShouldBe("Complex numbers not implemented yet, SquareRoot failed for negative value -25");
        }

        [Fact]
        public void SquareRoot_should_compute_real_square_roots()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(25);

            var token = new SquareRoot();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(5);
        }
    }
}
