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
    public class ComparisonsTests
    {
        [Fact]
        public void Equality_should_return_truthy_for_equal_numerics()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.4);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var token = new Equality();

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
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.5);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var token = new Equality();

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

            var token = new Equality();

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

            var token = new Equality();

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
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.4);

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

            var token = new Equality();

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
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.4);

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

            var token = new Equality();

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
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.5);

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

            var token = new Equality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Inequality_should_return_falsey_for_equal_numerics()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.4);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Inequality_should_return_truthy_for_different_numerics()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.5);

            var mockProgramState = new Mock<ProgramState>();
            mockProgramState
                .SetupSequence(p => p.DequeueAndEvaluate())
                .Returns(mockNumeric1.Object)
                .Returns(mockNumeric2.Object);

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Inequality_should_return_falsey_for_equal_strings()
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

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Inequality_should_return_truthy_for_different_strings()
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

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Inequality_should_return_falsey_for_equal_arrays()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.4);

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

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Inequality_should_return_truthy_for_equal_arrays_in_different_orders()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.4);

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

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Inequality_should_return_truthy_for_unequal_arrays()
        {
            // Arrange
            var mockNumeric1 = new Mock<NumericValue>();
            mockNumeric1.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric1.SetupGet(n => n.Value).Returns(1.4);

            var mockNumeric2 = new Mock<NumericValue>();
            mockNumeric2.SetupGet(n => n.Type).Returns(DataValueType.Numeric);
            mockNumeric2.SetupGet(n => n.Value).Returns(1.5);

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

            var token = new Inequality();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
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
    }
}
