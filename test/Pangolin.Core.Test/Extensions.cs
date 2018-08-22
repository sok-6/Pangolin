using Moq;
using Pangolin.Core.DataValueImplementations;
using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.Test
{
    public static class Extensions
    {
        public static void CompareArrayTo(this ArrayValue arrayValue, params DataValue[] comparison)
        {
            arrayValue.Value.Count.ShouldBe(comparison.Length);

            for (int i = 0; i < arrayValue.Value.Count; i++)
            {
                arrayValue.Value[i].ShouldBe(comparison[i]);
            }
        }

        public static void CompareTo(this ArrayValue arrayValue, params double[] comparison)
        {
            arrayValue.Value.Count.ShouldBe(comparison.Length);

            for (int i = 0; i < arrayValue.Value.Count; i++)
            {
                arrayValue.Value[i].ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(comparison[i]);
            }
        }

        public static void CompareTo(this ArrayValue arrayValue, params string[] comparison)
        {
            arrayValue.Value.Count.ShouldBe(comparison.Length);

            for (int i = 0; i < arrayValue.Value.Count; i++)
            {
                arrayValue.Value[i].ShouldBeAssignableTo<StringValue>().Value.ShouldBe(comparison[i]);
            }
        }

        public static void ShouldBeParsedAs(this char token, Type tokenType)
        {
            // Arrange
            var code = token.ToString();
            var mockLog = new Mock<Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);
            result[0].ShouldBeOfType(tokenType);
        }

        public static TestResultArrayContents ShouldBeArrayWhichStartsWith(this DataValue dataValue, params double[] numerics)
        {
            var values = dataValue.ShouldBeAssignableTo<ArrayValue>().Value;

            for (int i = 0; i < numerics.Length; i++)
            {
                values[i].ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(numerics[i]);
            }

            return new TestResultArrayContents(values.Skip(numerics.Length).ToList());
        }

        public static TestResultArrayContents ShouldBeArrayWhichStartsWith(this DataValue dataValue, params string[] strings)
        {
            var values = dataValue.ShouldBeAssignableTo<ArrayValue>().Value;

            for (int i = 0; i < strings.Length; i++)
            {
                values[i].ShouldBeAssignableTo<StringValue>().Value.ShouldBe(strings[i]);
            }

            return new TestResultArrayContents(values.Skip(strings.Length).ToList());
        }

        public static TestResultArrayContents ShouldBeArrayWhichStartsWith(this DataValue dataValue, Action<DataValue> arrayVerificationAction)
        {
            var values = dataValue.ShouldBeAssignableTo<ArrayValue>().Value;

            arrayVerificationAction(values[0]);

            return new TestResultArrayContents(values.Skip(1).ToList());
        }

        public static void ShouldBeEmptyArray(this DataValue dataValue)
        {
            dataValue.ShouldBeAssignableTo<ArrayValue>().Value.Count.ShouldBe(0);
        }

        public static TestResultArrayContents ThenShouldContinueWith(this TestResultArrayContents dataValues, params double[] numerics)
        {
            for (int i = 0; i < numerics.Length; i++)
            {
                dataValues[i].ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(numerics[i]);
            }

            return dataValues.StepOver(numerics.Length);
        }

        public static TestResultArrayContents ThenShouldContinueWith(this TestResultArrayContents dataValues, params string[] strings)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                dataValues[i].ShouldBeAssignableTo<StringValue>().Value.ShouldBe(strings[i]);
            }

            return dataValues.StepOver(strings.Length);
        }

        public static TestResultArrayContents ThenShouldContinueWith(this TestResultArrayContents dataValues, Action<DataValue> arrayVerificationAction)
        {
            arrayVerificationAction(dataValues[0]);

            return dataValues.StepOver(1);
        }

        public static void End(this TestResultArrayContents dataValues)
        {
            dataValues.Count.ShouldBe(0);
        }
    }

    public class TestResultArrayContents
    {
        private IReadOnlyList<DataValue> _values;

        public DataValue this[int index] => _values[index];

        public int Count => _values.Count;

        public TestResultArrayContents(IReadOnlyList<DataValue> values)
        {
            _values = values;
        }

        public TestResultArrayContents StepOver(int steps) => new TestResultArrayContents(_values.Skip(steps).ToList());
    }
}
