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
    public class InterpolationTests
    {
        [Fact]
        public void Interpolation_should_insert_correctly_indexed_array_values()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(x => x.Type).Returns(DataValueType.Numeric);
            mockNumeric.Setup(x => x.ToString()).Returns("4.5");

            var mockString = new Mock<StringValue>();
            mockString.SetupGet(x => x.Type).Returns(DataValueType.String);
            mockString.Setup(x => x.ToString()).Returns("abc");

            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(x => x.Type).Returns(DataValueType.Array);
            mockArray.SetupGet(x => x.Value).Returns(new DataValue[] { mockNumeric.Object, mockString.Object });

            var mockInterpolationString = new Mock<StringValue>();
            mockInterpolationString.SetupGet(x => x.Type).Returns(DataValueType.String);
            mockInterpolationString.SetupGet(x => x.Value).Returns("test\u24EAtest\u2460test\u24EAtest");

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockInterpolationString.Object)
                .Returns(mockArray.Object);

            var token = new Interpolation();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("test4.5testabctest4.5test");
        }

        [Fact]
        public void Interpolation_should_insert_use_single_value_repeatedly_if_not_array()
        {
            var mockString = new Mock<StringValue>();
            mockString.SetupGet(x => x.Type).Returns(DataValueType.String);
            mockString.Setup(x => x.ToString()).Returns("abc");

            var mockInterpolationString = new Mock<StringValue>();
            mockInterpolationString.SetupGet(x => x.Type).Returns(DataValueType.String);
            mockInterpolationString.SetupGet(x => x.Value).Returns("test\u24EAtest\u2460test\u24EAtest");

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockInterpolationString.Object)
                .Returns(mockString.Object);

            var token = new Interpolation();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("testabctestabctestabctest");
        }

        [Fact]
        public void Interpolation_should_throw_exception_when_1st_argument_not_string()
        {
            // Arrange
            var mockDataValue = new Mock<DataValue>();

            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(x => x.Type).Returns(DataValueType.Numeric);

            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(x => x.Type).Returns(DataValueType.Array);

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockDataValue.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockArray.Object)
                .Returns(mockDataValue.Object);

            var token = new Interpolation();

            // Act / Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to $ command - Numeric,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to $ command - Array,Numeric");
        }

    }
}
