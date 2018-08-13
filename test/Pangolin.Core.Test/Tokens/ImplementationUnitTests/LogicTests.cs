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
    public class LogicTests
    {
        [Fact]
        public void LogicAnd_should_return_truthy_when_both_operands_truthy()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockDataValue(true).Object);

            var token = new TokenImplementations.LogicAnd();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void LogicAnd_should_return_falsey_when_second_operand_falsey()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockDataValue(false).Object);

            var token = new TokenImplementations.LogicAnd();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void LogicAnd_should_return_falsey_when_first_operand_falsey_without_evaluating_second_operand()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockDataValue(false).Object);
            mockProgramState.Setup(p => p.StepOverNextTokenBlock());

            var token = new TokenImplementations.LogicAnd();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            mockProgramState.Verify(p => p.DequeueAndEvaluate(), Times.Once);
            mockProgramState.Verify(p => p.StepOverNextTokenBlock(), Times.Once);
        }

        [Fact]
        public void LogicOr_should_return_falsey_when_both_operands_falsey()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object, 
                MockFactory.MockDataValue(false).Object);

            var token = new TokenImplementations.LogicOr();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void LogicOr_should_return_truthy_when_second_operand_truthy()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object,
                MockFactory.MockDataValue(true).Object);

            var token = new TokenImplementations.LogicOr();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void LogicOr_should_return_truthy_when_first_operand_truthy_without_evaluating_second_operand()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockDataValue(true).Object);
            mockProgramState.Setup(p => p.StepOverNextTokenBlock());

            var token = new TokenImplementations.LogicOr();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            mockProgramState.Verify(p => p.DequeueAndEvaluate(), Times.Once);
            mockProgramState.Verify(p => p.StepOverNextTokenBlock(), Times.Once);
        }

        [Fact]
        public void LogicXor_should_follow_xor_truth_table()
        {
            // Arrange
            // Test 1 - false,false => false
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object,
                MockFactory.MockDataValue(false).Object);

            // Test 2 - false,true => true
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object,
                MockFactory.MockDataValue(true).Object);

            // Test 3 - true,false => true
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockDataValue(false).Object);

            // Test 4 - true,true => false
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockDataValue(true).Object);

            var token = new TokenImplementations.LogicXor();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void LogicXnor_should_follow_xnor_truth_table()
        {
            // Arrange
            // Test 1 - false,false => true
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object,
                MockFactory.MockDataValue(false).Object);

            // Test 2 - false,true => false
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object,
                MockFactory.MockDataValue(true).Object);

            // Test 3 - true,false => false
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockDataValue(false).Object);

            // Test 4 - true,true => true
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockDataValue(true).Object);

            var token = new TokenImplementations.LogicXnor();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }
    }
}
