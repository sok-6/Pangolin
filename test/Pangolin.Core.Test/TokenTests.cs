using Moq;
using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pangolin.Core.Test
{
    public class TokenTests
    {
        [Fact]
        public void NumericLiteral_should_evaluate_to_NumericValue_with_correct_contents()
        {
            // Arrange
            var token = Token.Get.NumericLiteral(1.5m);

            // Act
            var result = token.Evaluate(null);

            // Assert
            result.ShouldBeOfType(typeof(NumericValue));
            result.ToString().ShouldBe("1.5");
        }

        [Fact]
        public void StringLiteral_should_evaluate_to_StringValue_with_correct_contents()
        {
            // Arrange
            var token = Token.Get.StringLiteral("abc");

            // Act
            var result = token.Evaluate(null);

            // Assert
            result.ShouldBeOfType(typeof(StringValue));
            result.ToString().ShouldBe("abc");
        }

        [Fact]
        public void Truthify_should_evaluate_truthy_value_as_truthy()
        {
            // Arrange
            var mockValue = new Mock<DataValue>();
            mockValue.Setup(m => m.IsTruthy).Returns(true);
            var mockQueue = new Mock<ProgramState>();
            mockQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockValue.Object);
            var token = Token.Get.Truthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(1);
        }

        [Fact]
        public void Truthify_should_evaluate_falsey_value_as_falsey()
        {
            // Arrange
            var mockValue = new Mock<DataValue>();
            mockValue.Setup(m => m.IsTruthy).Returns(false);
            var mockQueue = new Mock<ProgramState>();
            mockQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockValue.Object);
            var token = Token.Get.Truthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(0);
        }

        [Fact]
        public void Untruthify_should_evaluate_truthy_value_as_falsey()
        {
            // Arrange
            var mockValue = new Mock<DataValue>();
            mockValue.Setup(m => m.IsTruthy).Returns(true);
            var mockQueue = new Mock<ProgramState>();
            mockQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockValue.Object);
            var token = Token.Get.UnTruthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(0);
        }

        [Fact]
        public void Untruthify_should_evaluate_falsey_value_as_truthy()
        {
            // Arrange
            var mockValue = new Mock<DataValue>();
            mockValue.Setup(m => m.IsTruthy).Returns(false);
            var mockQueue = new Mock<ProgramState>();
            mockQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockValue.Object);
            var token = Token.Get.UnTruthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(1);
        }

        [Fact]
        public void SingleArgument_should_default_to_zero_if_unindexed()
        {
            // Arrange
            var arguments = new DataValue[]
            {

            };
            var token = Token.Get.SingleArgument(arguments, 1);

            // Act
            var result = token.Evaluate(null);

            // Assert
            var num = result.ShouldBeOfType<NumericValue>();
            num.Value.ShouldBe(0);
        }

        [Fact]
        public void SingleArgument_should_retrieve_correct_argument()
        {
            // Arrange
            var value1 = new Mock<DataValue>();
            var value2 = new Mock<DataValue>();
            var value3 = new Mock<DataValue>();

            var arguments = new DataValue[]
            {
                value1.Object,
                value2.Object,
                value3.Object
            };

            var token1 = Token.Get.SingleArgument(arguments, 0);
            var token2 = Token.Get.SingleArgument(arguments, 1);
            var token3 = Token.Get.SingleArgument(arguments, 2);

            // Act
            var result1 = token1.Evaluate(null);
            var result2 = token2.Evaluate(null);
            var result3 = token3.Evaluate(null);

            // Assert
            result1.ShouldBe(value1.Object);
            result2.ShouldBe(value2.Object);
            result3.ShouldBe(value3.Object);
        }

        [Fact]
        public void ArgumentArray_should_evaluate_to_correct_array_value()
        {
            // Arrange
            var value1 = new Mock<DataValue>();
            var value2 = new Mock<DataValue>();
            var value3 = new Mock<DataValue>();

            var arguments = new DataValue[]
            {
                value1.Object,
                value2.Object,
                value3.Object
            };
            var token = Token.Get.ArgumentArray(arguments);

            // Act
            var result = token.Evaluate(null);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value[0].ShouldBe(value1.Object);
            arrayValue.Value[1].ShouldBe(value2.Object);
            arrayValue.Value[2].ShouldBe(value3.Object);
        }

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

            var addToken = Token.Get.Add();

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
            mockNumeric1.Setup(m => m.Value).Returns(1.5m);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.Setup(m => m.Type).Returns(DataValueType.Numeric);
            mockNumeric2.Setup(m => m.Value).Returns(2.3m);

            var mockQueue = new Mock<ProgramState>();
            mockQueue.SetupSequence(m => m.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var addToken = Token.Get.Add();

            // Act
            var result = addToken.Evaluate(mockQueue.Object);

            // Assert
            var numResult = result.ShouldBeOfType<NumericValue>();
            numResult.Value.ShouldBe(3.8m);
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

            var addToken = Token.Get.Add();

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

            var addToken = Token.Get.Add();

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

            var token = Token.Get.Add();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value[0].ShouldBe(mockValue1.Object);
            arrayValue.Value[1].ShouldBe(mockValue2.Object);
            arrayValue.Value[2].ShouldBe(mockValue3.Object);
            arrayValue.Value[3].ShouldBe(mockValue4.Object);
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

            var token = Token.Get.Add();

            // Act
            var result1 = token.Evaluate(mockQueue1.Object);
            var result2 = token.Evaluate(mockQueue2.Object);

            // Assert
            var arrayValue1 = result1.ShouldBeOfType<ArrayValue>();
            arrayValue1.Value[0].ShouldBe(mockValue1.Object);
            arrayValue1.Value[1].ShouldBe(mockValue2.Object);
            arrayValue1.Value[2].ShouldBe(mockValue3.Object);

            var arrayValue2 = result2.ShouldBeOfType<ArrayValue>();
            arrayValue2.Value[0].ShouldBe(mockValue3.Object);
            arrayValue2.Value[1].ShouldBe(mockValue1.Object);
            arrayValue2.Value[2].ShouldBe(mockValue2.Object);
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

            var token = Token.Get.Add();

            // Act
            var result1 = token.Evaluate(mockQueue1.Object);
            var result2 = token.Evaluate(mockQueue2.Object);

            // Assert
            var arrayValue1 = result1.ShouldBeOfType<ArrayValue>();
            arrayValue1.Value[0].ShouldBe(mockValue1.Object);
            arrayValue1.Value[1].ShouldBe(mockValue2.Object);
            arrayValue1.Value[2].ShouldBe(mockValue3.Object);

            var arrayValue2 = result2.ShouldBeOfType<ArrayValue>();
            arrayValue2.Value[0].ShouldBe(mockValue3.Object);
            arrayValue2.Value[1].ShouldBe(mockValue1.Object);
            arrayValue2.Value[2].ShouldBe(mockValue2.Object);
        }

        [Fact]
        public void Range_should_return_positive_range_when_positive_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.Range();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value.Count.ShouldBe(5);

            arrayValue.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            arrayValue.Value[1].ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            arrayValue.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(2);
            arrayValue.Value[3].ShouldBeOfType<NumericValue>().Value.ShouldBe(3);
            arrayValue.Value[4].ShouldBeOfType<NumericValue>().Value.ShouldBe(4);
        }

        [Fact]
        public void Range_should_return_negative_range_when_negative_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(-5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.Range();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value.Count.ShouldBe(5);

            arrayValue.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(-4);
            arrayValue.Value[1].ShouldBeOfType<NumericValue>().Value.ShouldBe(-3);
            arrayValue.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(-2);
            arrayValue.Value[3].ShouldBeOfType<NumericValue>().Value.ShouldBe(-1);
            arrayValue.Value[4].ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Range_should_return_empty_array_when_0_passed()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(0);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.Range();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value.Count.ShouldBe(0);
        }

        [Fact]
        public void Range_should_throw_InvalidArgumentTypeException_when_non_numeric_passed()
        {
            // Arrange
            var mockStringValue = new Mock<StringValue>();
            mockStringValue.Setup(m => m.Type).Returns(DataValueType.String);
            var mockStringTokenQueue = new Mock<ProgramState>();
            mockStringTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockStringValue.Object);
            var mockArrayValue = new Mock<ArrayValue>();
            mockArrayValue.Setup(m => m.Type).Returns(DataValueType.Array);
            var mockArrayTokenQueue = new Mock<ProgramState>();
            mockArrayTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockArrayValue.Object);

            var token = new TokenImplementations.Range();

            // Act / Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe($"Invalid argument passed to \u2192 command - {DataValueType.String} not supported");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe($"Invalid argument passed to \u2192 command - {DataValueType.Array} not supported");
        }

        [Fact]
        public void ReverseRange_should_return_positive_range_when_positive_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.ReverseRange();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value.Count.ShouldBe(5);

            arrayValue.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(4);
            arrayValue.Value[1].ShouldBeOfType<NumericValue>().Value.ShouldBe(3);
            arrayValue.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(2);
            arrayValue.Value[3].ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            arrayValue.Value[4].ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void ReverseRange_should_return_negative_range_when_negative_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(-5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.ReverseRange();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value.Count.ShouldBe(5);

            arrayValue.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            arrayValue.Value[1].ShouldBeOfType<NumericValue>().Value.ShouldBe(-1);
            arrayValue.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(-2);
            arrayValue.Value[3].ShouldBeOfType<NumericValue>().Value.ShouldBe(-3);
            arrayValue.Value[4].ShouldBeOfType<NumericValue>().Value.ShouldBe(-4);
        }

        [Fact]
        public void ReverseRange_should_return_empty_array_when_0_passed()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(0);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.ReverseRange();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value.Count.ShouldBe(0);
        }

        [Fact]
        public void ReverseRange_should_throw_InvalidArgumentTypeException_when_non_numeric_passed()
        {
            // Arrange
            var mockStringValue = new Mock<StringValue>();
            mockStringValue.Setup(m => m.Type).Returns(DataValueType.String);
            var mockStringTokenQueue = new Mock<ProgramState>();
            mockStringTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockStringValue.Object);
            var mockArrayValue = new Mock<ArrayValue>();
            mockArrayValue.Setup(m => m.Type).Returns(DataValueType.Array);
            var mockArrayTokenQueue = new Mock<ProgramState>();
            mockArrayTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockArrayValue.Object);

            var token = new TokenImplementations.ReverseRange();

            // Act / Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe($"Invalid argument passed to \u2190 command - {DataValueType.String} not supported");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe($"Invalid argument passed to \u2190 command - {DataValueType.Array} not supported");
        }

        [Fact]
        public void Range1_should_return_positive_range_when_positive_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.Range1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value.Count.ShouldBe(5);

            arrayValue.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            arrayValue.Value[1].ShouldBeOfType<NumericValue>().Value.ShouldBe(2);
            arrayValue.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(3);
            arrayValue.Value[3].ShouldBeOfType<NumericValue>().Value.ShouldBe(4);
            arrayValue.Value[4].ShouldBeOfType<NumericValue>().Value.ShouldBe(5);
        }

        [Fact]
        public void Range1_should_return_negative_range_when_negative_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(-5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.Range1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value.Count.ShouldBe(5);

            arrayValue.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(-5);
            arrayValue.Value[1].ShouldBeOfType<NumericValue>().Value.ShouldBe(-4);
            arrayValue.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(-3);
            arrayValue.Value[3].ShouldBeOfType<NumericValue>().Value.ShouldBe(-2);
            arrayValue.Value[4].ShouldBeOfType<NumericValue>().Value.ShouldBe(-1);
        }

        [Fact]
        public void Range1_should_return_empty_array_when_0_passed()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(0);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.Range1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value.Count.ShouldBe(0);
        }

        [Fact]
        public void Range1_should_throw_InvalidArgumentTypeException_when_non_numeric_passed()
        {
            // Arrange
            var mockStringValue = new Mock<StringValue>();
            mockStringValue.Setup(m => m.Type).Returns(DataValueType.String);
            var mockStringTokenQueue = new Mock<ProgramState>();
            mockStringTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockStringValue.Object);
            var mockArrayValue = new Mock<ArrayValue>();
            mockArrayValue.Setup(m => m.Type).Returns(DataValueType.Array);
            var mockArrayTokenQueue = new Mock<ProgramState>();
            mockArrayTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockArrayValue.Object);

            var token = new TokenImplementations.Range1();

            // Act / Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe($"Invalid argument passed to \u0411 command - {DataValueType.String} not supported");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe($"Invalid argument passed to \u0411 command - {DataValueType.Array} not supported");
        }

        [Fact]
        public void ReverseRange1_should_return_positive_range_when_positive_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.ReverseRange1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value.Count.ShouldBe(5);

            arrayValue.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(5);
            arrayValue.Value[1].ShouldBeOfType<NumericValue>().Value.ShouldBe(4);
            arrayValue.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(3);
            arrayValue.Value[3].ShouldBeOfType<NumericValue>().Value.ShouldBe(2);
            arrayValue.Value[4].ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void ReverseRange1_should_return_negative_range_when_negative_numeric_provided()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(-5);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.ReverseRange1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value.Count.ShouldBe(5);

            arrayValue.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(-1);
            arrayValue.Value[1].ShouldBeOfType<NumericValue>().Value.ShouldBe(-2);
            arrayValue.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(-3);
            arrayValue.Value[3].ShouldBeOfType<NumericValue>().Value.ShouldBe(-4);
            arrayValue.Value[4].ShouldBeOfType<NumericValue>().Value.ShouldBe(-5);
        }

        [Fact]
        public void ReverseRange1_should_return_empty_array_when_0_passed()
        {
            // Arrange
            var mockDataValue = new Mock<NumericValue>();
            mockDataValue.Setup(m => m.Value).Returns(0);
            var mockTokenQueue = new Mock<ProgramState>();
            mockTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.ReverseRange1();

            // Act
            var result = token.Evaluate(mockTokenQueue.Object);

            // Assert
            var arrayValue = result.ShouldBeOfType<ArrayValue>();
            arrayValue.Value.Count.ShouldBe(0);
        }

        [Fact]
        public void ReverseRange1_should_throw_InvalidArgumentTypeException_when_non_numeric_passed()
        {
            // Arrange
            var mockStringValue = new Mock<StringValue>();
            mockStringValue.Setup(m => m.Type).Returns(DataValueType.String);
            var mockStringTokenQueue = new Mock<ProgramState>();
            mockStringTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockStringValue.Object);
            var mockArrayValue = new Mock<ArrayValue>();
            mockArrayValue.Setup(m => m.Type).Returns(DataValueType.Array);
            var mockArrayTokenQueue = new Mock<ProgramState>();
            mockArrayTokenQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockArrayValue.Object);

            var token = new TokenImplementations.ReverseRange1();

            // Act / Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe($"Invalid argument passed to \u042A command - {DataValueType.String} not supported");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe($"Invalid argument passed to \u042A command - {DataValueType.Array} not supported");
        }

        [Fact]
        public void GetVariable_should_return_correct_variable()
        {
            // Arrange
            var mockValue0 = new Mock<DataValue>();
            var mockValue1 = new Mock<DataValue>();
            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.Setup(s => s.GetVariable(0)).Returns(mockValue0.Object);
            mockProgramState.Setup(s => s.GetVariable(1)).Returns(mockValue1.Object);

            var token0 = new TokenImplementations.GetVariable(0);
            var token1 = new TokenImplementations.GetVariable(1);

            // Act
            var result0 = token0.Evaluate(mockProgramState.Object);
            var result1 = token1.Evaluate(mockProgramState.Object);

            // Assert
            result0.ShouldBe(mockValue0.Object);
            result1.ShouldBe(mockValue1.Object);
        }

        [Fact]
        public void SetVariable_should_store_correct_variable()
        {
            // Arrange

            // Act

            // Assert
            0.ShouldBe(1);
        }
    }
}
