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
    public class ConstantsTests
    {
        [Fact]
        public void ConstantNewline_should_evaluate_to_newline_string()
        {
            // Arrange
            var mockProgramState = new Mock<ProgramState>();

            var token = new ConstantNewline();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("\n");
        }

        [Fact]
        public void ConstantEmptyArray_should_evaluate_to_empty_array()
        {
            // Arrange
            var mockProgramState = new Mock<ProgramState>();

            var token = new ConstantEmptyArray();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
        }

        [Fact]
        public void ConstantEmptyString_should_evaluate_to_empty_string()
        {
            // Arrange
            var mockProgramState = new Mock<ProgramState>();

            var token = new ConstantEmptyString();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("");
        }
    }
}
