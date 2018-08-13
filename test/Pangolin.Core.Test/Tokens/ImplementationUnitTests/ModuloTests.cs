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
    public class ModuloTests
    {
        [Fact]
        public void Modulo_should_calculate_mod_between_numerics()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(7, 18);

            var token = new Modulo();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(4);
        }

        [Fact]
        public void Modulo_should_error_if_non_numerics_passed()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.Zero, MockFactory.EmptyString);
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.Zero, MockFactory.MockArrayBuilder.Empty);
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.EmptyString, MockFactory.Zero);
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty, MockFactory.Zero);

            var token = new Modulo();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to % command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to % command - Numeric,Array");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to % command - String,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to % command - Array,Numeric");
        }
    }
}
