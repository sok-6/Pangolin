﻿using System;
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

            var programState = new ProgramState(new DataValue[0], mockToken.Object);

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
    }
}
