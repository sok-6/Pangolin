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
    public class ProgramStateTests
    {
        [Fact]
        public void ProgramState_DequeueAndEvaluate_should_return_evaluated_token()
        {
            // Arrange
            var mockDataValue = new Mock<DataValue>();
            var mockToken = new Mock<Token>();
            mockToken.Setup(t => t.Evaluate(It.IsAny<ProgramState>())).Returns(mockDataValue.Object);

            var tokenList = new List<Token>();
            tokenList.Add(mockToken.Object);

            var programState = new ProgramState(new DataValue[0], tokenList);

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBe(mockDataValue.Object);
        }

        [Fact]
        public void ProgramState_variables_should_initialise_to_numeric_0()
        {
            // Arrange
            var programState = new ProgramState();

            // Act / Assert
            programState.GetVariable(0).ShouldBeOfType<DataValueImplementations.NumericValue>().Value.ShouldBe(0);
            programState.GetVariable(1).ShouldBeOfType<DataValueImplementations.NumericValue>().Value.ShouldBe(0);
            programState.GetVariable(2).ShouldBeOfType<DataValueImplementations.NumericValue>().Value.ShouldBe(0);
            programState.GetVariable(3).ShouldBeOfType<DataValueImplementations.NumericValue>().Value.ShouldBe(0);
            programState.GetVariable(4).ShouldBeOfType<DataValueImplementations.NumericValue>().Value.ShouldBe(0);
            programState.GetVariable(5).ShouldBeOfType<DataValueImplementations.NumericValue>().Value.ShouldBe(0);
            programState.GetVariable(6).ShouldBeOfType<DataValueImplementations.NumericValue>().Value.ShouldBe(0);
            programState.GetVariable(7).ShouldBeOfType<DataValueImplementations.NumericValue>().Value.ShouldBe(0);
            programState.GetVariable(8).ShouldBeOfType<DataValueImplementations.NumericValue>().Value.ShouldBe(0);
            programState.GetVariable(9).ShouldBeOfType<DataValueImplementations.NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void ProgramState_should_correctly_store_and_get_variable_values()
        {
            // Arrange
            var mockValues = Enumerable
                .Repeat(0, 10)
                .Select(i => new Mock<DataValue>())
                .ToArray();

            var programState = new ProgramState();

            // Act
            var resultSet = new DataValue[10];
            for (int i = 0; i < 10; i++)
            {
                programState.SetVariable(i, mockValues[i].Object);

                resultSet[i] = programState.GetVariable(i);
            }

            // Assert
            for (int i = 0; i < 10; i++)
            {
                resultSet[i].ShouldBe(mockValues[i].Object);
            }
        }

        [Fact]
        public void ProgramState_should_find_end_of_block()
        {
            // Arrange
            var mockTokenArity0 = new Mock<Token>();
            mockTokenArity0.SetupGet(m => m.Arity).Returns(0);
            var mockTokenArity1 = new Mock<Token>();
            mockTokenArity1.SetupGet(m => m.Arity).Returns(1);
            var mockTokenArity2 = new Mock<Token>();
            mockTokenArity2.SetupGet(m => m.Arity).Returns(2);

            var tokenList = new List<Token>();

            tokenList.Add(mockTokenArity1.Object); // 0
            tokenList.Add(mockTokenArity2.Object); // 1
            tokenList.Add(mockTokenArity2.Object); // 2
            tokenList.Add(mockTokenArity0.Object); // 3
            tokenList.Add(mockTokenArity1.Object); // 4
            tokenList.Add(mockTokenArity0.Object); // 5
            tokenList.Add(mockTokenArity1.Object); // 6
            tokenList.Add(mockTokenArity0.Object); // 7
            tokenList.Add(mockTokenArity0.Object); // 8
            tokenList.Add(mockTokenArity1.Object); // 9

            var programState = new ProgramState(null, tokenList);

            // Act
            var result1 = programState.FindEndOfFunction(0);
            var result2 = programState.FindEndOfFunction(2);
            var result3 = programState.FindEndOfFunction(8);
            var result4 = programState.FindEndOfFunction(9);

            // Assert
            result1.ShouldBe(7);
            result2.ShouldBe(5);
            result3.ShouldBe(8);
            result4.ShouldBe(10);
        }
    }
}
