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
    public class IterateTests
    {
        [Fact]
        public void Iterate_should_error_if_numeric_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(x => x.Type).Returns(DataValueType.Numeric);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.Setup(p => p.DequeueAndEvaluate()).Returns(mockNumeric.Object);

            var token = new Iterate();

            // Act / Assert
            Should.Throw<PangolinException>(() => { token.Evaluate(mockProgramState.Object); }).Message.ShouldBe("Iterate is not defined for Numeric");
        }

        [Fact]
        public void Iterate_should_set_iteration_required_flag_on_iterable()
        {
            // Arrange
            var mockString = new Mock<StringValue>();
            mockString.SetupGet(x => x.Type).Returns(DataValueType.String);

            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(x => x.Type).Returns(DataValueType.Array);

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.Setup(p => p.DequeueAndEvaluate()).Returns(mockString.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.Setup(p => p.DequeueAndEvaluate()).Returns(mockArray.Object);

            var token = new Iterate();

            // Act 
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBe(mockString.Object);
            mockString.Verify(s => s.SetIterationRequired(true));

            result2.ShouldBe(mockArray.Object);
            mockArray.Verify(a => a.SetIterationRequired(true));
        }
    }
}
