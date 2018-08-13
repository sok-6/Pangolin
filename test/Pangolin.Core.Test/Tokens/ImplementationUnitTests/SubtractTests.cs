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
    public class SubtractTests
    {
        [Fact]
        public void Subtract_should_calculate_mod_between_numerics()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(7, 18);

            var token = new Subtract();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(11);
        }

        [Fact]
        public void Subtract_should_error_if_non_numerics_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);

            var mockString = new Mock<StringValue>();
            mockString.SetupGet(s => s.Type).Returns(DataValueType.String);

            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(a => a.Type).Returns(DataValueType.Array);

            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.Zero, MockFactory.EmptyString);
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.Zero, MockFactory.MockArrayBuilder.Empty);
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.EmptyString, MockFactory.Zero);
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty, MockFactory.Zero);

            var token = new Subtract();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to - command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to - command - Numeric,Array");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to - command - String,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to - command - Array,Numeric");
        }
    }
}
