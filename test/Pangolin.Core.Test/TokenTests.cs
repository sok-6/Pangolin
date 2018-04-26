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
            var tokenQueue = new TokenQueue(NumericValue.Zero);

            // Act
            var result = token.Evaluate(tokenQueue);

            // Assert
            result.ShouldBeOfType(typeof(NumericValue));
            result.ToString().ShouldBe("1.5");
        }

        [Fact]
        public void StringLiteral_should_evaluate_to_StringValue_with_correct_contents()
        {
            // Arrange
            var token = Token.Get.StringLiteral("abc");
            var tokenQueue = new TokenQueue(NumericValue.Zero);

            // Act
            var result = token.Evaluate(tokenQueue);

            // Assert
            result.ShouldBeOfType(typeof(StringValue));
            result.ToString().ShouldBe("abc");
        }

        [Fact]
        public void Truthify_should_evaluate_0_as_falsey()
        {
            // Arrange
            var token = Token.Get.Truthify();
            var tokenQueue = new TokenQueue(NumericValue.Zero,
                Token.Get.NumericLiteral(0));

            // Act
            var result = token.Evaluate(tokenQueue);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(0);
        }

        [Fact]
        public void Truthify_should_evaluate_non_0_as_truthy()
        {
            // Arrange
            var token = Token.Get.Truthify();
            var tokenQueue = new TokenQueue(NumericValue.Zero,
                Token.Get.NumericLiteral(5),
                Token.Get.NumericLiteral(-5),
                Token.Get.NumericLiteral(0.0000000001m),
                Token.Get.NumericLiteral(decimal.MaxValue),
                Token.Get.NumericLiteral(decimal.MinValue));

            // Act
            var result1 = token.Evaluate(tokenQueue);
            var result2 = token.Evaluate(tokenQueue);
            var result3 = token.Evaluate(tokenQueue);
            var result4 = token.Evaluate(tokenQueue);
            var result5 = token.Evaluate(tokenQueue);

            // Assert
            var result_5 = result1.ShouldBeOfType<NumericValue>();
            result_5.Value.ShouldBe(1);

            var result_minus5 = result1.ShouldBeOfType<NumericValue>();
            result_minus5.Value.ShouldBe(1);

            var result_nearly0 = result1.ShouldBeOfType<NumericValue>();
            result_nearly0.Value.ShouldBe(1);

            var result_max = result1.ShouldBeOfType<NumericValue>();
            result_max.Value.ShouldBe(1);

            var result_min = result1.ShouldBeOfType<NumericValue>();
            result_min.Value.ShouldBe(1);
        }

        [Fact]
        public void Truthify_should_evaluate_empty_string_as_falsey()
        {
            // Arrange
            var token = Token.Get.Truthify();
            var tokenQueue = new TokenQueue(NumericValue.Zero,
                Token.Get.StringLiteral(""));

            // Act
            var result = token.Evaluate(tokenQueue);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(0);
        }

        [Fact]
        public void Truthify_should_evaluate_populated_string_as_truthy()
        {
            // Arrange
            var token = Token.Get.Truthify();
            var tokenQueue = new TokenQueue(NumericValue.Zero,
                Token.Get.StringLiteral("abc"));

            // Act
            var result = token.Evaluate(tokenQueue);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(1);
        }

        [Fact(Skip = "Array literal not implemented yet")]
        public void Truthify_should_evaluate_empty_array_as_falsey()
        {

        }

        [Fact(Skip = "Array literal not implemented yet")]
        public void Truthify_should_evaluate_populated_array_as_truthy()
        {

        }

        [Fact]
        public void UnTruthify_should_evaluate_0_as_truthy()
        {
            // Arrange
            var token = Token.Get.UnTruthify();
            var tokenQueue = new TokenQueue(NumericValue.Zero,
                Token.Get.NumericLiteral(0));

            // Act
            var result = token.Evaluate(tokenQueue);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(1);
        }

        [Fact]
        public void UnTruthify_should_evaluate_non_0_as_falsey()
        {
            // Arrange
            var token = Token.Get.UnTruthify();
            var tokenQueue = new TokenQueue(NumericValue.Zero,
                Token.Get.NumericLiteral(5),
                Token.Get.NumericLiteral(-5),
                Token.Get.NumericLiteral(0.0000000001m),
                Token.Get.NumericLiteral(decimal.MaxValue),
                Token.Get.NumericLiteral(decimal.MinValue));

            // Act
            var result1 = token.Evaluate(tokenQueue);
            var result2 = token.Evaluate(tokenQueue);
            var result3 = token.Evaluate(tokenQueue);
            var result4 = token.Evaluate(tokenQueue);
            var result5 = token.Evaluate(tokenQueue);

            // Assert
            var result_5 = result1.ShouldBeOfType<NumericValue>();
            result_5.Value.ShouldBe(0);

            var result_minus5 = result1.ShouldBeOfType<NumericValue>();
            result_minus5.Value.ShouldBe(0);

            var result_nearly0 = result1.ShouldBeOfType<NumericValue>();
            result_nearly0.Value.ShouldBe(0);

            var result_max = result1.ShouldBeOfType<NumericValue>();
            result_max.Value.ShouldBe(0);

            var result_min = result1.ShouldBeOfType<NumericValue>();
            result_min.Value.ShouldBe(0);
        }

        [Fact]
        public void UnTruthify_should_evaluate_empty_string_as_truthy()
        {
            // Arrange
            var token = Token.Get.UnTruthify();
            var tokenQueue = new TokenQueue(NumericValue.Zero,
                Token.Get.StringLiteral(""));

            // Act
            var result = token.Evaluate(tokenQueue);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(1);
        }

        [Fact]
        public void UnTruthify_should_evaluate_populated_string_as_falsey()
        {
            // Arrange
            var token = Token.Get.UnTruthify();
            var tokenQueue = new TokenQueue(NumericValue.Zero,
                Token.Get.StringLiteral("abc"));

            // Act
            var result = token.Evaluate(tokenQueue);

            // Assert
            var numericResult = result.ShouldBeOfType<NumericValue>();
            numericResult.Value.ShouldBe(0);
        }

        [Fact(Skip = "Array literal not implemented yet")]
        public void UnTruthify_should_evaluate_empty_array_as_truthy()
        {

        }

        [Fact(Skip = "Array literal not implemented yet")]
        public void UnTruthify_should_evaluate_populated_array_as_falsey()
        {

        }

        [Fact]
        public void SingleArgument_should_default_to_zero_if_unindexed()
        {
            // Arrange
            var tokenQueue = new TokenQueue(NumericValue.Zero);
            var arguments = new DataValue[]
            {

            };
            var token = Token.Get.SingleArgument(arguments, 1);

            // Act
            var result = token.Evaluate(tokenQueue);

            // Assert
            var num = result.ShouldBeOfType<NumericValue>();
            num.Value.ShouldBe(0);
        }

        [Fact]
        public void SingleArgument_should_retrieve_correct_argument()
        {
            // Arrange
            var tokenQueue = new TokenQueue(NumericValue.Zero);
            var arguments = new DataValue[]
            {
                new NumericValue(1),
                new NumericValue(2),
                new NumericValue(3)
            };
            var token1 = Token.Get.SingleArgument(arguments, 0);
            var token2 = Token.Get.SingleArgument(arguments, 1);
            var token3 = Token.Get.SingleArgument(arguments, 2);

            // Act
            var result1 = token1.Evaluate(tokenQueue);
            var result2 = token2.Evaluate(tokenQueue);
            var result3 = token3.Evaluate(tokenQueue);

            // Assert
            var num1 = result1.ShouldBeOfType<NumericValue>();
            num1.Value.ShouldBe(1);
            var num2 = result2.ShouldBeOfType<NumericValue>();
            num2.Value.ShouldBe(2);
            var num3 = result3.ShouldBeOfType<NumericValue>();
            num3.Value.ShouldBe(3);
        }

        [Fact]
        public void ArgumentArray_should_evaluate_to_correct_array_value()
        {
            // Arrange
            var tokenQueue = new TokenQueue(NumericValue.Zero);
            var arguments = new DataValue[]
            {
                new NumericValue(1),
                new NumericValue(2),
                new NumericValue(3)
            };
            var token = Token.Get.ArgumentArray(arguments);

            // Act
            var result = token.Evaluate(tokenQueue);

            // Assert
            var value = result.ShouldBeOfType<ArrayValue>();
            value.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            value.Value[1].ShouldBeOfType<NumericValue>().Value.ShouldBe(2);
            value.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(3);
        }

        [Fact]
        public void Add_should_add_two_integers()
        {
            // Arrange
            var addToken = Token.Get.Add();
            var tokenQueue = new TokenQueue(NumericValue.Zero,
                Token.Get.NumericLiteral(1),
                Token.Get.NumericLiteral(2));

            // Act
            var result = addToken.Evaluate(tokenQueue);

            // Assert
            var numResult = result.ShouldBeOfType<NumericValue>();
            numResult.Value.ShouldBe(3);
        }

        [Fact]
        public void Add_should_add_two_floats()
        {
            // Arrange
            var addToken = Token.Get.Add();
            var tokenQueue = new TokenQueue(NumericValue.Zero,
                Token.Get.NumericLiteral(1.5m),
                Token.Get.NumericLiteral(2.3m));

            // Act
            var result = addToken.Evaluate(tokenQueue);

            // Assert
            var numResult = result.ShouldBeOfType<NumericValue>();
            numResult.Value.ShouldBe(3.8m);
        }

        [Fact]
        public void Add_should_concatenate_two_strings()
        {
            // Arrange
            var addToken = Token.Get.Add();
            var tokenQueue = new TokenQueue(NumericValue.Zero,
                Token.Get.StringLiteral("ab"),
                Token.Get.StringLiteral("cd"));

            // Act
            var result = addToken.Evaluate(tokenQueue);

            // Assert
            var stringResult = result.ShouldBeOfType<StringValue>();
            stringResult.Value.ShouldBe("abcd");
        }

        [Fact]
        public void Add_should_concatenate_string_and_numeric()
        {
            // Arrange
            var addToken = Token.Get.Add();
            var tokenQueue1 = new TokenQueue(NumericValue.Zero,
                Token.Get.StringLiteral("ab"),
                Token.Get.NumericLiteral(1));
            var tokenQueue2 = new TokenQueue(NumericValue.Zero,
                Token.Get.NumericLiteral(2),
                Token.Get.StringLiteral("cd"));

            // Act
            var result1 = addToken.Evaluate(tokenQueue1);
            var result2 = addToken.Evaluate(tokenQueue2);

            // Assert
            var stringResult1 = result1.ShouldBeOfType<StringValue>();
            stringResult1.Value.ShouldBe("ab1");

            var stringResult2 = result2.ShouldBeOfType<StringValue>();
            stringResult2.Value.ShouldBe("2cd");
        }

        [Fact(Skip = "Array literal not implemented yet")]
        public void Add_should_concatenate_two_arrays()
        {

        }

        [Fact(Skip = "Array literal not implemented yet")]
        public void Add_should_concatenate_array_and_string()
        {

        }

        [Fact(Skip = "Array literal not implemented yet")]
        public void Add_should_concatenate_array_and_numeric()
        {

        }
    }
}
