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
    public class ConversionsTests
    {
        [Fact]
        public void Arrayify_should_wrap_any_value_in_an_array()
        {
            // Arrange
            var mockDataValue = new Mock<DataValue>();

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.Setup(p => p.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new Arrayify();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var arrayResult = result.ShouldBeOfType<ArrayValue>();
            arrayResult.Value.Count.ShouldBe(1);
            arrayResult.Value[0].ShouldBe(mockDataValue.Object);
        }

        [Fact]
        public void ArrayPair_should_wrap_any_two_values_in_an_array()
        {
            // Arrange
            var mockDataValue1 = new Mock<DataValue>();
            var mockDataValue2 = new Mock<DataValue>();

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockDataValue1.Object)
                .Returns(mockDataValue2.Object);

            var token = new ArrayPair();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var arrayResult = result.ShouldBeOfType<ArrayValue>();
            arrayResult.Value.Count.ShouldBe(2);
            arrayResult.Value[0].ShouldBe(mockDataValue1.Object);
            arrayResult.Value[1].ShouldBe(mockDataValue2.Object);
        }
    }
}
