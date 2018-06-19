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
    public class AddTests
    {
        [Fact]
        public void Add_should_add_two_integers()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.Setup(m => m.Type).Returns(DataValueType.Numeric);
            mockNumeric1.Setup(m => m.Value).Returns(1);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.Setup(m => m.Type).Returns(DataValueType.Numeric);
            mockNumeric2.Setup(m => m.Value).Returns(2);

            var mockQueue = new Mock<ProgramState>();
            mockQueue.SetupSequence(m => m.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var addToken = new Add();

            // Act
            var result = addToken.Evaluate(mockQueue.Object);

            // Assert
            var numResult = result.ShouldBeOfType<NumericValue>();
            numResult.Value.ShouldBe(3);
        }

        [Fact]
        public void Add_should_add_two_floats()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.Setup(m => m.Type).Returns(DataValueType.Numeric);
            mockNumeric1.Setup(m => m.Value).Returns(1.5);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.Setup(m => m.Type).Returns(DataValueType.Numeric);
            mockNumeric2.Setup(m => m.Value).Returns(2.3);

            var mockQueue = new Mock<ProgramState>();
            mockQueue.SetupSequence(m => m.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var addToken = new Add();

            // Act
            var result = addToken.Evaluate(mockQueue.Object);

            // Assert
            var numResult = result.ShouldBeOfType<NumericValue>();
            numResult.Value.ShouldBe(3.8);
        }

        [Fact]
        public void Add_should_concatenate_two_strings()
        {
            // Arrange
            var mockString1 = new Mock<StringValue>();
            mockString1.Setup(m => m.Type).Returns(DataValueType.String);
            mockString1.Setup(m => m.Value).Returns("ab");
            mockString1.Setup(m => m.ToString()).Returns("ab");

            var mockString2 = new Mock<StringValue>();
            mockString2.Setup(m => m.Type).Returns(DataValueType.String);
            mockString2.Setup(m => m.Value).Returns("cd");
            mockString2.Setup(m => m.ToString()).Returns("cd");

            var mockQueue = new Mock<ProgramState>();
            mockQueue.SetupSequence(m => m.DequeueAndEvaluate())
                .Returns(mockString1.Object)
                .Returns(mockString2.Object);

            var addToken = new Add();

            // Act
            var result = addToken.Evaluate(mockQueue.Object);

            // Assert
            var stringResult = result.ShouldBeOfType<StringValue>();
            stringResult.Value.ShouldBe("abcd");
        }

        [Fact]
        public void Add_should_concatenate_string_and_numeric()
        {
            // Arrange
            var mockString = new Mock<StringValue>();
            mockString.Setup(m => m.Type).Returns(DataValueType.String);
            mockString.Setup(m => m.Value).Returns("ab");
            mockString.Setup(m => m.ToString()).Returns("ab");

            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.Setup(m => m.Type).Returns(DataValueType.Numeric);
            mockNumeric.Setup(m => m.Value).Returns(1);
            mockNumeric.Setup(m => m.ToString()).Returns("1");

            var mockQueue1 = new Mock<ProgramState>();
            mockQueue1.SetupSequence(m => m.DequeueAndEvaluate())
                .Returns(mockString.Object)
                .Returns(mockNumeric.Object);

            var mockQueue2 = new Mock<ProgramState>();
            mockQueue2.SetupSequence(m => m.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockString.Object);

            var addToken = new Add();

            // Act
            var result1 = addToken.Evaluate(mockQueue1.Object);
            var result2 = addToken.Evaluate(mockQueue2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("ab1");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("1ab");
        }

        [Fact]
        public void Add_should_concatenate_two_arrays()
        {
            // Arrange
            var mockValue1 = new Mock<NumericValue>();
            var mockValue2 = new Mock<NumericValue>();
            var mockValue3 = new Mock<NumericValue>();
            var mockValue4 = new Mock<NumericValue>();

            var mockArray1 = new Mock<ArrayValue>();
            mockArray1.Setup(m => m.Value).Returns(new DataValue[] { mockValue1.Object, mockValue2.Object });
            mockArray1.Setup(m => m.Type).Returns(DataValueType.Array);

            var mockArray2 = new Mock<ArrayValue>();
            mockArray2.Setup(m => m.Value).Returns(new DataValue[] { mockValue3.Object, mockValue4.Object });
            mockArray2.Setup(m => m.Type).Returns(DataValueType.Array);

            var mockQueue = new Mock<ProgramState>();
            mockQueue.SetupSequence(m => m.DequeueAndEvaluate())
                .Returns(mockArray1.Object)
                .Returns(mockArray2.Object);

            var token = new Add();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(mockValue1.Object, mockValue2.Object, mockValue3.Object, mockValue4.Object);
        }

        [Fact]
        public void Add_should_concatenate_array_and_string()
        {
            // Arrange
            var mockValue1 = new Mock<NumericValue>();
            var mockValue2 = new Mock<NumericValue>();
            var mockValue3 = new Mock<StringValue>();
            mockValue3.Setup(m => m.Type).Returns(DataValueType.String);

            var mockArray = new Mock<ArrayValue>();
            mockArray.Setup(m => m.Value).Returns(new DataValue[] { mockValue1.Object, mockValue2.Object });
            mockArray.Setup(m => m.Type).Returns(DataValueType.Array);

            var mockQueue1 = new Mock<ProgramState>(); // Queue 1 is array -> string
            mockQueue1.SetupSequence(m => m.DequeueAndEvaluate())
                .Returns(mockArray.Object)
                .Returns(mockValue3.Object);

            var mockQueue2 = new Mock<ProgramState>(); // Queue 2 is string -> array
            mockQueue2.SetupSequence(m => m.DequeueAndEvaluate())
                .Returns(mockValue3.Object)
                .Returns(mockArray.Object);

            var token = new Add();

            // Act
            var result1 = token.Evaluate(mockQueue1.Object);
            var result2 = token.Evaluate(mockQueue2.Object);

            // Assert
            result1.ShouldBeOfType<ArrayValue>().CompareTo(mockValue1.Object, mockValue2.Object, mockValue3.Object);
            result2.ShouldBeOfType<ArrayValue>().CompareTo(mockValue3.Object, mockValue1.Object, mockValue2.Object);
        }

        [Fact]
        public void Add_should_concatenate_array_and_numeric()
        {
            // Arrange
            var mockValue1 = new Mock<NumericValue>();
            var mockValue2 = new Mock<NumericValue>();
            var mockValue3 = new Mock<NumericValue>();
            mockValue3.Setup(m => m.Type).Returns(DataValueType.Numeric);

            var mockArray = new Mock<ArrayValue>();
            mockArray.Setup(m => m.Value).Returns(new DataValue[] { mockValue1.Object, mockValue2.Object });
            mockArray.Setup(m => m.Type).Returns(DataValueType.Array);

            var mockQueue1 = new Mock<ProgramState>(); // Queue 1 is array -> numeric
            mockQueue1.SetupSequence(m => m.DequeueAndEvaluate())
                .Returns(mockArray.Object)
                .Returns(mockValue3.Object);

            var mockQueue2 = new Mock<ProgramState>(); // Queue 2 is numeric -> array
            mockQueue2.SetupSequence(m => m.DequeueAndEvaluate())
                .Returns(mockValue3.Object)
                .Returns(mockArray.Object);

            var token = new Add();

            // Act
            var result1 = token.Evaluate(mockQueue1.Object);
            var result2 = token.Evaluate(mockQueue2.Object);

            // Assert
            result1.ShouldBeOfType<ArrayValue>().CompareTo(mockValue1.Object, mockValue2.Object, mockValue3.Object);
            result2.ShouldBeOfType<ArrayValue>().CompareTo(mockValue3.Object, mockValue1.Object, mockValue2.Object);
        }

        [Fact]
        public void Add_should_evaluate_over_iteratable_array_elements_and_numeric()
        {
            // Arrange
            var mockQueue1 = MockFactory.MockProgramState(
                MockFactory.MockArrayValueWithIteration(
                    MockFactory.MockNumericValue(5).Object,
                    MockFactory.MockStringValue("abc").Object,
                    MockFactory.MockNumericValue(5).Object).Object,
                MockFactory.MockNumericValue(5).Object);

            var mockQueue2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(5).Object,
                MockFactory.MockArrayValueWithIteration(
                    MockFactory.MockNumericValue(5).Object,
                    MockFactory.MockStringValue("abc").Object,
                    MockFactory.MockNumericValue(5).Object).Object);

            var token = new Add();

            // Act
            var result1 = token.Evaluate(mockQueue1.Object);
            var result2 = token.Evaluate(mockQueue2.Object);

            // Assert
            var resultArray1 = result1.ShouldBeOfType<ArrayValue>();
            resultArray1.Value.Count.ShouldBe(3);
            resultArray1.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(10);
            resultArray1.Value[1].ShouldBeOfType<StringValue>().Value.ShouldBe("abc5");
            resultArray1.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(10);

            var resultArray2 = result2.ShouldBeOfType<ArrayValue>();
            resultArray2.Value.Count.ShouldBe(3);
            resultArray2.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(10);
            resultArray2.Value[1].ShouldBeOfType<StringValue>().Value.ShouldBe("5abc");
            resultArray2.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(10);
        }
    }
}
