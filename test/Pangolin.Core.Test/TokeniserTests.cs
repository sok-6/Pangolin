using Xunit;
using Shouldly;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Common;

namespace Pangolin.Core.Test
{
    public class TokeniserTests
    {
        [Fact]
        public void Tokeniser_should_return_empty_queue_when_empty_code_string_provided()
        {
            // Arrange
            var code = "";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(0);
        }

        [Fact]
        public void Tokeniser_should_throw_excpetion_when_invalid_character_in_code()
        {
            // Arrange
            var code = "\u263A"; // Smiley, definitly not going to be used!

            // Act/Assert
            var exception = Should.Throw(() => { Tokeniser.Tokenise(code, null); }, typeof(PangolinInvalidTokenException));
            exception.Message.ShouldBe($"Unrecognised character in code: {code}");
        }

        [Fact]
        public void Tokeniser_should_parse_backslash_as_single_character_string()
        {
            // Arrange
            var code = "\\a";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token1 = result.TokenList[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("a");
        }

        [Fact]
        public void Tokeniser_should_throw_exception_when_backslash_at_end_of_code()
        {
            // Arrange
            var code = "\\";

            // Act/Assert
            var exception = Should.Throw(() => { Tokeniser.Tokenise(code, null); }, typeof(PangolinInvalidTokenException));
            exception.Message.ShouldBe("\\ token encountered at end of string");
        }

        [Fact]
        public void Tokeniser_should_parse_string_terminated_with_single_quotes()
        {
            // Arrange
            var code = "'abc'";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token1 = result.TokenList[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("abc");
        }

        [Fact]
        public void Tokeniser_should_parse_string_terminated_with_double_quotes()
        {
            // Arrange
            var code = "\"abc\"";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token1 = result.TokenList[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("abc");
        }

        [Fact]
        public void Tokeniser_should_parse_open_ended_string()
        {
            // Arrange
            var code = "'abc";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token1 = result.TokenList[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("abc");
        }

        [Fact]
        public void Tokeniser_should_parse_string_with_escaped_terminator()
        {
            // Arrange
            var code = "'ab\\'c'";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token1 = result.TokenList[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("ab'c");
        }

        [Fact]
        public void Tokeniser_should_parse_string_terminated_with_single_quotes_with_double_quote_inside()
        {
            // Arrange
            var code = "'ab\"c'";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token1 = result.TokenList[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("ab\"c");
        }

        [Fact]
        public void Tokeniser_should_parse_string_terminated_with_double_quotes_with_single_quote_inside()
        {
            // Arrange
            var code = "\"ab'c\"";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token1 = result.TokenList[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("ab'c");
        }

        [Fact]
        public void Tokeniser_should_parse_0()
        {
            // Arrange
            var code = "0";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token1 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token1.LiteralValue.ShouldBe(0);
        }

        [Fact]
        public void Tokeniser_should_parse_consecutive_0s_as_separate_tokens()
        {
            // Arrange
            var code = "00";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(2);

            var token1 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token1.LiteralValue.ShouldBe(0);

            var token2 = result.TokenList[1].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token2.LiteralValue.ShouldBe(0);
        }

        [Fact]
        public void Tokeniser_should_parse_subscript_numbers_as_single_digit_numerics()
        {
            // Arrange
            var code = "\u2080\u2081\u2082\u2083\u2084\u2085\u2086\u2087\u2088\u2089";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(10);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(0);

            var token1 = result.TokenList[1].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token1.LiteralValue.ShouldBe(1);

            var token2 = result.TokenList[2].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token2.LiteralValue.ShouldBe(2);

            var token3 = result.TokenList[3].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token3.LiteralValue.ShouldBe(3);

            var token4 = result.TokenList[4].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token4.LiteralValue.ShouldBe(4);

            var token5 = result.TokenList[5].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token5.LiteralValue.ShouldBe(5);

            var token6 = result.TokenList[6].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token6.LiteralValue.ShouldBe(6);

            var token7 = result.TokenList[7].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token7.LiteralValue.ShouldBe(7);

            var token8 = result.TokenList[8].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token8.LiteralValue.ShouldBe(8);

            var token9 = result.TokenList[9].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token9.LiteralValue.ShouldBe(9);
        }

        [Fact]
        public void Tokeniser_should_parse_integer()
        {
            // Arrange
            var code = "123";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(123);
        }

        [Fact]
        public void Tokeniser_should_parse_integer_with_0()
        {
            // Arrange
            var code = "120";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(120);
        }

        [Fact]
        public void Tokeniser_should_parse_0_followed_by_integer_as_separate_tokens()
        {
            // Arrange
            var code = "0123";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(2);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(0);

            var token1 = result.TokenList[1].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token1.LiteralValue.ShouldBe(123);
        }

        [Fact]
        public void Tokeniser_should_parse_float()
        {
            // Arrange
            var code = "1.5";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(1.5m);
        }

        [Fact]
        public void Tokeniser_should_parse_float_starting_with_decimal_point()
        {
            // Arrange
            var code = ".5";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(0.5m);
        }

        [Fact]
        public void Tokeniser_should_parse_int_scientific_notation()
        {
            // Arrange
            var code = "10\u23E83";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(10000);
        }

        [Fact]
        public void Tokeniser_should_parse_float_scientific_notation()
        {
            // Arrange
            var code = "1.5\u23E83";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(1500);
        }

        [Fact]
        public void Tokeniser_should_parse_float_scientific_notation_with_leading_decimal_point()
        {
            // Arrange
            var code = ".5\u23E83";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(500);
        }

        [Fact]
        public void Tokeniser_should_parse_int_scientific_notation_negative_exponent()
        {
            // Arrange
            var code = "10\u23E8-3";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(0.01m);
        }

        [Fact]
        public void Tokeniser_should_parse_float_scientific_notation_negative_exponent()
        {
            // Arrange
            var code = "1.5\u23E8-3";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(0.0015m);
        }

        [Fact]
        public void Tokeniser_should_parse_float_scientific_notation_with_leading_decimal_point_negative_exponent()
        {
            // Arrange
            var code = ".5\u23E8-3";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(0.0005m);
        }

        [Fact]
        public void Tokeniser_should_parse_scientific_notation_inferring_leading_1()
        {
            // Arrange
            var code = "\u23E83";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(1000);
        }

        [Fact]
        public void Tokeniser_should_parse_scientific_notation_inferring_trailing_1()
        {
            // Arrange
            var code = "5\u23E8";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(50);
        }

        [Fact]
        public void Tokeniser_should_parse_scientific_notation_inferring_leading_and_trailing_1()
        {
            // Arrange
            var code = "\u23E8";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);

            var token0 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(10);
        }

        [Fact]
        public void Tokeniser_should_parse_truthify()
        {
            // Arrange
            var code = "\u00A1";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);
            result.TokenList[0].ShouldBeOfType(typeof(TokenImplementations.Truthify));
        }

        [Fact]
        public void Tokeniser_should_parse_untruthify()
        {
            // Arrange
            var code = "\u00AC";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);
            result.TokenList[0].ShouldBeOfType(typeof(TokenImplementations.UnTruthify));
        }

        [Fact]
        public void Tokeniser_should_parse_single_arguments()
        {
            // Arrange
            var code = "\uDFD8\uDFD9\uDFDA";
            var arguments = new DataValue[]
            {
                new NumericValue(1),
                new NumericValue(2),
                new NumericValue(3)
            };

            // Act
            var results = Tokeniser.Tokenise(code, arguments);

            // Assert
            var token1 = results.TokenList[0].ShouldBeOfType<TokenImplementations.SingleArgument>();
            var value1 = token1.Value.ShouldBeOfType<NumericValue>();
            value1.Value.ShouldBe(1);

            var token2 = results.TokenList[1].ShouldBeOfType<TokenImplementations.SingleArgument>();
            var value2 = token2.Value.ShouldBeOfType<NumericValue>();
            value2.Value.ShouldBe(2);

            var token3 = results.TokenList[2].ShouldBeOfType<TokenImplementations.SingleArgument>();
            var value3 = token3.Value.ShouldBeOfType<NumericValue>();
            value3.Value.ShouldBe(3);
        }

        [Fact]
        public void Tokeniser_should_parse_argument_array()
        {
            // Arrange
            var code = "\u00AE";
            var arguments = new DataValue[]
            {
                new NumericValue(1),
                new StringValue("abc")
            };

            // Act
            var results = Tokeniser.Tokenise(code, arguments);

            // Assert
            results.TokenList.Count.ShouldBe(1);
            var token = results.TokenList[0].ShouldBeOfType<TokenImplementations.ArgumentArray>();
            var value1 = token.ArrayValue.Value[0].ShouldBeOfType<NumericValue>();
            value1.Value.ShouldBe(1);
            var value2 = token.ArrayValue.Value[1].ShouldBeOfType<StringValue>();
            value2.Value.ShouldBe("abc");
        }

        [Fact]
        public void Tokeniser_should_parse_add()
        {
            // Arrange
            var code = "+";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);
            result.TokenList[0].ShouldBeOfType(typeof(TokenImplementations.Add));
        }

        [Fact]
        public void Tokeniser_should_step_over_spaces()
        {
            // Arrange
            var code = "1 2";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(2);

            var token1 = result.TokenList[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token1.LiteralValue.ShouldBe(1);

            var token2 = result.TokenList[1].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token2.LiteralValue.ShouldBe(2);
        }

        [Fact]
        public void Tokeniser_should_parse_range()
        {
            // Arrange
            var code = "\u2192";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);
            result.TokenList[0].ShouldBeOfType(typeof(TokenImplementations.Range));
        }

        [Fact]
        public void Tokeniser_should_parse_ReverseRange()
        {
            // Arrange
            var code = "\u2190";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);
            result.TokenList[0].ShouldBeOfType(typeof(TokenImplementations.ReverseRange));
        }

        [Fact]
        public void Tokeniser_should_parse_range1()
        {
            // Arrange
            var code = "\u0411";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);
            result.TokenList[0].ShouldBeOfType(typeof(TokenImplementations.Range1));
        }

        [Fact]
        public void Tokeniser_should_parse_ReverseRange1()
        {
            // Arrange
            var code = "\u042A";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);
            result.TokenList[0].ShouldBeOfType(typeof(TokenImplementations.ReverseRange1));
        }
    }
}

