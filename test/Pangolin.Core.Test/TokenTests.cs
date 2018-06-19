﻿using Moq;
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

namespace Pangolin.Core.Test
{
    public class TokenTests
    {
        private static class MockFactory
        {
            private static Lazy<Mock<NumericValue>> _mockTruthy = new Lazy<Mock<NumericValue>>(() =>
            {
                var mockTruthy = new Mock<NumericValue>();
                mockTruthy.SetupGet(x => x.Type).Returns(DataValueType.Numeric);
                mockTruthy.SetupGet(x => x.Value).Returns(1);
                mockTruthy.SetupGet(x => x.IterationRequired).Returns(false);
                mockTruthy.SetupGet(x => x.IsTruthy).Returns(true);
                mockTruthy.Setup(x => x.ToString()).Returns("1");
                return mockTruthy;
            });

            private static Lazy<Mock<NumericValue>> _mockFalsey = new Lazy<Mock<NumericValue>>(() =>
            {
                var mockFalsey = new Mock<NumericValue>();
                mockFalsey.SetupGet(x => x.Type).Returns(DataValueType.Numeric);
                mockFalsey.SetupGet(x => x.Value).Returns(0);
                mockFalsey.SetupGet(x => x.IterationRequired).Returns(false);
                mockFalsey.SetupGet(x => x.IsTruthy).Returns(false);
                mockFalsey.Setup(x => x.ToString()).Returns("0");
                return mockFalsey;
            });

            public static Mock<DataValue> MockDataValue(bool isTruthy = false)
            {
                var mockDataValue = new Mock<DataValue>();
                mockDataValue.SetupGet(x => x.IsTruthy).Returns(isTruthy);
                return mockDataValue;
            }

            public static Mock<NumericValue> MockNumericValue(decimal value)
            {
                var mockNumericValue = new Mock<NumericValue>();
                mockNumericValue.SetupGet(x => x.Type).Returns(DataValueType.Numeric);
                mockNumericValue.SetupGet(x => x.Value).Returns(value);
                mockNumericValue.SetupGet(x => x.IterationRequired).Returns(false);
                mockNumericValue.SetupGet(x => x.IsTruthy).Returns(value != 0);
                mockNumericValue.Setup(x => x.ToString()).Returns(value.ToString());
                return mockNumericValue;
            }

            public static Mock<StringValue> MockStringValue(string value)
            {
                var mockStringValue = new Mock<StringValue>();
                mockStringValue.SetupGet(x => x.Type).Returns(DataValueType.String);
                mockStringValue.SetupGet(x => x.Value).Returns(value);
                mockStringValue.SetupGet(x => x.IterationRequired).Returns(false);
                mockStringValue.SetupGet(x => x.IsTruthy).Returns(value != "");
                mockStringValue.Setup(x => x.ToString()).Returns(value);
                return mockStringValue;
            }

            public static Mock<StringValue> MockStringValueWithIteration(string value)
            {
                var mockStringValue = new Mock<StringValue>();
                mockStringValue.SetupGet(x => x.Type).Returns(DataValueType.String);
                mockStringValue.SetupGet(x => x.Value).Returns(value);
                mockStringValue.SetupGet(x => x.IterationRequired).Returns(true);
                mockStringValue.SetupGet(x => x.IterationValues).Returns(value.Select(c => MockStringValue(c.ToString()).Object).ToList());
                mockStringValue.SetupGet(x => x.IsTruthy).Returns(value != "");
                mockStringValue.Setup(x => x.ToString()).Returns(value);
                return mockStringValue;
            }

            public static Mock<ArrayValue> MockArrayValue(params DataValue[] arrayContents)
            {
                var mockArrayValue = new Mock<ArrayValue>();
                mockArrayValue.SetupGet(x => x.Type).Returns(DataValueType.Array);
                mockArrayValue.SetupGet(x => x.Value).Returns(arrayContents);
                mockArrayValue.SetupGet(x => x.IterationRequired).Returns(false);
                mockArrayValue.SetupGet(x => x.IsTruthy).Returns(arrayContents.Length > 0);
                mockArrayValue.Setup(x => x.ToString()).Returns($"[{String.Join(",", arrayContents.Select(a => a.ToString()))}]");
                return mockArrayValue;
            }

            public static Mock<ArrayValue> MockArrayValueWithIteration(params DataValue[] arrayContents)
            {
                var mockArrayValue = new Mock<ArrayValue>();
                mockArrayValue.SetupGet(x => x.Type).Returns(DataValueType.Array);
                mockArrayValue.SetupGet(x => x.Value).Returns(arrayContents);
                mockArrayValue.SetupGet(x => x.IterationRequired).Returns(true);
                mockArrayValue.SetupGet(x => x.IterationValues).Returns(arrayContents);
                mockArrayValue.SetupGet(x => x.IsTruthy).Returns(arrayContents.Length > 0);
                mockArrayValue.Setup(x => x.ToString()).Returns($"[{String.Join(",", arrayContents.Select(a => a.ToString()))}]");
                return mockArrayValue;
            }

            public static Mock<ProgramState> MockProgramState(params DataValue[] dequeueSequence)
            {
                var mockProgramState = new Mock<ProgramState>();

                if (dequeueSequence.Length == 1)
                {
                    mockProgramState.Setup(p => p.DequeueAndEvaluate()).Returns(dequeueSequence[0]);
                }
                else
                {
                    var returnSet = mockProgramState.SetupSequence(p => p.DequeueAndEvaluate());

                    foreach (var v in dequeueSequence)
                    {
                        returnSet.Returns(v);
                    }
                }

                return mockProgramState;
            }
        }

