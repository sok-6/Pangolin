using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Pangolin.Core.DataValueImplementations;

namespace Pangolin.Core.Test
{
    public class DataValueTests
    {
        [Fact]
        public void NumericValue_should_correctly_store_integers()
        {
            // Arrange
            var intValue0 = 0;
            var intValue1 = 1;

            // Act
            var numericValue0 = new NumericValue(intValue0);
            var numericValue1 = new NumericValue(intValue1);

            // Assert
            numericValue0.IntValue.ShouldBe(intValue0);
            numericValue1.IntValue.ShouldBe(intValue1);
        }

        [Fact]
        public void NumericValue_should_correctly_store_floating_point_numbers()
        {
            // Arrange
            var floatValue1 = 1.5m;
            var floatValue2 = -6.7m;

            // Act
            var numericValue1 = new NumericValue(floatValue1);
            var numericValue2 = new NumericValue(floatValue2);

            // Assert
            numericValue1.Value.ShouldBe(floatValue1);
            numericValue1.IntValue.ShouldBe(1);
            numericValue2.Value.ShouldBe(floatValue2);
            numericValue2.IntValue.ShouldBe(-6);
        }

        [Fact]
        public void StringValue_should_correctly_store_unicode_string()
        {
            // Arrange
            var s = "abc\u23E8";

            // Act
            var stringValue = new StringValue(s);

            // Assert
            stringValue.Value.ShouldBe(s);
        }

        [Fact]
        public void StringValue_should_correctly_store_newline_in_string()
        {
            // Arrange
            var s = @"ab
cd";

            // Act
            var stringValue = new StringValue(s);

            // Assert
            stringValue.Value.ShouldBe(s);
        }

        [Fact]
        public void ArrayValue_should_correctly_store_empty_array()
        {
            // Arrange
            var a = new DataValue[0];

            // Act
            var arrayValue = new ArrayValue(a);

            // Assert
            arrayValue.Value.Count.ShouldBe(0);
        }

        [Fact]
        public void ArrayValue_should_correctly_store_populated_array()
        {
            // Arrange
            var a = new DataValue[]
            {
                new NumericValue(1),
                new StringValue("abc")
            };

            // Act
            var arrayValue = new ArrayValue(a);

            // Assert
            arrayValue.Value.Count.ShouldBe(2);

            var a1 = arrayValue.Value[0].ShouldBeOfType<NumericValue>();
            a1.Value.ShouldBe(1);

            var a2 = arrayValue.Value[1].ShouldBeOfType<StringValue>();
            a2.Value.ShouldBe("abc");
        }
    }
}
