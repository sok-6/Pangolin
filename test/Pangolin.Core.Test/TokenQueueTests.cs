using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Moq;

namespace Pangolin.Core.Test
{
    public class TokenQueueTests
    {
        [Fact]
        public void TokenQueue_DequeueAndEvaluate_should_return_evaluated_token()
        {
            // Arrange
            var mockDataValue = new Mock<DataValue>();
            var mockToken = new Mock<Token>();
            mockToken.Setup(t => t.Evaluate(It.IsAny<TokenQueue>())).Returns(mockDataValue.Object);

            var tokenQueue = new TokenQueue(null, mockToken.Object);

            // Act
            var result = tokenQueue.DequeueAndEvaluate();

            // Assert
            result.ShouldBe(mockDataValue.Object);
        }
    }
}