        [Fact]
        public void NumericLiteral_should_evaluate_to_NumericValue_with_correct_contents()
        {
            // Arrange
            var token = new NumericLiteral(1.5m);

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
            var token = new StringLiteral("abc");

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
            var mockQueue = MockFactory.MockProgramState(MockFactory.MockDataValue(true).Object);

            var token = new Truthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Truthify_should_evaluate_falsey_value_as_falsey()
        {
            // Arrange
            var mockQueue = MockFactory.MockProgramState(MockFactory.MockDataValue(false).Object);

            var token = new Truthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Truthify_should_evaluate_over_iterable()
        {
            // Arrange
            var mockTruthyValue = new Mock<DataValue>();
            mockTruthyValue.Setup(m => m.IsTruthy).Returns(true);
            var mockFalseyValue = new Mock<DataValue>();
            mockFalseyValue.Setup(m => m.IsTruthy).Returns(false);

            var mockIteratableArray = new Mock<ArrayValue>();
            mockIteratableArray.SetupGet(x => x.IterationRequired).Returns(true);
            mockIteratableArray.SetupGet(x => x.IterationValues).Returns(new DataValue[] { mockTruthyValue.Object, mockFalseyValue.Object });

            var mockQueue = new Mock<ProgramState>();
            mockQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockIteratableArray.Object);

            var token = new Truthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(1, 0);
        }

        [Fact]
        public void Untruthify_should_evaluate_truthy_value_as_falsey()
        {
            // Arrange
            var mockValue = new Mock<DataValue>();
            mockValue.Setup(m => m.IsTruthy).Returns(true);
            var mockQueue = new Mock<ProgramState>();
            mockQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockValue.Object);
            var token = new UnTruthify();

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
            var token = new UnTruthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(1);
        }

        [Fact]
        public void Untruthify_should_evaluate_over_iterable()
        {
            // Arrange
            var mockTruthyValue = new Mock<DataValue>();
            mockTruthyValue.Setup(m => m.IsTruthy).Returns(true);
            var mockFalseyValue = new Mock<DataValue>();
            mockFalseyValue.Setup(m => m.IsTruthy).Returns(false);

            var mockIteratableArray = new Mock<ArrayValue>();
            mockIteratableArray.SetupGet(x => x.IterationRequired).Returns(true);
            mockIteratableArray.SetupGet(x => x.IterationValues).Returns(new DataValue[] { mockTruthyValue.Object, mockFalseyValue.Object });

            var mockQueue = new Mock<ProgramState>();
            mockQueue.Setup(m => m.DequeueAndEvaluate()).Returns(mockIteratableArray.Object);

            var token = new UnTruthify();

            // Act
            var result = token.Evaluate(mockQueue.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(0, 1);
        }

        [Fact]
        public void SingleArgument_should_default_to_zero_if_unindexed()
        {
            // Arrange
            var mockState = new Mock<ProgramState>();
            mockState.SetupGet(s => s.ArgumentList).Returns(new DataValue[0]);
            var token = new SingleArgument(SingleArgument.CHAR_LIST[0]);

            // Act
            var result = token.Evaluate(mockState.Object);

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

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupGet(s => s.ArgumentList).Returns(arguments);

            var token1 = new SingleArgument(SingleArgument.CHAR_LIST[0]);
            var token2 = new SingleArgument(SingleArgument.CHAR_LIST[1]);
            var token3 = new SingleArgument(SingleArgument.CHAR_LIST[2]);

            // Act
            var result1 = token1.Evaluate(mockProgramState.Object);
            var result2 = token2.Evaluate(mockProgramState.Object);
            var result3 = token3.Evaluate(mockProgramState.Object);

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

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupGet(s => s.ArgumentList).Returns(arguments);

            var token = new ArgumentArray();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareTo(value1.Object, value2.Object, value3.Object);
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
            mockNumeric1.Setup(m => m.Value).Returns(1.5m);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.Setup(m => m.Type).Returns(DataValueType.Numeric);
            mockNumeric2.Setup(m => m.Value).Returns(2.3m);

            var mockQueue = new Mock<ProgramState>();
            mockQueue.SetupSequence(m => m.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var addToken = new Add();

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
            result.ShouldBeOfType<ArrayValue>().CompareTo(0, 1, 2, 3, 4);
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
            result.ShouldBeOfType<ArrayValue>().CompareTo(-4, -3, -2, -1, 0);
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
            result.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
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
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u2192 command - String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u2192 command - Array");
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
            result.ShouldBeOfType<ArrayValue>().CompareTo(4,3,2,1,0);
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
            result.ShouldBeOfType<ArrayValue>().CompareTo(0,-1,-2,-3,-4);
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
            result.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
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
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u2190 command - String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u2190 command - Array");
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
            result.ShouldBeOfType<ArrayValue>().CompareTo(1,2,3,4,5);
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
            result.ShouldBeOfType<ArrayValue>().CompareTo(-5, -4, -3, -2, -1);
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
            result.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
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
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u0411 command - String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u0411 command - Array");
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
            result.ShouldBeOfType<ArrayValue>().CompareTo(5, 4, 3, 2, 1);
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
            result.ShouldBeOfType<ArrayValue>().CompareTo(-1, -2, -3, -4, -5);
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
            result.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
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
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockStringTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u042A command - String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockArrayTokenQueue.Object)).Message.ShouldBe("Invalid argument type passed to \u042A command - Array");
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

            var token0 = new TokenImplementations.GetVariable(GetVariable.CHAR_LIST[0]);
            var token1 = new TokenImplementations.GetVariable(GetVariable.CHAR_LIST[1]);

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
            var mockDataValue = new Mock<DataValue>();
            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.Setup(s => s.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new TokenImplementations.SetVariable(SetVariable.CHAR_LIST[0]);

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBe(mockDataValue.Object);
            mockProgramState.Verify(s => s.SetVariable(0, mockDataValue.Object));
        }

        [Fact]
        public void Multiply_should_multiply_two_numerics()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(-4);
            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(2.5m);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(-10);
        }

