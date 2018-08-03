using Xunit;
using Shouldly;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Common;
using Moq;
using System;

namespace Pangolin.Core.Test
{
    public class TokeniserTests
    {
        [Fact]
        public void Tokeniser_should_return_empty_queue_when_empty_code_string_provided()
        {
            // Arrange
            var code = "";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);
            
            // Assert
            result.Count.ShouldBe(0);
        }

        [Fact]
        public void Tokeniser_should_return_ignore_characters_not_on_code_page()
        {
            // Arrange
            var code = "\u263A"; // Smiley, definitly not going to be used!
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            //Assert
            result.Count.ShouldBe(0);
        }

        [Fact]
        public void Tokeniser_should_parse_backslash_as_single_character_string()
        {
            // Arrange
            var code = "\\a";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token1 = result[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("a");
        }

        [Fact]
        public void Tokeniser_should_throw_exception_when_backslash_at_end_of_code()
        {
            // Arrange
            var code = "\\";
            var mockLog = new Mock<System.Action<string>>();

            // Act/Assert
            var exception = Should.Throw(() => { Tokeniser.Tokenise(code, mockLog.Object); }, typeof(PangolinInvalidTokenException));
            exception.Message.ShouldBe("\\ token encountered at end of string");
        }

        [Fact]
        public void Tokeniser_should_parse_string_terminated_with_single_quotes()
        {
            // Arrange
            var code = "'abc'";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token1 = result[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("abc");
        }

        [Fact]
        public void Tokeniser_should_parse_string_terminated_with_double_quotes()
        {
            // Arrange
            var code = "\"abc\"";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token1 = result[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("abc");
        }

        [Fact]
        public void Tokeniser_should_parse_open_ended_string()
        {
            // Arrange
            var code = "'abc";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token1 = result[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("abc");
        }

        [Fact]
        public void Tokeniser_should_parse_string_with_escaped_terminator()
        {
            // Arrange
            var code = "'ab\\'c'";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token1 = result[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("ab'c");
        }

        [Fact]
        public void Tokeniser_should_parse_string_terminated_with_single_quotes_with_double_quote_inside()
        {
            // Arrange
            var code = "'ab\"c'";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token1 = result[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("ab\"c");
        }

        [Fact]
        public void Tokeniser_should_parse_string_terminated_with_double_quotes_with_single_quote_inside()
        {
            // Arrange
            var code = "\"ab'c\"";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token1 = result[0].ShouldBeOfType<TokenImplementations.StringLiteral>();
            token1.LiteralValue.ShouldBe("ab'c");
        }

        [Fact]
        public void Tokeniser_should_parse_0()
        {
            // Arrange
            var code = "0";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token1 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token1.LiteralValue.ShouldBe(0);
        }

        [Fact]
        public void Tokeniser_should_parse_consecutive_0s_as_separate_tokens()
        {
            // Arrange
            var code = "00";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(2);

            var token1 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token1.LiteralValue.ShouldBe(0);

            var token2 = result[1].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token2.LiteralValue.ShouldBe(0);
        }

        [Fact]
        public void Tokeniser_should_parse_subscript_numbers_as_single_digit_numerics()
        {
            // Arrange
            var code = "\u2080\u2081\u2082\u2083\u2084\u2085\u2086\u2087\u2088\u2089";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(10);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(0);

            var token1 = result[1].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token1.LiteralValue.ShouldBe(1);

            var token2 = result[2].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token2.LiteralValue.ShouldBe(2);

            var token3 = result[3].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token3.LiteralValue.ShouldBe(3);

            var token4 = result[4].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token4.LiteralValue.ShouldBe(4);

            var token5 = result[5].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token5.LiteralValue.ShouldBe(5);

            var token6 = result[6].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token6.LiteralValue.ShouldBe(6);

            var token7 = result[7].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token7.LiteralValue.ShouldBe(7);

            var token8 = result[8].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token8.LiteralValue.ShouldBe(8);

            var token9 = result[9].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token9.LiteralValue.ShouldBe(9);
        }

        [Fact]
        public void Tokeniser_should_parse_integer()
        {
            // Arrange
            var code = "123";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(123);
        }

        [Fact]
        public void Tokeniser_should_parse_integer_with_0()
        {
            // Arrange
            var code = "120";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(120);
        }

        [Fact]
        public void Tokeniser_should_parse_0_followed_by_integer_as_separate_tokens()
        {
            // Arrange
            var code = "0123";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(2);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(0);

            var token1 = result[1].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token1.LiteralValue.ShouldBe(123);
        }

        [Fact]
        public void Tokeniser_should_parse_float()
        {
            // Arrange
            var code = "1.5";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(1.5);
        }

        [Fact]
        public void Tokeniser_should_parse_float_starting_with_decimal_point()
        {
            // Arrange
            var code = ".5";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(0.5);
        }

        [Fact]
        public void Tokeniser_should_parse_int_scientific_notation()
        {
            // Arrange
            var code = "10\u23E83";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(10000);
        }

        [Fact]
        public void Tokeniser_should_parse_float_scientific_notation()
        {
            // Arrange
            var code = "1.5\u23E83";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(1500);
        }

        [Fact]
        public void Tokeniser_should_parse_float_scientific_notation_with_leading_decimal_point()
        {
            // Arrange
            var code = ".5\u23E83";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(500);
        }

        [Fact]
        public void Tokeniser_should_parse_int_scientific_notation_negative_exponent()
        {
            // Arrange
            var code = "10\u23E8-3";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(0.01);
        }

        [Fact]
        public void Tokeniser_should_parse_float_scientific_notation_negative_exponent()
        {
            // Arrange
            var code = "1.5\u23E8-3";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(0.0015);
        }

        [Fact]
        public void Tokeniser_should_parse_float_scientific_notation_with_leading_decimal_point_negative_exponent()
        {
            // Arrange
            var code = ".5\u23E8-3";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(0.0005);
        }

        [Fact]
        public void Tokeniser_should_parse_scientific_notation_inferring_leading_1()
        {
            // Arrange
            var code = "\u23E83";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(1000);
        }

        [Fact]
        public void Tokeniser_should_parse_scientific_notation_inferring_trailing_1()
        {
            // Arrange
            var code = "5\u23E8";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(50);
        }

        [Fact]
        public void Tokeniser_should_parse_scientific_notation_inferring_leading_and_trailing_1()
        {
            // Arrange
            var code = "\u23E8";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);

            var token0 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token0.LiteralValue.ShouldBe(10);
        }

        [Fact]
        public void Tokeniser_should_parse_truthify()
        {
            '\u00A1'.ShouldBeParsedAs(typeof(TokenImplementations.Truthify));
        }

        [Fact]
        public void Tokeniser_should_parse_untruthify()
        {
            '!'.ShouldBeParsedAs(typeof(TokenImplementations.UnTruthify));
        }

        [Fact]
        public void Tokeniser_should_parse_single_arguments()
        {
            // Arrange
            var code = "\u24EA\u2460\u2461";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var results = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            var token1 = results[0].ShouldBeOfType<TokenImplementations.SingleArgument>();
            token1.ArgumentIndex.ShouldBe(0);

            var token2 = results[1].ShouldBeOfType<TokenImplementations.SingleArgument>();
            token2.ArgumentIndex.ShouldBe(1);

            var token3 = results[2].ShouldBeOfType<TokenImplementations.SingleArgument>();
            token3.ArgumentIndex.ShouldBe(2);
        }

        [Fact]
        public void Tokeniser_should_parse_argument_array()
        {
            '\u00A5'.ShouldBeParsedAs(typeof(TokenImplementations.ArgumentArray));
        }

        [Fact]
        public void Tokeniser_should_parse_add()
        {
            '+'.ShouldBeParsedAs(typeof(TokenImplementations.Add));
        }

        [Fact]
        public void Tokeniser_should_step_over_spaces()
        {
            // Arrange
            var code = "1 2";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(2);

            var token1 = result[0].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token1.LiteralValue.ShouldBe(1);

            var token2 = result[1].ShouldBeOfType<TokenImplementations.NumericLiteral>();
            token2.LiteralValue.ShouldBe(2);
        }

        [Fact]
        public void Tokeniser_should_parse_range()
        {
            '\u2192'.ShouldBeParsedAs(typeof(TokenImplementations.Range));
        }

        [Fact]
        public void Tokeniser_should_parse_ReverseRange()
        {
            '\u2190'.ShouldBeParsedAs(typeof(TokenImplementations.ReverseRange));
        }

        [Fact]
        public void Tokeniser_should_parse_range1()
        {
            '\u0411'.ShouldBeParsedAs(typeof(TokenImplementations.Range1));
        }

        [Fact]
        public void Tokeniser_should_parse_ReverseRange1()
        {
            '\u042A'.ShouldBeParsedAs(typeof(TokenImplementations.ReverseRange1));
        }

        [Fact]
        public void Tokeniser_should_parse_GetVariable()
        {
            // Arrange
            var code = "\u2825\u2845\u28A1\u2885\u2861\u28E1\u28C5\u28A5\u2865\u28E5";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(10);

            result[0].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(0);
            result[1].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(1);
            result[2].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(2);
            result[3].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(3);
            result[4].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(4);
            result[5].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(5);
            result[6].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(6);
            result[7].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(7);
            result[8].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(8);
            result[9].ShouldBeOfType<TokenImplementations.GetVariable>().VariableIndex.ShouldBe(9);
        }

        [Fact]
        public void Tokeniser_should_parse_SetVariable()
        {
            // Arrange
            var code = "\u2849\u2843\u2858\u2851\u284A\u285A\u2853\u2859\u284B\u285B";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(10);

            result[0].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(0);
            result[1].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(1);
            result[2].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(2);
            result[3].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(3);
            result[4].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(4);
            result[5].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(5);
            result[6].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(6);
            result[7].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(7);
            result[8].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(8);
            result[9].ShouldBeOfType<TokenImplementations.SetVariable>().VariableIndex.ShouldBe(9);
        }

        [Fact]
        public void Tokeniser_should_parse_multiply()
        {
            '*'.ShouldBeParsedAs(typeof(TokenImplementations.Multiply));
        }

        [Fact]
        public void Tokeniser_should_parse_equality()
        {
            // Arrange
            var code = "=";
            var mockLog = new Mock<System.Action<string>>();

            // Act
            var result = Tokeniser.Tokenise(code, mockLog.Object);

            // Assert
            result.Count.ShouldBe(1);
            result[0].ShouldBeOfType<TokenImplementations.Equality>();
        }

        [Fact]
        public void Tokeniser_should_parse_inequality()
        {
            '\u2260'.ShouldBeParsedAs(typeof(TokenImplementations.Inequality));
        }

        [Fact]
        public void Tokeniser_should_parse_Select()
        {
            'S'.ShouldBeParsedAs(typeof(TokenImplementations.Select));
        }

        [Fact]
        public void Tokeniser_should_parse_SelectIterationVariable0()
        {
            's'.ShouldBeParsedAs(typeof(TokenImplementations.SelectIterationVariable0));
        }

        [Fact]
        public void Tokeniser_should_parse_SelectIterationVariable1()
        {
            't'.ShouldBeParsedAs(typeof(TokenImplementations.SelectIterationVariable1));
        }

        [Fact]
        public void Tokeniser_should_parse_SelectIterationVariable2()
        {
            'u'.ShouldBeParsedAs(typeof(TokenImplementations.SelectIterationVariable2));
        }

        [Fact]
        public void Tokeniser_should_parse_SelectIterationVariable3()
        {
            'v'.ShouldBeParsedAs(typeof(TokenImplementations.SelectIterationVariable3));
        }

        [Fact]
        public void Tokeniser_should_parse_SelectIterationIndex0()
        {
            'i'.ShouldBeParsedAs(typeof(TokenImplementations.SelectIterationIndex0));
        }

        [Fact]
        public void Tokeniser_should_parse_SelectIterationIndex1()
        {
            'j'.ShouldBeParsedAs(typeof(TokenImplementations.SelectIterationIndex1));
        }

        [Fact]
        public void Tokeniser_should_parse_SelectIterationIndex2()
        {
            'k'.ShouldBeParsedAs(typeof(TokenImplementations.SelectIterationIndex2));
        }

        [Fact]
        public void Tokeniser_should_parse_SelectIterationIndex3()
        {
            'l'.ShouldBeParsedAs(typeof(TokenImplementations.SelectIterationIndex3));
        }

        [Fact]
        public void Tokeniser_should_parse_Where()
        {
            'W'.ShouldBeParsedAs(typeof(TokenImplementations.Where));
        }

        [Fact]
        public void Tokeniser_should_parse_WhereIterationVariable0()
        {
            'w'.ShouldBeParsedAs(typeof(TokenImplementations.WhereIterationVariable0));
        }

        [Fact]
        public void Tokeniser_should_parse_WhereIterationVariable1()
        {
            'x'.ShouldBeParsedAs(typeof(TokenImplementations.WhereIterationVariable1));
        }

        [Fact]
        public void Tokeniser_should_parse_WhereIterationIndex0()
        {
            'g'.ShouldBeParsedAs(typeof(TokenImplementations.WhereIterationIndex0));
        }

        [Fact]
        public void Tokeniser_should_parse_WhereIterationIndex1()
        {
            'h'.ShouldBeParsedAs(typeof(TokenImplementations.WhereIterationIndex1));
        }

        [Fact]
        public void Tokeniser_should_parse_LogicAnd()
        {
            '&'.ShouldBeParsedAs(typeof(TokenImplementations.LogicAnd));
        }

        [Fact]
        public void Tokeniser_should_parse_LogicOr()
        {
            '|'.ShouldBeParsedAs(typeof(TokenImplementations.LogicOr));
        }

        [Fact]
        public void Tokeniser_should_parse_LogicXor()
        {
            'X'.ShouldBeParsedAs(typeof(TokenImplementations.LogicXor));
        }

        [Fact]
        public void Tokeniser_should_parse_LogicXnor()
        {
            '~'.ShouldBeParsedAs(typeof(TokenImplementations.LogicXnor));
        }

        [Fact]
        public void Tokeniser_should_parse_Length()
        {
            'L'.ShouldBeParsedAs(typeof(TokenImplementations.Length));
        }

        [Fact]
        public void Tokeniser_should_parse_Modulo()
        {
            '%'.ShouldBeParsedAs(typeof(TokenImplementations.Modulo));
        }

        [Fact]
        public void Tokeniser_should_parse_Division()
        {
            '/'.ShouldBeParsedAs(typeof(TokenImplementations.Division));
        }

        [Fact]
        public void Tokeniser_should_parse_Arrayify()
        {
            'A'.ShouldBeParsedAs(typeof(TokenImplementations.Arrayify));
        }

        [Fact]
        public void Tokeniser_should_parse_ConstantNewline()
        {
            '\u00B6'.ShouldBeParsedAs(typeof(TokenImplementations.ConstantNewline));
        }

        [Fact]
        public void Tokeniser_should_parse_ConstantEmptyArray()
        {
            'a'.ShouldBeParsedAs(typeof(TokenImplementations.ConstantEmptyArray));
        }

        [Fact]
        public void Tokeniser_should_parse_ConstantEmptyString()
        {
            'e'.ShouldBeParsedAs(typeof(TokenImplementations.ConstantEmptyString));
        }

        [Fact]
        public void Tokeniser_should_parse_ArrayPair()
        {
            ']'.ShouldBeParsedAs(typeof(TokenImplementations.ArrayPair));
        }

        [Fact]
        public void Tokeniser_should_parse_ArrayTriple()
        {
            '\u039E'.ShouldBeParsedAs(typeof(TokenImplementations.ArrayTriple));
        }

        [Fact]
        public void Tokeniser_should_parse_Interpolation()
        {
            '$'.ShouldBeParsedAs(typeof(TokenImplementations.Interpolation));
        }

        [Fact]
        public void Tokeniser_should_parse_GetRandomDecimal()
        {
            'r'.ShouldBeParsedAs(typeof(TokenImplementations.GetRandomDecimal));
        }

        [Fact]
        public void Tokeniser_should_parse_LessThan()
        {
            '<'.ShouldBeParsedAs(typeof(TokenImplementations.LessThan));
        }

        [Fact]
        public void Tokeniser_should_parse_GreaterThan()
        {
            '>'.ShouldBeParsedAs(typeof(TokenImplementations.GreaterThan));
        }

        [Fact]
        public void Tokeniser_should_parse_Iterate()
        {
            'I'.ShouldBeParsedAs(typeof(TokenImplementations.Iterate));
        }

        [Fact]
        public void Tokeniser_should_parse_IteratedAdd()
        {
            '\u2295'.ShouldBeParsedAs(typeof(TokenImplementations.IteratedAdd));
        }

        [Fact]
        public void Tokeniser_should_parse_SquareRoot()
        {
            '\u221A'.ShouldBeParsedAs(typeof(TokenImplementations.SquareRoot));
        }

        [Fact]
        public void Tokeniser_should_parse_Membership()
        {
            '\u2208'.ShouldBeParsedAs(typeof(TokenImplementations.Membership));
        }

        [Fact]
        public void Tokeniser_should_parse_Sum()
        {
            '\u03A3'.ShouldBeParsedAs(typeof(TokenImplementations.Sum));
        }

        [Fact]
        public void Tokeniser_should_parse_Elements()
        {
            '\u03B4'.ShouldBeParsedAs(typeof(TokenImplementations.Elements));
        }

        [Fact]
        public void Tokeniser_should_parse_IteratedEquality()
        {
            '\u229C'.ShouldBeParsedAs(typeof(TokenImplementations.IteratedEquality));
        }

        [Fact]
        public void Tokeniser_should_parse_ConstantLowercaseAlphabet()
        {
            '\u1EA0'.ShouldBeParsedAs(typeof(TokenImplementations.ConstantLowercaseAlphabet));
        }

        [Fact]
        public void Tokeniser_should_parse_ConstantUppercaseAlphabet()
        {
            '\u1E05'.ShouldBeParsedAs(typeof(TokenImplementations.ConstantUppercaseAlphabet));
        }

        [Fact]
        public void Tokeniser_should_parse_Double()
        {
            'D'.ShouldBeParsedAs(typeof(TokenImplementations.Double));
        }

        [Fact]
        public void Tokeniser_should_parse_Half()
        {
            'H'.ShouldBeParsedAs(typeof(TokenImplementations.Half));
        }

        [Fact]
        public void Tokeniser_should_parse_IsEven()
        {
            '\u1EB8'.ShouldBeParsedAs(typeof(TokenImplementations.IsEven));
        }

        [Fact]
        public void Tokeniser_should_parse_IsOdd()
        {
            '\u1ECC'.ShouldBeParsedAs(typeof(TokenImplementations.IsOdd));
        }

        [Fact]
        public void Tokeniser_should_parse_Reverse()
        {
            '\u042F'.ShouldBeParsedAs(typeof(TokenImplementations.Reverse));
        }        
    }
}

