using Moq;
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
    public class TruthinessTests
    {
        [Fact]
        public void Truthify_should_evaluate_truthy_value_as_truthy()
        {
            // Arrange
            var mockQueue = MockFactory.MockProgramState(MockFactory.MockDataValue(true).Object);

            var token = new Truthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Truthify_should_evaluate_falsey_value_as_falsey()
        {
            // Arrange
            var mockQueue = MockFactory.MockProgramState(MockFactory.MockDataValue(false).Object);

            var token = new Truthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Truthify_should_evaluate_over_iterable()
        {
            // Arrange
            var mockTruthyValue = new Mock<DataValue>();
            mockTruthyValue.Setup(m => m.IsTruthy).Returns(true);
            var mockFalseyValue = new Mock<DataValue>();
            mockFalseyValue.Setup(m => m.IsTruthy).Returns(false);

            var mockIteratableArray = new Mock<ArrayValue>();
            mockIteratableArray.SetupGet(x => x.IterationRequired).Returns(true);
            mockIteratableArray.SetupGet(x => x.IterationValues).Returns(new DataValue[] { mockTruthyValue.Object, mockFalseyValue.Object });

            var mockQueue = new Mock<ProgramState>();
            mockQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockIteratableArray.Object);

            var token = new Truthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(1, 0);
        }

        [Fact]
        public void Untruthify_should_evaluate_truthy_value_as_falsey()
        {
            // Arrange
            var mockValue = new Mock<DataValue>();
            mockValue.Setup(m => m.IsTruthy).Returns(true);
            var mockQueue = new Mock<ProgramState>();
            mockQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockValue.Object);
            var token = new UnTruthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(0);
        }

        [Fact]
        public void Untruthify_should_evaluate_falsey_value_as_truthy()
        {
            // Arrange
            var mockValue = new Mock<DataValue>();
            mockValue.Setup(m => m.IsTruthy).Returns(false);
            var mockQueue = new Mock<ProgramState>();
            mockQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockValue.Object);
            var token = new UnTruthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(1);
        }

        [Fact]
        public void Untruthify_should_evaluate_over_iterable()
        {
            // Arrange
            var mockTruthyValue = new Mock<DataValue>();
            mockTruthyValue.Setup(m => m.IsTruthy).Returns(true);
            var mockFalseyValue = new Mock<DataValue>();
            mockFalseyValue.Setup(m => m.IsTruthy).Returns(false);

            var mockIteratableArray = new Mock<ArrayValue>();
            mockIteratableArray.SetupGet(x => x.IterationRequired).Returns(true);
            mockIteratableArray.SetupGet(x => x.IterationValues).Returns(new DataValue[] { mockTruthyValue.Object, mockFalseyValue.Object });

            var mockQueue = new Mock<ProgramState>();
            mockQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockIteratableArray.Object);

            var token = new UnTruthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(0, 1);
        }
    }
}
