using Moq;
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
    public class VariablesTests
    {
        [Fact]
        public void GetVariable_should_return_correct_variable()
        {
            // Arrange
            var mockValue0 = new Mock<DataValue>();
            var mockValue1 = new Mock<DataValue>();
            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.Setup(s => s.GetVariable(0)).Returns(mockValue0.Object);
            mockProgramState.Setup(s => s.GetVariable(1)).Returns(mockValue1.Object);

            var token0 = new TokenImplementations.GetVariable(GetVariable.CHAR_LIST[0]);
            var token1 = new TokenImplementations.GetVariable(GetVariable.CHAR_LIST[1]);

            // Act
            var result0 = token0.Evaluate(mockProgramState.Object);
            var result1 = token1.Evaluate(mockProgramState.Object);

            // Assert
            result0.ShouldBe(mockValue0.Object);
            result1.ShouldBe(mockValue1.Object);
        }

        [Fact]
        public void SetVariable_should_store_correct_variable()
        {
            // Arrange
            var mockDataValue = new Mock<DataValue>();
            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.Setup(s => s.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.SetVariable(SetVariable.CHAR_LIST[0]);

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBe(mockDataValue.Object);
            mockProgramState.Verify(s => s.SetVariable(0, mockDataValue.Object));
        }
    }
}