        [Fact]
        public void Multiply_should_repeat_string_if_numeric_and_string_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric.SetupGet(n => n.Value).Returns(3);
            var mockString = new Mock<StringValue>();
            mockString.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString.SetupGet(n => n.Value).Returns("abc");

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockString.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockString.Object)
                .Returns(mockNumeric.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("abcabcabc");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("abcabcabc");
        }

        [Fact]
        public void Multiply_should_return_empty_string_if_0_and_string_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric.SetupGet(n => n.Value).Returns(0);
            var mockString = new Mock<StringValue>();
            mockString.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString.SetupGet(n => n.Value).Returns("abc");

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockString.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockString.Object)
                .Returns(mockNumeric.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Multiply_should_return_empty_string_if_numeric_and_empty_string_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric.SetupGet(n => n.Value).Returns(3);
            var mockString = new Mock<StringValue>();
            mockString.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString.SetupGet(n => n.Value).Returns("");

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockString.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockString.Object)
                .Returns(mockNumeric.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Multiply_should_repeat_string_partial_times_if_non_integral_numeric_and_string_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric.SetupGet(n => n.Value).Returns(2.2m);
            var mockString = new Mock<StringValue>();
            mockString.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString.SetupGet(n => n.Value).Returns("abc");

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockString.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockString.Object)
                .Returns(mockNumeric.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("abcabca");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("abcabca");
        }

        [Fact]
        public void Multiply_should_repeat_string_and_reverse_if_negative_numeric_and_string_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric.SetupGet(n => n.Value).Returns(-3);
            var mockString = new Mock<StringValue>();
            mockString.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString.SetupGet(n => n.Value).Returns("abc");

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockString.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockString.Object)
                .Returns(mockNumeric.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("cbacbacba");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("cbacbacba");
        }

        [Fact]
        public void Multiply_should_repeat_string_partial_times_and_reverse_if_negative_non_integral_numeric_and_string_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric.SetupGet(n => n.Value).Returns(-1.2m);
            var mockString = new Mock<StringValue>();
            mockString.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString.SetupGet(n => n.Value).Returns("abc");

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockString.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockString.Object)
                .Returns(mockNumeric.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("acba");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("acba");
        }

        [Fact]
        public void Multiply_should_repeat_array_if_numeric_and_array_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric.SetupGet(n => n.Value).Returns(2);

            var mockVal1 = new Mock<DataValue>();
            var mockVal2 = new Mock<DataValue>();
            var mockVal3 = new Mock<DataValue>();
            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(n => n.Type).Returns(DataValueType.Array);
            mockArray.SetupGet(n => n.Value).Returns(new DataValue[] { mockVal1.Object, mockVal2.Object, mockVal3.Object });

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockArray.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockArray.Object)
                .Returns(mockNumeric.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            var expectedResult = new DataValue[]
            {
                mockVal1.Object,
                mockVal2.Object,
                mockVal3.Object,
                mockVal1.Object,
                mockVal2.Object,
                mockVal3.Object
            };

            result1.ShouldBeOfType<ArrayValue>().CompareTo(expectedResult);
            result2.ShouldBeOfType<ArrayValue>().CompareTo(expectedResult);
        }

        [Fact]
        public void Multiply_should_return_empty_array_if_0_and_array_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric.SetupGet(n => n.Value).Returns(0);

            var mockVal1 = new Mock<DataValue>();
            var mockVal2 = new Mock<DataValue>();
            var mockVal3 = new Mock<DataValue>();
            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(n => n.Type).Returns(DataValueType.Array);
            mockArray.SetupGet(n => n.Value).Returns(new DataValue[] { mockVal1.Object, mockVal2.Object, mockVal3.Object });

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockArray.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockArray.Object)
                .Returns(mockNumeric.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
            result2.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
        }

        [Fact]
        public void Multiply_should_return_empty_array_if_numeric_and_empty_array_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric.SetupGet(n => n.Value).Returns(3);

            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(n => n.Type).Returns(DataValueType.Array);
            mockArray.SetupGet(n => n.Value).Returns(new DataValue[0]);

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockArray.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockArray.Object)
                .Returns(mockNumeric.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
            result2.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
        }

        [Fact]
        public void Multiply_should_repeat_array_partial_times_if_non_integral_numeric_and_array_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric.SetupGet(n => n.Value).Returns(2.2m);

            var mockVal1 = new Mock<DataValue>();
            var mockVal2 = new Mock<DataValue>();
            var mockVal3 = new Mock<DataValue>();
            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(n => n.Type).Returns(DataValueType.Array);
            mockArray.SetupGet(n => n.Value).Returns(new DataValue[] { mockVal1.Object, mockVal2.Object, mockVal3.Object });

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockArray.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockArray.Object)
                .Returns(mockNumeric.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            var expectedResult = new DataValue[]
            {
                mockVal1.Object,
                mockVal2.Object,
                mockVal3.Object,
                mockVal1.Object,
                mockVal2.Object,
                mockVal3.Object,
                mockVal1.Object
            };

            result1.ShouldBeOfType<ArrayValue>().CompareTo(expectedResult);
            result2.ShouldBeOfType<ArrayValue>().CompareTo(expectedResult);
        }

        [Fact]
        public void Multiply_should_repeat_array_and_reverse_if_negative_numeric_and_array_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric.SetupGet(n => n.Value).Returns(-3);

            var mockVal1 = new Mock<DataValue>();
            var mockVal2 = new Mock<DataValue>();
            var mockVal3 = new Mock<DataValue>();
            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(n => n.Type).Returns(DataValueType.Array);
            mockArray.SetupGet(n => n.Value).Returns(new DataValue[] { mockVal1.Object, mockVal2.Object, mockVal3.Object });

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockArray.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockArray.Object)
                .Returns(mockNumeric.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            var expectedResult = new DataValue[]
            {
                mockVal3.Object,
                mockVal2.Object,
                mockVal1.Object,
                mockVal3.Object,
                mockVal2.Object,
                mockVal1.Object,
                mockVal3.Object,
                mockVal2.Object,
                mockVal1.Object
            };

            result1.ShouldBeOfType<ArrayValue>().CompareTo(expectedResult);
            result2.ShouldBeOfType<ArrayValue>().CompareTo(expectedResult);
        }

        [Fact]
        public void Multiply_should_repeat_array_partial_times_and_reverse_if_negative_non_integral_numeric_and_array_passed()
        {
            // Arrange
            var mockNumeric = new Mock<NumericValue>();
            mockNumeric.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric.SetupGet(n => n.Value).Returns(-1.2m);

            var mockVal1 = new Mock<DataValue>();
            var mockVal2 = new Mock<DataValue>();
            var mockVal3 = new Mock<DataValue>();
            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(n => n.Type).Returns(DataValueType.Array);
            mockArray.SetupGet(n => n.Value).Returns(new DataValue[] { mockVal1.Object, mockVal2.Object, mockVal3.Object });

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockNumeric.Object)
                .Returns(mockArray.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2
                .SetupSequence(s => s.DequeueAndEvaluate())
                .Returns(mockArray.Object)
                .Returns(mockNumeric.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            var expectedResult = new DataValue[]
            {
                mockVal1.Object,
                mockVal3.Object,
                mockVal2.Object,
                mockVal1.Object
            };

            result1.ShouldBeOfType<ArrayValue>().CompareTo(expectedResult);
            result2.ShouldBeOfType<ArrayValue>().CompareTo(expectedResult);
        }

        [Fact]
        public void Multiply_should_return_cartesian_product_of_two_strings()
        {
            // Arrange
            var mockString1 = new Mock<StringValue>();
            mockString1.SetupGet(s => s.Type).Returns(DataValueType.String);
            mockString1.SetupGet(s => s.IsTruthy).Returns(true);
            mockString1.SetupGet(s => s.Value).Returns("abc");

            var mockString2 = new Mock<StringValue>();
            mockString2.SetupGet(s => s.Type).Returns(DataValueType.String);
            mockString2.SetupGet(s => s.IsTruthy).Returns(true);
            mockString2.SetupGet(s => s.Value).Returns("xyz");

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockString1.Object)
                .Returns(mockString2.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var arrayResult = result.ShouldBeOfType<ArrayValue>();
            arrayResult.Value.Count.ShouldBe(9);
            arrayResult.Value[0].ShouldBeOfType<ArrayValue>().CompareTo("a", "x");
            arrayResult.Value[1].ShouldBeOfType<ArrayValue>().CompareTo("a", "y");
            arrayResult.Value[2].ShouldBeOfType<ArrayValue>().CompareTo("a", "z");
            arrayResult.Value[3].ShouldBeOfType<ArrayValue>().CompareTo("b", "x");
            arrayResult.Value[4].ShouldBeOfType<ArrayValue>().CompareTo("b", "y");
            arrayResult.Value[5].ShouldBeOfType<ArrayValue>().CompareTo("b", "z");
            arrayResult.Value[6].ShouldBeOfType<ArrayValue>().CompareTo("c", "x");
            arrayResult.Value[7].ShouldBeOfType<ArrayValue>().CompareTo("c", "y");
            arrayResult.Value[8].ShouldBeOfType<ArrayValue>().CompareTo("c", "z");
        }

        [Fact]
        public void Multiply_should_return_empty_array_for_empty_string_and_string()
        {
            // Arrange
            var mockString1 = new Mock<StringValue>();
            mockString1.SetupGet(s => s.Type).Returns(DataValueType.String);
            mockString1.SetupGet(s => s.IsTruthy).Returns(true);
            mockString1.SetupGet(s => s.Value).Returns("abc");

            var mockString2 = new Mock<StringValue>();
            mockString2.SetupGet(s => s.Type).Returns(DataValueType.String);
            mockString2.SetupGet(s => s.IsTruthy).Returns(false);
            mockString2.SetupGet(s => s.Value).Returns("");

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockString1.Object)
                .Returns(mockString2.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
        }

        [Fact]
        public void Multiply_should_return_cartesian_product_of_two_arrays()
        {
            // Arrange
            var mockVal1 = new Mock<DataValue>();
            var mockVal2 = new Mock<DataValue>();
            var mockVal3 = new Mock<DataValue>();
            var mockVal4 = new Mock<DataValue>();
            var mockVal5 = new Mock<DataValue>();
            var mockVal6 = new Mock<DataValue>();

            var mockArray1 = new Mock<ArrayValue>();
            mockArray1.SetupGet(s => s.Type).Returns(DataValueType.Array);
            mockArray1.SetupGet(s => s.IsTruthy).Returns(true);
            mockArray1.SetupGet(s => s.Value).Returns(new DataValue[] { mockVal1.Object, mockVal2.Object, mockVal3.Object });

            var mockArray2 = new Mock<ArrayValue>();
            mockArray2.SetupGet(s => s.Type).Returns(DataValueType.Array);
            mockArray2.SetupGet(s => s.IsTruthy).Returns(true);
            mockArray2.SetupGet(s => s.Value).Returns(new DataValue[] { mockVal4.Object, mockVal5.Object, mockVal6.Object });

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockArray1.Object)
                .Returns(mockArray2.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var arrayResult = result.ShouldBeOfType<ArrayValue>();
            arrayResult.Value.Count.ShouldBe(9);
            arrayResult.Value[0].ShouldBeOfType<ArrayValue>().CompareTo(mockVal1.Object, mockVal4.Object);
            arrayResult.Value[1].ShouldBeOfType<ArrayValue>().CompareTo(mockVal1.Object, mockVal5.Object);
            arrayResult.Value[2].ShouldBeOfType<ArrayValue>().CompareTo(mockVal1.Object, mockVal6.Object);
            arrayResult.Value[3].ShouldBeOfType<ArrayValue>().CompareTo(mockVal2.Object, mockVal4.Object);
            arrayResult.Value[4].ShouldBeOfType<ArrayValue>().CompareTo(mockVal2.Object, mockVal5.Object);
            arrayResult.Value[5].ShouldBeOfType<ArrayValue>().CompareTo(mockVal2.Object, mockVal6.Object);
            arrayResult.Value[6].ShouldBeOfType<ArrayValue>().CompareTo(mockVal3.Object, mockVal4.Object);
            arrayResult.Value[7].ShouldBeOfType<ArrayValue>().CompareTo(mockVal3.Object, mockVal5.Object);
            arrayResult.Value[8].ShouldBeOfType<ArrayValue>().CompareTo(mockVal3.Object, mockVal6.Object);
        }

        [Fact]
        public void Multiply_should_return_empty_array_for_empty_array_and_array()
        {
            // Arrange
            var mockVal1 = new Mock<DataValue>();
            var mockVal2 = new Mock<DataValue>();
            var mockVal3 = new Mock<DataValue>();

            var mockArray1 = new Mock<ArrayValue>();
            mockArray1.SetupGet(s => s.Type).Returns(DataValueType.Array);
            mockArray1.SetupGet(s => s.IsTruthy).Returns(true);
            mockArray1.SetupGet(s => s.Value).Returns(new DataValue[] { mockVal1.Object, mockVal2.Object, mockVal3.Object });

            var mockArray2 = new Mock<ArrayValue>();
            mockArray2.SetupGet(s => s.Type).Returns(DataValueType.Array);
            mockArray2.SetupGet(s => s.IsTruthy).Returns(false);
            mockArray2.SetupGet(s => s.Value).Returns(new DataValue[0]);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockArray1.Object)
                .Returns(mockArray2.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
        }

        [Fact]
        public void Multiply_should_return_cartesian_product_of_string_and_array()
        {
            // Arrange
            var mockVal1 = new Mock<DataValue>();
            var mockVal2 = new Mock<DataValue>();
            var mockVal3 = new Mock<DataValue>();

            var mockArray = new Mock<ArrayValue>();
            mockArray.SetupGet(s => s.Type).Returns(DataValueType.Array);
            mockArray.SetupGet(s => s.IsTruthy).Returns(true);
            mockArray.SetupGet(s => s.Value).Returns(new DataValue[] { mockVal1.Object, mockVal2.Object, mockVal3.Object });

            var mockString = new Mock<StringValue>();
            mockString.SetupGet(s => s.Type).Returns(DataValueType.String);
            mockString.SetupGet(s => s.IsTruthy).Returns(true);
            mockString.SetupGet(s => s.Value).Returns("abc");

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockArray.Object)
                .Returns(mockString.Object);

            var token = new TokenImplementations.Multiply();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var arrayResult = result.ShouldBeOfType<ArrayValue>();
            arrayResult.Value.Count.ShouldBe(9);

            var a1 = arrayResult.Value[0].ShouldBeOfType<ArrayValue>();
            a1.Value.Count.ShouldBe(2);
            a1.Value[0].ShouldBe(mockVal1.Object);
            a1.Value[1].ShouldBeOfType<StringValue>().Value.ShouldBe("a");

            var a2 = arrayResult.Value[1].ShouldBeOfType<ArrayValue>();
            a2.Value.Count.ShouldBe(2);
            a2.Value[0].ShouldBe(mockVal1.Object);
            a2.Value[1].ShouldBeOfType<StringValue>().Value.ShouldBe("b");

            var a3 = arrayResult.Value[2].ShouldBeOfType<ArrayValue>();
            a3.Value.Count.ShouldBe(2);
            a3.Value[0].ShouldBe(mockVal1.Object);
            a3.Value[1].ShouldBeOfType<StringValue>().Value.ShouldBe("c");
            
            var a4 = arrayResult.Value[3].ShouldBeOfType<ArrayValue>();
            a4.Value.Count.ShouldBe(2);
            a4.Value[0].ShouldBe(mockVal2.Object);
            a4.Value[1].ShouldBeOfType<StringValue>().Value.ShouldBe("a");
            
            var a5 = arrayResult.Value[4].ShouldBeOfType<ArrayValue>();
            a5.Value.Count.ShouldBe(2);
            a5.Value[0].ShouldBe(mockVal2.Object);
            a5.Value[1].ShouldBeOfType<StringValue>().Value.ShouldBe("b");
            
            var a6 = arrayResult.Value[5].ShouldBeOfType<ArrayValue>();
            a6.Value.Count.ShouldBe(2);
            a6.Value[0].ShouldBe(mockVal2.Object);
            a6.Value[1].ShouldBeOfType<StringValue>().Value.ShouldBe("c");
            
            var a7 = arrayResult.Value[6].ShouldBeOfType<ArrayValue>();
            a7.Value.Count.ShouldBe(2);
            a7.Value[0].ShouldBe(mockVal3.Object);
            a7.Value[1].ShouldBeOfType<StringValue>().Value.ShouldBe("a");
            
            var a8 = arrayResult.Value[7].ShouldBeOfType<ArrayValue>();
            a8.Value.Count.ShouldBe(2);
            a8.Value[0].ShouldBe(mockVal3.Object);
            a8.Value[1].ShouldBeOfType<StringValue>().Value.ShouldBe("b");
            
            var a9 = arrayResult.Value[8].ShouldBeOfType<ArrayValue>();
            a9.Value.Count.ShouldBe(2);
            a9.Value[0].ShouldBe(mockVal3.Object);
            a9.Value[1].ShouldBeOfType<StringValue>().Value.ShouldBe("c");
        }

        [Fact]
        public void Equality_should_return_truthy_for_equal_numerics()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4m);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.4m);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var token = new TokenImplementations.Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Equality_should_return_falsey_for_different_numerics()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4m);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.5m);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var token = new TokenImplementations.Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Equality_should_return_truthy_for_equal_strings()
        {
            // Arrange
            var mockString1 = new Mock<StringValue>();
            mockString1.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString1.SetupGet(n => n.Value).Returns("abc");

            var mockString2 = new Mock<StringValue>();
            mockString2.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString2.SetupGet(n => n.Value).Returns("abc");

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockString1.Object)
                .Returns(mockString2.Object);

            var token = new TokenImplementations.Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Equality_should_return_falsey_for_different_strings()
        {
            // Arrange
            var mockString1 = new Mock<StringValue>();
            mockString1.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString1.SetupGet(n => n.Value).Returns("abc");

            var mockString2 = new Mock<StringValue>();
            mockString2.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString2.SetupGet(n => n.Value).Returns("xyz");

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockString1.Object)
                .Returns(mockString2.Object);

            var token = new TokenImplementations.Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Equality_should_return_truthy_for_equal_arrays()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4m);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.4m);

            var mockString1 = new Mock<StringValue>();
            mockString1.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString1.SetupGet(n => n.Value).Returns("abc");

            var mockString2 = new Mock<StringValue>();
            mockString2.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString2.SetupGet(n => n.Value).Returns("abc");

            var mockArray1 = new Mock<ArrayValue>();
            mockArray1.SetupGet(n => n.Type).Returns(DataValueType.Array);
            mockArray1.SetupGet(n => n.Value).Returns(new DataValue[] { mockNumeric1.Object, mockString1.Object });

            var mockArray2 = new Mock<ArrayValue>();
            mockArray2.SetupGet(n => n.Type).Returns(DataValueType.Array);
            mockArray2.SetupGet(n => n.Value).Returns(new DataValue[] { mockNumeric2.Object, mockString2.Object });

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockArray1.Object)
                .Returns(mockArray2.Object);

            var token = new TokenImplementations.Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Equality_should_return_falsey_for_equal_arrays_in_different_orders()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4m);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.4m);

            var mockString1 = new Mock<StringValue>();
            mockString1.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString1.SetupGet(n => n.Value).Returns("abc");

            var mockString2 = new Mock<StringValue>();
            mockString2.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString2.SetupGet(n => n.Value).Returns("abc");

            var mockArray1 = new Mock<ArrayValue>();
            mockArray1.SetupGet(n => n.Type).Returns(DataValueType.Array);
            mockArray1.SetupGet(n => n.Value).Returns(new DataValue[] { mockNumeric1.Object, mockString1.Object });

            var mockArray2 = new Mock<ArrayValue>();
            mockArray2.SetupGet(n => n.Type).Returns(DataValueType.Array);
            mockArray2.SetupGet(n => n.Value).Returns(new DataValue[] { mockString2.Object, mockNumeric2.Object });

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockArray1.Object)
                .Returns(mockArray2.Object);

            var token = new TokenImplementations.Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Equality_should_return_falsey_for_unequal_arrays()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4m);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.5m);

            var mockString1 = new Mock<StringValue>();
            mockString1.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString1.SetupGet(n => n.Value).Returns("abc");

            var mockString2 = new Mock<StringValue>();
            mockString2.SetupGet(n => n.Type).Returns(DataValueType.String);
            mockString2.SetupGet(n => n.Value).Returns("abc");

            var mockArray1 = new Mock<ArrayValue>();
            mockArray1.SetupGet(n => n.Type).Returns(DataValueType.Array);
            mockArray1.SetupGet(n => n.Value).Returns(new DataValue[] { mockNumeric1.Object, mockString1.Object });

            var mockArray2 = new Mock<ArrayValue>();
            mockArray2.SetupGet(n => n.Type).Returns(DataValueType.Array);
            mockArray2.SetupGet(n => n.Value).Returns(new DataValue[] { mockNumeric2.Object, mockString2.Object });

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockArray1.Object)
                .Returns(mockArray2.Object);

            var token = new TokenImplementations.Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact(Skip = "This is really hard to test, how to go about doing it?")]
        public void Where_should_do_things()
        {
            // Arrange

            // Act

            // Assert

        }

        [Fact]
        public void LogicAnd_should_return_truthy_when_both_operands_truthy()
        {
            // Arrange
            var mockTruthyValue = new Mock<DataValue>();
            mockTruthyValue.SetupGet(t => t.IsTruthy).Returns(true);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.Setup(p => p.DequeueAndEvaluate()).Returns(mockTruthyValue.Object);

            var token = new TokenImplementations.LogicAnd();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void LogicAnd_should_return_falsey_when_second_operand_falsey()
        {
            // Arrange
            var mockTruthyValue = new Mock<DataValue>();
            mockTruthyValue.SetupGet(t => t.IsTruthy).Returns(true);

            var mockFalseyValue = new Mock<DataValue>();
            mockFalseyValue.SetupGet(t => t.IsTruthy).Returns(false);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockTruthyValue.Object)
                .Returns(mockFalseyValue.Object);

            var token = new TokenImplementations.LogicAnd();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void LogicAnd_should_return_falsey_when_first_operand_falsey_without_evaluating_second_operand()
        {
            // Arrange
            var mockFalseyValue = new Mock<DataValue>();
            mockFalseyValue.SetupGet(t => t.IsTruthy).Returns(false);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.Setup(p => p.DequeueAndEvaluate()).Returns(mockFalseyValue.Object);
            mockProgramState.Setup(p => p.StepOverNextTokenBlock());

            var token = new TokenImplementations.LogicAnd();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            mockProgramState.Verify(p => p.DequeueAndEvaluate(), Times.Once);
            mockProgramState.Verify(p => p.StepOverNextTokenBlock(), Times.Once);
        }

        [Fact]
        public void LogicOr_should_return_falsey_when_both_operands_falsey()
        {
            // Arrange
            var mockFalseyValue = new Mock<DataValue>();
            mockFalseyValue.SetupGet(t => t.IsTruthy).Returns(false);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.Setup(p => p.DequeueAndEvaluate()).Returns(mockFalseyValue.Object);

            var token = new TokenImplementations.LogicOr();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void LogicOr_should_return_truthy_when_second_operand_truthy()
        {
            // Arrange
            var mockTruthyValue = new Mock<DataValue>();
            mockTruthyValue.SetupGet(t => t.IsTruthy).Returns(true);

            var mockFalseyValue = new Mock<DataValue>();
            mockFalseyValue.SetupGet(t => t.IsTruthy).Returns(false);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockFalseyValue.Object)
                .Returns(mockTruthyValue.Object);

            var token = new TokenImplementations.LogicOr();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void LogicOr_should_return_truthy_when_first_operand_truthy_without_evaluating_second_operand()
        {
            // Arrange
            var mockTruthyValue = new Mock<DataValue>();
            mockTruthyValue.SetupGet(t => t.IsTruthy).Returns(true);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.Setup(p => p.DequeueAndEvaluate()).Returns(mockTruthyValue.Object);
            mockProgramState.Setup(p => p.StepOverNextTokenBlock());

            var token = new TokenImplementations.LogicOr();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            mockProgramState.Verify(p => p.DequeueAndEvaluate(), Times.Once);
            mockProgramState.Verify(p => p.StepOverNextTokenBlock(), Times.Once);
        }

        [Fact]
        public void LogicXor_should_follow_xor_truth_table()
        {
            // Arrange
            var mockTruthyValue = new Mock<DataValue>();
            mockTruthyValue.SetupGet(t => t.IsTruthy).Returns(true);

            var mockFalseyValue = new Mock<DataValue>();
            mockFalseyValue.SetupGet(t => t.IsTruthy).Returns(false);

            // Test 1 - false,false => false
            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockFalseyValue.Object).Returns(mockFalseyValue.Object);

            // Test 2 - false,true => true
            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockFalseyValue.Object).Returns(mockTruthyValue.Object);

            // Test 3 - true,false => true
            var mockProgramState3 = new Mock<ProgramState>();
            mockProgramState3.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockTruthyValue.Object).Returns(mockFalseyValue.Object);

            // Test 4 - true,true => false
            var mockProgramState4 = new Mock<ProgramState>();
            mockProgramState4.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockTruthyValue.Object).Returns(mockTruthyValue.Object);

            var token = new TokenImplementations.LogicXor();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void LogicXnor_should_follow_xnor_truth_table()
        {
            // Arrange
            var mockTruthyValue = new Mock<DataValue>();
            mockTruthyValue.SetupGet(t => t.IsTruthy).Returns(true);

            var mockFalseyValue = new Mock<DataValue>();
            mockFalseyValue.SetupGet(t => t.IsTruthy).Returns(false);

            // Test 1 - false,false => true
            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockFalseyValue.Object).Returns(mockFalseyValue.Object);

            // Test 2 - false,true => false
            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockFalseyValue.Object).Returns(mockTruthyValue.Object);

            // Test 3 - true,false => false
            var mockProgramState3 = new Mock<ProgramState>();
            mockProgramState3.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockTruthyValue.Object).Returns(mockFalseyValue.Object);

            // Test 4 - true,true => true
            var mockProgramState4 = new Mock<ProgramState>();
            mockProgramState4.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockTruthyValue.Object).Returns(mockTruthyValue.Object);

            var token = new TokenImplementations.LogicXnor();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

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
            mockPositiveFloat.SetupGet(x => x.Value).Returns(1.23m);

            var mockNegativeFloat = new Mock<NumericValue>();
            mockNegativeFloat.SetupGet(x => x.Type).Returns(DataValueType.Numeric);
            mockNegativeFloat.SetupGet(x => x.Value).Returns(-10.5m);

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
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(1.23m);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(10.5m);
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

        [Fact]
        public void Division_should_calculate_float_division_between_numerics()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(2);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(15);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var token = new Division();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(7.5m);
        }

        [Fact]
        public void Division_should_throw_exception_if_1st_argument_0()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(0);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(15);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var token = new Division();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Division by zero attempted");
        }

        [Fact]
        public void Division_should_error_if_non_numerics_passed()
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

            var token = new Division();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to / command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to / command - Numeric,Array");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to / command - String,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to / command - Array,Numeric");
        }

        [Fact]
        public void Arrayify_should_wrap_any_value_in_an_array()
        {
            // Arrange
            var mockDataValue = new Mock<DataValue>();

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.Setup(p => p.DequeueAndEvaluate()).Returns(mockDataValue.Object);

            var token = new Arrayify();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var arrayResult = result.ShouldBeOfType<ArrayValue>();
            arrayResult.Value.Count.ShouldBe(1);
            arrayResult.Value[0].ShouldBe(mockDataValue.Object);
        }

        [Fact]
        public void ArrayPair_should_wrap_any_two_values_in_an_array()
        {
            // Arrange
            var mockDataValue1 = new Mock<DataValue>();
            var mockDataValue2 = new Mock<DataValue>();

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockDataValue1.Object)
                .Returns(mockDataValue2.Object);

            var token = new ArrayPair();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var arrayResult = result.ShouldBeOfType<ArrayValue>();
            arrayResult.Value.Count.ShouldBe(2);
            arrayResult.Value[0].ShouldBe(mockDataValue1.Object);
            arrayResult.Value[1].ShouldBe(mockDataValue2.Object);
        }

        [Fact]
        public void ConstantNewline_should_evaluate_to_newline_string()
        {
            // Arrange
            var mockProgramState = new Mock<ProgramState>();

            var token = new ConstantNewline();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("\n");
        }

        [Fact]
        public void ConstantEmptyArray_should_evaluate_to_empty_array()
        {
            // Arrange
            var mockProgramState = new Mock<ProgramState>();

            var token = new ConstantEmptyArray();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().Value.Count.ShouldBe(0);
        }

        [Fact]
        public void ConstantEmptyString_should_evaluate_to_empty_string()
        {
            // Arrange
            var mockProgramState = new Mock<ProgramState>();

            var token = new ConstantEmptyString();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Subtract_should_calculate_mod_between_numerics()
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

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockNumeric.Object).Returns(mockString.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockNumeric.Object).Returns(mockArray.Object);

            var mockProgramState3 = new Mock<ProgramState>();
            mockProgramState3.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockString.Object).Returns(mockNumeric.Object);

            var mockProgramState4 = new Mock<ProgramState>();
            mockProgramState4.SetupSequence(p => p.DequeueAndEvaluate()).Returns(mockArray.Object).Returns(mockNumeric.Object);

            var token = new Subtract();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to - command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to - command - Numeric,Array");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to - command - String,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to - command - Array,Numeric");
        }

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

        [Fact]
        public void GetRandomDecimal_should_return_numeric_less_than_1()
        {
            // Arrange
            var mockProgramState = new Mock<ProgramState>();

            var token = new GetRandomDecimal();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBeGreaterThanOrEqualTo(0);
            numericResult.Value.ShouldBeLessThan(1);
        }

        [Fact]
        public void LessThan_should_evaluate_inequality_between_numerics()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(7);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(18);

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric2.Object)
                .Returns(mockNumeric1.Object);

            var token = new LessThan();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void LessThan_should_error_if_non_numerics_passed()
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

            var token = new LessThan();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to < command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to < command - Numeric,Array");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to < command - String,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to < command - Array,Numeric");
        }

        [Fact]
        public void GreaterThan_should_evaluate_inequality_between_numerics()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(7);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(18);

            var mockProgramState1 = new Mock<ProgramState>();
            mockProgramState1.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var mockProgramState2 = new Mock<ProgramState>();
            mockProgramState2.SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric2.Object)
                .Returns(mockNumeric1.Object);

            var token = new GreaterThan();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void GreaterThan_should_error_if_non_numerics_passed()
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

            var token = new GreaterThan();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to > command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to > command - Numeric,Array");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to > command - String,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to > command - Array,Numeric");
            
        }

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
