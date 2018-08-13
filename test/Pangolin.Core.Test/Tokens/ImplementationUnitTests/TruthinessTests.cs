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

namespace Pangolin.Core.Test.Tokens.ImplementationUnitTests
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
            var mockQueue = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 0).CompleteIterationRequired());

            var token = new Truthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(1, 0).End();
        }

        [Fact]
        public void Untruthify_should_evaluate_truthy_value_as_falsey()
        {
            // Arrange
            var mockQueue = MockFactory.MockProgramState(MockFactory.MockDataValue(true).Object);

            var token = new UnTruthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Untruthify_should_evaluate_falsey_value_as_truthy()
        {
            // Arrange
            var mockQueue = MockFactory.MockProgramState(MockFactory.MockDataValue(false).Object);

            var token = new UnTruthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Untruthify_should_evaluate_over_iterable()
        {
            // Arrange
            var mockQueue = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 0).CompleteIterationRequired());

            var token = new UnTruthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(0, 1).End();
        }
    }
}
