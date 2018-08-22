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
    public class ArgumentsTests
    {
        [Fact]
        public void SingleArgument_should_default_to_zero_if_unindexed()
        {
            // Arrange
            var mockState = new Mock<ProgramState>();
            mockState.SetupGet(s => s.ArgumentList).Returns(new DataValue[0]);
            var token = new SingleArgument(SingleArgument.CHAR_LIST[0]);

            // Act
            var result = token.Evaluate(mockState.Object);

            // Assert
            var num = result.ShouldBeOfType<NumericValue>();
            num.Value.ShouldBe(0);
        }

        [Fact]
        public void SingleArgument_should_retrieve_correct_argument()
        {
            // Arrange
            var value1 = new Mock<DataValue>();
            var value2 = new Mock<DataValue>();
            var value3 = new Mock<DataValue>();

            var arguments = new DataValue[]
            {
                value1.Object,
                value2.Object,
                value3.Object
            };

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupGet(s => s.ArgumentList).Returns(arguments);

            var token1 = new SingleArgument(SingleArgument.CHAR_LIST[0]);
            var token2 = new SingleArgument(SingleArgument.CHAR_LIST[1]);
            var token3 = new SingleArgument(SingleArgument.CHAR_LIST[2]);

            // Act
            var result1 = token1.Evaluate(mockProgramState.Object);
            var result2 = token2.Evaluate(mockProgramState.Object);
            var result3 = token3.Evaluate(mockProgramState.Object);

            // Assert
            result1.ShouldBe(value1.Object);
            result2.ShouldBe(value2.Object);
            result3.ShouldBe(value3.Object);
        }

        [Fact]
        public void ArgumentArray_should_evaluate_to_correct_array_value()
        {
            // Arrange
            var value1 = new Mock<DataValue>();
            var value2 = new Mock<DataValue>();
            var value3 = new Mock<DataValue>();

            var arguments = new DataValue[]
            {
                value1.Object,
                value2.Object,
                value3.Object
            };

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupGet(s => s.ArgumentList).Returns(arguments);

            var token = new ArgumentArray();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareArrayTo(value1.Object, value2.Object, value3.Object);
        }
    }
}
