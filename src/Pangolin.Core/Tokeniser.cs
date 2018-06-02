using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Pangolin.Common;

namespace Pangolin.Core
{
    public static class Tokeniser { 

        /// <summary>
        /// Converts a program into a queue of tokens to be processed
        /// </summary>
        /// <param name="code">The code to tokenise</param>
        /// <returns>The queue of tokens derived from the code</returns>
        public static IReadOnlyList<Token> Tokenise(string code, Action<string> log)
        {
            var result = new List<Token>();
            
            // Iterate over characters until all processed
            var index = 0;
            while (index < code.Length)
            {
                log($"Remaining tokens = {code.Substring(index)}");

                var current = code[index];
                if (!CodePage.CharacterExistsInCodePage(current))
                {
                    log($"Character {current} U+{Convert.ToString(current, 16)} not found in code page, ignoring");
                }
                else if (current == ' ')
                {
                    // Do nothing, move on
                    // TODO: set flag?

                    log("Space, no token enqueued");
                }
                // Single char string
                else if (current == '\\')
                {
                    log("Single character string constant");

                    // If at end of string, error
                    index++;
                    if (index == code.Length) throw new PangolinInvalidTokenException("\\ token encountered at end of string");

                    result.Add(Token.GetStringLiteral(code[index].ToString()));
                }
                // Arbitrary length string
                else if(current == '"' || current == '\'')
                {
                    log($"Plain string constant, terminator = {current}");

                    // Check if at end of string
                    index++;
                    if (index == code.Length)
                    {
                        log("0 length string");
                        result.Add(Token.GetStringLiteral(""));
                    }
                    else
                    {
                        var terminator = current;
                        var sb = new StringBuilder();
                        while (index < code.Length && code[index] != terminator)
                        {
                            // Check for escaped terminator
                            if (index + 1 < code.Length && code.Substring(index, 2) == $"\\{terminator}")
                            {
                                sb.Append(terminator);
                                index += 2;
                            }
                            else
                            {
                                sb.Append(code[index]);
                                index++;
                            }
                        }

                        log($"Tokenised string = {sb.ToString()}");
                        result.Add(Token.GetStringLiteral(sb.ToString()));
                    }                    
                }
                // Compressed string/numeric
                else if (current == '\u00AB')
                {
                    log("Compressed string/numeric constant");

                    // Check if at end of string
                    index++;
                    if (index == code.Length)
                    {
                        throw new PangolinInvalidTokenException("Empty compressed constant encountered");
                    }
                    else
                    {
                        // Assume compressed string unless 
                        var isString = true;

                        var sb = new StringBuilder();
                        while (index < code.Length)
                        {
                            // Check for string terminator
                            if ('\u00BB' == code[index])
                            {
                                break;
                            }
                            // Numeric terminator
                            else if ('\u00AB' == code[index])
                            {
                                isString = false;
                            }
                            else
                            {
                                sb.Append(code[index]);
                                index++;
                            }
                        }

                        log($"Characters = {sb.ToString()}");

                        if (isString)
                        {
                            result.Add(Token.GetStringLiteral(DecodeCompressedString(sb.ToString(), log)));
                        }
                        else
                        {
                            result.Add(Token.GetNumericLiteral(DecodeCompressedNumeric(sb.ToString(), log)));
                        }
                    }
                }
                // Leading 0
                else if (current == '0')
                {
                    log("Numeric constant 0");
                    result.Add(Token.GetNumericLiteral(0));
                }
                // Other single digit 
                else if ("\u2080\u2081\u2082\u2083\u2084\u2085\u2086\u2087\u2088\u2089".Contains(current))
                {
                    log("Single digit numeric constant");
                    result.Add(Token.GetNumericLiteral((int)current - 0x2080));
                }
                // Other numerics - \u23E8 is subscript 10
                else if ("123456789.\u23E8".Contains(current))
                {
                    // Match a number
                    var readString = Regex.Match(code.Substring(index), @"^((\d+(\.\d+)?|\.\d+)\u23E8?-?\d*|\u23E8-?\d*)").Value;

                    // Replace sub-10 with e, parse
                    var fettledString = readString.Replace('\u23E8', 'e');

                    // If starts with e, prepend 1
                    if (fettledString.StartsWith('e')) fettledString = $"1{fettledString}";

                    // If ends with e, append 1
                    if (fettledString.EndsWith('e')) fettledString = $"{fettledString}1";

                    var literalValue = decimal.Parse(fettledString, System.Globalization.NumberStyles.Float);
                    log($"Plain numeric constant, match = {readString},  value = {literalValue}");
                    result.Add(Token.GetNumericLiteral(literalValue));

                    // Move on appropriate number of characters - last +1 is at end of loop
                    index += readString.Length - 1;
                }
                // Single argument
                else if (TokenImplementations.SingleArgument.CHAR_LIST.Contains(current))
                {
                    result.Add(Token.GetSingleArgument(current));
                }
                // Get variable
                else if (TokenImplementations.GetVariable.CHAR_LIST.Contains(current))
                {
                    result.Add(Token.GetGetVariable(current));
                }
                // Set variable
                else if (TokenImplementations.SetVariable.CHAR_LIST.Contains(current))
                {
                    result.Add(Token.GetSetVariable(current));
                }
                // Other single character token
                else
                {
                    result.Add(Token.Get(current));
                }

                // Move on to next character
                index++;
            }

            return result;
        }

        public static string DecodeCompressedString(string compressedString, Action<string> log)
        {
            throw new NotImplementedException();

            //var sb = new StringBuilder();
            
            //var x = new System.Globalization.StringInfo(compressedString)

            //while (index < compressedString.Length)
            //{
            //    // Convert character to code point
            //    var codePoint = CodePage.GetIndexFromCharacter()

            //    index++;
            //}
        }

        public static decimal DecodeCompressedNumeric(string compressedNumeric, Action<string> log)
        {
            throw new NotImplementedException();
        }
    }
}
