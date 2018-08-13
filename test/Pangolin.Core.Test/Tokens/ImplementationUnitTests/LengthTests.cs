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
    public class LengthTests
    {
        [Fact]
        public void Length_should_return_absolute_value_of_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(5);
            var mockProgramState2 = MockFactory.MockProgramState(-3);
            var mockProgramState3 = MockFactory.MockProgramState(1.23);
            var mockProgramState4 = MockFactory.MockProgramState(-10.5);
            var mockProgramState5 = MockFactory.MockProgramState(0);
            
            var token = new Length();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(5);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(3);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(1.23);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(10.5);
            result5.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Length_should_return_length_of_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("test");
            var mockProgramState2 = MockFactory.MockProgramState("");

            var token = new Length();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(4);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Length_should_return_length_of_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(0, 0, 0).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new Length();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(3);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }
    }
}
