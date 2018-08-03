using Moq;
using Pangolin.Core.DataValueImplementations;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pangolin.Core.Test
{
    public static class Extensions
    {
        public static void CompareTo(this ArrayValue arrayValue, params DataValue[] comparison)
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

        public static IReadOnlyList<DataValue> ShouldBeArrayWhichStartsWith(this DataValue dataValue, params double[] numerics)
        {
            var values = dataValue.ShouldBeAssignableTo<ArrayValue>().Value;

            for (int i = 0; i < numerics.Length; i++)
            {
                values[i].ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(numerics[i]);
            }

            return values.Skip(numerics.Length).ToList();
        }

        public static IReadOnlyList<DataValue> ShouldBeArrayWhichStartsWith(this DataValue dataValue, params string[] strings)
        {
            var values = dataValue.ShouldBeAssignableTo<ArrayValue>().Value;

            for (int i = 0; i < strings.Length; i++)
            {
                values[i].ShouldBeAssignableTo<StringValue>().Value.ShouldBe(strings[i]);
            }

            return values.Skip(strings.Length).ToList();
        }

        public static IReadOnlyList<DataValue> ThenShouldContinueWith(this IReadOnlyList<DataValue> dataValues, params double[] numerics)
        {
            for (int i = 0; i < numerics.Length; i++)
            {
                dataValues[i].ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(numerics[i]);
            }

            return dataValues.Skip(numerics.Length).ToList();
        }

        public static IReadOnlyList<DataValue> ThenShouldContinueWith(this IReadOnlyList<DataValue> dataValues, params string[] strings)
        {
            for (int i = 0; i < strings.Length; i++)
            {
                dataValues[i].ShouldBeAssignableTo<StringValue>().Value.ShouldBe(strings[i]);
            }

            return dataValues.Skip(strings.Length).ToList();
        }
    }
}
