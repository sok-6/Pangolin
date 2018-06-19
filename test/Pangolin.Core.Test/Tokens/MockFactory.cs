using Moq;
using Pangolin.Core.DataValueImplementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.Test.Tokens
{
    public static class MockFactory
    {
        //private static Lazy<Mock<NumericValue>> _mockTruthy = new Lazy<Mock<NumericValue>>(() =>
        //{
        //    var mockTruthy = new Mock<NumericValue>();
        //    mockTruthy.SetupGet(x => x.Type).Returns(DataValueType.Numeric);
        //    mockTruthy.SetupGet(x => x.Value).Returns(1);
        //    mockTruthy.SetupGet(x => x.IterationRequired).Returns(false);
        //    mockTruthy.SetupGet(x => x.IsTruthy).Returns(true);
        //    mockTruthy.Setup(x => x.ToString()).Returns("1");
        //    return mockTruthy;
        //});

        //private static Lazy<Mock<NumericValue>> _mockFalsey = new Lazy<Mock<NumericValue>>(() =>
        //{
        //    var mockFalsey = new Mock<NumericValue>();
        //    mockFalsey.SetupGet(x => x.Type).Returns(DataValueType.Numeric);
        //    mockFalsey.SetupGet(x => x.Value).Returns(0);
        //    mockFalsey.SetupGet(x => x.IterationRequired).Returns(false);
        //    mockFalsey.SetupGet(x => x.IsTruthy).Returns(false);
        //    mockFalsey.Setup(x => x.ToString()).Returns("0");
        //    return mockFalsey;
        //});

        public static Mock<DataValue> MockDataValue(bool isTruthy = false)
        {
            var mockDataValue = new Mock<DataValue>();
            mockDataValue.SetupGet(x => x.IsTruthy).Returns(isTruthy);
            return mockDataValue;
        }

        public static Mock<NumericValue> MockNumericValue(double value)
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

        //public static Mock<ArrayValue> MockArrayValue(params DataValue[] arrayContents)
        //{
        //    var mockArrayValue = new Mock<ArrayValue>();
        //    mockArrayValue.SetupGet(x => x.Type).Returns(DataValueType.Array);
        //    mockArrayValue.SetupGet(x => x.Value).Returns(arrayContents);
        //    mockArrayValue.SetupGet(x => x.IterationRequired).Returns(false);
        //    mockArrayValue.SetupGet(x => x.IsTruthy).Returns(arrayContents.Length > 0);
        //    mockArrayValue.Setup(x => x.ToString()).Returns($"[{String.Join(",", arrayContents.Select(a => a.ToString()))}]");
        //    return mockArrayValue;
        //}

        //public static Mock<ArrayValue> MockArrayValue(params double[] arrayContents)
        //{
        //    var mockArrayContents = arrayContents.Select(a => MockNumericValue(a).Object).ToArray();

        //    var mockArrayValue = new Mock<ArrayValue>();
        //    mockArrayValue.SetupGet(x => x.Type).Returns(DataValueType.Array);
        //    mockArrayValue.SetupGet(x => x.Value).Returns(mockArrayContents);
        //    mockArrayValue.SetupGet(x => x.IterationRequired).Returns(false);
        //    mockArrayValue.SetupGet(x => x.IsTruthy).Returns(arrayContents.Length > 0);
        //    mockArrayValue.Setup(x => x.ToString()).Returns($"[{String.Join(",", arrayContents.Select(a => a.ToString()))}]");
        //    return mockArrayValue;
        //}

        //public static Mock<ArrayValue> MockArrayValueWithIteration(params DataValue[] arrayContents)
        //{
        //    var mockArrayValue = new Mock<ArrayValue>();
        //    mockArrayValue.SetupGet(x => x.Type).Returns(DataValueType.Array);
        //    mockArrayValue.SetupGet(x => x.Value).Returns(arrayContents);
        //    mockArrayValue.SetupGet(x => x.IterationRequired).Returns(true);
        //    mockArrayValue.SetupGet(x => x.IterationValues).Returns(arrayContents);
        //    mockArrayValue.SetupGet(x => x.IsTruthy).Returns(arrayContents.Length > 0);
        //    mockArrayValue.Setup(x => x.ToString()).Returns($"[{String.Join(",", arrayContents.Select(a => a.ToString()))}]");
        //    return mockArrayValue;
        //}

        //public static Mock<ArrayValue> MockArrayValueWithIteration(params double[] arrayContents)
        //{
        //    var mockArrayContents = arrayContents.Select(a => MockNumericValue(a).Object).ToArray();

        //    var mockArrayValue = new Mock<ArrayValue>();
        //    mockArrayValue.SetupGet(x => x.Type).Returns(DataValueType.Array);
        //    mockArrayValue.SetupGet(x => x.Value).Returns(mockArrayContents);
        //    mockArrayValue.SetupGet(x => x.IterationRequired).Returns(true);
        //    mockArrayValue.SetupGet(x => x.IterationValues).Returns(mockArrayContents);
        //    mockArrayValue.SetupGet(x => x.IsTruthy).Returns(arrayContents.Length > 0);
        //    mockArrayValue.Setup(x => x.ToString()).Returns($"[{String.Join(",", arrayContents.Select(a => a.ToString()))}]");
        //    return mockArrayValue;
        //}

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

        public class MockArrayBuilder
        {
            private List<DataValue> _valueList;

            private MockArrayBuilder()
            {
                _valueList = new List<DataValue>();
            }

            public static ArrayValue Empty => new MockArrayBuilder().Complete();

            public static MockArrayBuilder StartingNumerics(params double[] numerics) => new MockArrayBuilder().WithNumerics(numerics);
            public static MockArrayBuilder StartingStrings(params string[] strings) => new MockArrayBuilder().WithStrings(strings);
            public static MockArrayBuilder StartingArray(ArrayValue arrayValue) => new MockArrayBuilder().WithArray(arrayValue);

            public MockArrayBuilder WithNumerics(params double[] numerics)
            {
                _valueList.AddRange(numerics.Select(n => MockNumericValue(n).Object));
                return this;
            }

            public MockArrayBuilder WithStrings(params string[] strings)
            {
                _valueList.AddRange(strings.Select(s => MockStringValue(s).Object));
                return this;
            }

            public MockArrayBuilder WithArray(ArrayValue arrayValue)
            {
                _valueList.Add(arrayValue);
                return this;
            }

            private ArrayValue Complete(bool iterationRequired)
            {
                var mockArrayValue = new Mock<ArrayValue>();

                mockArrayValue.SetupGet(x => x.Type).Returns(DataValueType.Array);
                mockArrayValue.SetupGet(x => x.Value).Returns(_valueList);
                mockArrayValue.SetupGet(x => x.IterationRequired).Returns(iterationRequired);
                mockArrayValue.SetupGet(x => x.IterationValues).Returns(_valueList);
                mockArrayValue.SetupGet(x => x.IsTruthy).Returns(_valueList.Count > 0);
                mockArrayValue.Setup(x => x.ToString()).Returns($"[{String.Join(",", _valueList.Select(a => a.ToString()))}]");

                return mockArrayValue.Object;
            }

            public ArrayValue Complete() => Complete(false);
            public ArrayValue CompleteIterationRequired() => Complete(true);
        }
    }
}
