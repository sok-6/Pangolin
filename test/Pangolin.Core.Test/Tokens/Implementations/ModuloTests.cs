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
    public class ModuloTests
    {
        [Fact]
        public void Modulo_should_calculate_mod_between_numerics()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(7);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(18);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

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
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);

            var mockString = new Mock<StringValue>();
            mockString.SetupGet(s => s.Type).Returns(DataValueType.String);

            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(a => a.Type).Returns(DataValueType.Array);

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockNumeric.Object).Returns(mockString.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockNumeric.Object).Returns(mockArray.Object);

            var mockProgramState3 = new Mock<ProgramState>();
            mockProgramState3.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockString.Object).Returns(mockNumeric.Object);

            var mockProgramState4 = new Mock<ProgramState>();
            mockProgramState4.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockArray.Object).Returns(mockNumeric.Object);

            var token = new Modulo();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to % command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to % command - Numeric,Array");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to % command - String,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to % command - Array,Numeric");
        }
    }
}
