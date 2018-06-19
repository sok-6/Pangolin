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

namespace Pangolin.Core.Test.Tokens.Implementations
{
    public class LengthTests
    {
        [Fact]
        public void Length_should_return_absolute_value_of_numeric()
        {
            // Arrange
            var mockPositiveInt = new Mock<NumericValue>();
            mockPositiveInt.SetupGet(x => x.Type).Returns(DataValueType.Numeric);
            mockPositiveInt.SetupGet(x => x.Value).Returns(5);

            var mockNegativeInt = new Mock<NumericValue>();
            mockNegativeInt.SetupGet(x => x.Type).Returns(DataValueType.Numeric);
            mockNegativeInt.SetupGet(x => x.Value).Returns(-3);

            var mockPositiveFloat = new Mock<NumericValue>();
            mockPositiveFloat.SetupGet(x => x.Type).Returns(DataValueType.Numeric);
            mockPositiveFloat.SetupGet(x => x.Value).Returns(1.23);

            var mockNegativeFloat = new Mock<NumericValue>();
            mockNegativeFloat.SetupGet(x => x.Type).Returns(DataValueType.Numeric);
            mockNegativeFloat.SetupGet(x => x.Value).Returns(-10.5);

            var mockZero = new Mock<NumericValue>();
            mockZero.SetupGet(x => x.Type).Returns(DataValueType.Numeric);
            mockZero.SetupGet(x => x.Value).Returns(0);

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.Setup(p => p.DequeueAndEvaluate()).Returns(mockPositiveInt.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.Setup(p => p.DequeueAndEvaluate()).Returns(mockNegativeInt.Object);

            var mockProgramState3 = new Mock<ProgramState>();
            mockProgramState3.Setup(p => p.DequeueAndEvaluate()).Returns(mockPositiveFloat.Object);

            var mockProgramState4 = new Mock<ProgramState>();
            mockProgramState4.Setup(p => p.DequeueAndEvaluate()).Returns(mockNegativeFloat.Object);

            var mockProgramState5 = new Mock<ProgramState>();
            mockProgramState5.Setup(p => p.DequeueAndEvaluate()).Returns(mockZero.Object);

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
            var mockPopulatedString = new Mock<StringValue>();
            mockPopulatedString.SetupGet(x => x.Type).Returns(DataValueType.String);
            mockPopulatedString.SetupGet(x => x.Value).Returns("test");

            var mockEmptyString = new Mock<StringValue>();
            mockEmptyString.SetupGet(x => x.Type).Returns(DataValueType.String);
            mockEmptyString.SetupGet(x => x.Value).Returns("");

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.Setup(p => p.DequeueAndEvaluate()).Returns(mockPopulatedString.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.Setup(p => p.DequeueAndEvaluate()).Returns(mockEmptyString.Object);

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
            var mockGenericDataValue = new Mock<DataValue>();

            var mockPopulatedArray = new Mock<ArrayValue>();
            mockPopulatedArray.SetupGet(x => x.Type).Returns(DataValueType.Array);
            mockPopulatedArray.SetupGet(x => x.Value).Returns(new DataValue[]
            {
                mockGenericDataValue.Object,
                mockGenericDataValue.Object,
                mockGenericDataValue.Object
            });

            var mockEmptyArray = new Mock<ArrayValue>();
            mockEmptyArray.SetupGet(x => x.Type).Returns(DataValueType.Array);
            mockEmptyArray.SetupGet(x => x.Value).Returns(new DataValue[] { });

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.Setup(p => p.DequeueAndEvaluate()).Returns(mockPopulatedArray.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.Setup(p => p.DequeueAndEvaluate()).Returns(mockEmptyArray.Object);

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
