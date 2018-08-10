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
    public class ReverseTests
    {
        [Fact]
        public void Reverse_should_invert_sign_of_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(-1);
            var mockProgramState3 = MockFactory.MockProgramState(0);

            var token = new TokenImplementations.Reverse();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(-10);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Reverse_should_reverse_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abc");
            var mockProgramState2 = MockFactory.MockProgramState("");

            var token = new TokenImplementations.Reverse();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("cba");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Reverse_should_reverse_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new TokenImplementations.Reverse();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(3, 2, 1);
            result2.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
        }
    }
}
