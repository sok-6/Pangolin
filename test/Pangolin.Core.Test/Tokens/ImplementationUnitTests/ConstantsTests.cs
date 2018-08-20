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

namespace Pangolin.Core.Test.Tokens.ImplementationUnitTests
{
    public class ConstantsTests
    {
        [Fact]
        public void ConstantNewline_should_evaluate_to_newline_string()
        {
            // Arrange
            var token = new ConstantNewline();

            // Act
            var result = token.Evaluate(MockFactory.EmptyProgramState);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("\n");
        }

        [Fact]
        public void ConstantEmptyArray_should_evaluate_to_empty_array()
        {
            // Arrange
            var token = new ConstantEmptyArray();

            // Act
            var result = token.Evaluate(MockFactory.EmptyProgramState);

            // Assert
            result.ShouldBeEmptyArray();
        }

        [Fact]
        public void ConstantEmptyString_should_evaluate_to_empty_string()
        {
            // Arrange
            var token = new ConstantEmptyString();

            // Act
            var result = token.Evaluate(MockFactory.EmptyProgramState);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void ConstantLowercaseAlphabet_should_evaluate_to_lowercase_alphabet()
        {
            // Arrange
            var token = new ConstantLowercaseAlphabet();

            // Act
            var result = token.Evaluate(MockFactory.EmptyProgramState);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("abcdefghijklmnopqrstuvwxyz");
        }

        [Fact]
        public void ConstantUppercaseAlphabet_should_evaluate_to_uppercase_alphabet()
        {
            // Arrange
            var token = new ConstantUppercaseAlphabet();

            // Act
            var result = token.Evaluate(MockFactory.EmptyProgramState);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
        }

        [Fact]
        public void ConstantPi_should_evaluate_to_numeric_pi()
        {
            // Arrange
            var token = new ConstantPi();

            // Act
            var result = token.Evaluate(MockFactory.EmptyProgramState);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(Math.PI);
        }

        [Fact]
        public void ConstantPi_should_evaluate_to_numeric_tau()
        {
            // Arrange
            var token = new ConstantTau();

            // Act
            var result = token.Evaluate(MockFactory.EmptyProgramState);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(2 * Math.PI);
        }

        [Fact]
        public void ConstantSpace_should_evaluate_to_string_space()
        {
            // Arrange
            var token = new ConstantSpace();

            // Act
            var result = token.Evaluate(MockFactory.EmptyProgramState);

            // Assert
            result.ShouldBeAssignableTo<StringValue>().Value.ShouldBe(" ");
        }
    }
}
