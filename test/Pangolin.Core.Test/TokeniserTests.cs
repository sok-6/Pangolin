using Xunit;
using Shouldly;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Common;
using Moq;

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
                new Mock<DataValue>().Object,
                new Mock<DataValue>().Object,
                new Mock<DataValue>().Object
            };

            // Act
            var results = Tokeniser.Tokenise(code, arguments);

            // Assert
            var token1 = results.TokenList[0].ShouldBeOfType<TokenImplementations.SingleArgument>();
            token1.ArgumentIndex.ShouldBe(0);

            var token2 = results.TokenList[1].ShouldBeOfType<TokenImplementations.SingleArgument>();
            token2.ArgumentIndex.ShouldBe(1);

            var token3 = results.TokenList[2].ShouldBeOfType<TokenImplementations.SingleArgument>();
            token3.ArgumentIndex.ShouldBe(2);
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
            results.TokenList[0].ShouldBeOfType<TokenImplementations.ArgumentArray>();
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

        [Fact]
        public void Tokeniser_should_parse_GetVariable()
        {
            // Arrange
            var code = "\uDD52\uDD53\uDD54\uDD55\uDD56\uDD57\uDD58\uDD59\uDD5A\uDD5B";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(10);

            result.TokenList[0].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(0);
            result.TokenList[1].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(1);
            result.TokenList[2].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(2);
            result.TokenList[3].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(3);
            result.TokenList[4].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(4);
            result.TokenList[5].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(5);
            result.TokenList[6].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(6);
            result.TokenList[7].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(7);
            result.TokenList[8].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(8);
            result.TokenList[9].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(9);
        }

        [Fact]
        public void Tokeniser_should_parse_SetVariable()
        {
            // Arrange
            var code = "\uDD38\uDD39\u2102\uDD3B\uDD3C\uDD3D\uDD3E\u210D\uDD40\uDD41";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(10);

            result.TokenList[0].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(0);
            result.TokenList[1].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(1);
            result.TokenList[2].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(2);
            result.TokenList[3].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(3);
            result.TokenList[4].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(4);
            result.TokenList[5].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(5);
            result.TokenList[6].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(6);
            result.TokenList[7].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(7);
            result.TokenList[8].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(8);
            result.TokenList[9].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(9);
        }

        [Fact]
        public void Tokeniser_should_parse_multiply()
        {
            // Arrange
            var code = "*";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);
            result.TokenList[0].ShouldBeOfType<TokenImplementations.Multiply>();
        }

        [Fact]
        public void Tokeniser_should_parse_equality()
        {
            // Arrange
            var code = "=";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);
            result.TokenList[0].ShouldBeOfType<TokenImplementations.Equality>();
        }

        [Fact]
        public void Tokeniser_should_parse_inequality()
        {
            // Arrange
            var code = "\u2260";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);
            result.TokenList[0].ShouldBeOfType<TokenImplementations.Inequality>();
        }

        [Fact]
        public void Tokeniser_should_parse_Where()
        {
            // Arrange
            var code = "W";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);
            result.TokenList[0].ShouldBeOfType<TokenImplementations.Where>();
        }

        [Fact]
        public void Tokeniser_should_parse_WhereValue()
        {
            // Arrange
            var code = "w";

            // Act
            var result = Tokeniser.Tokenise(code, null);

            // Assert
            result.TokenList.Count.ShouldBe(1);
            result.TokenList[0].ShouldBeOfType<TokenImplementations.WhereValue>();
        }
    }
}

