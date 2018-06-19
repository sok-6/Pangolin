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
    public class RandomTests
    {
        [Fact]
        public void GetRandomDecimal_should_return_numeric_less_than_1()
        {
            // Arrange
            var mockProgramState = new Mock<ProgramState>();

            var token = new GetRandomDecimal();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBeGreaterThanOrEqualTo(0);
            numericResult.Value.ShouldBeLessThan(1);
        }
    }
}
