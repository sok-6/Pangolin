using Moq;
using Pangolin.Core.DataValueImplementations;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pangolin.Core.Test.Tokens.Implementations
{
    public class MultiplyTests
    {
        [Fact]
        public void Multiply_should_multiply_two_numerics()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(-4);
            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(2.5);

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
            mockNumeric.SetupGet(n => n.Value).Returns(2.2);
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
            mockNumeric.SetupGet(n => n.Value).Returns(-1.2);
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
            mockNumeric.SetupGet(n => n.Value).Returns(2.2);

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
            mockNumeric.SetupGet(n => n.Value).Returns(-1.2);

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
    }
}
