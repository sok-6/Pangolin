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
using Moq;

namespace Pangolin.Core.Test.Tokens.ImplementationUnitTests
{
    public class FlowControlTests
    {
        [Fact]
        public void IfThenElse_should_return_second_argument_if_first_argument_truthy()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockNumericValue(2).Object,
                MockFactory.MockNumericValue(3).Object);
            mockProgramState.Setup(p => p.StepOverNextFunction());

            var token = new IfThenElse();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(2);
            mockProgramState.Verify(p => p.StepOverNextFunction(), Times.Once);
        }

        [Fact(Skip = "This is order dependent, how to set this up with the mocking framework?")]
        public void IfThenElse_should_return_third_argument_if_first_argument_falsey()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object,
                MockFactory.MockNumericValue(2).Object,
                MockFactory.MockNumericValue(3).Object);
            mockProgramState.Setup(p => p.StepOverNextFunction());

            var token = new IfThenElse();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(3);
            mockProgramState.Verify(p => p.StepOverNextFunction(), Times.Once);
        }
    }
}
