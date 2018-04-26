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
        public static Queue<Token> Tokenise(string code, IReadOnlyList<DataValue> arguments)
        {
            var result = new Queue<Token>();

            // Iterate over characters until all processed
            var index = 0;
            while (index < code.Length)
            {
                var current = code[index];

                // Single char string
                if (current == '\\')
                {
                    // If at end of string, error
                    index++;
                    if (index == code.Length) throw new PangolinInvalidTokenException("\\ token encountered at end of string");

                    result.Enqueue(Token.Get.StringLiteral(code[index].ToString()));
                }
                // Arbitrary length string
                else if(current == '"' || current == '\'')
                {
                    // Check if at end of string
                    index++;
                    if (index == code.Length)
                    {
                        result.Enqueue(Token.Get.StringLiteral(""));
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

                        result.Enqueue(Token.Get.StringLiteral(sb.ToString()));
                    }                    
                }
                // Leading 0
                else if (current == '0')
                {
                    result.Enqueue(Token.Get.NumericLiteral(0));
                }
                // Other single digit 
                else if ("\u2080\u2081\u2082\u2083\u2084\u2085\u2086\u2087\u2088\u2089".Contains(current))
                {
                    result.Enqueue(Token.Get.NumericLiteral((int)current - 0x2080));
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

                    result.Enqueue(Token.Get.NumericLiteral(decimal.Parse(fettledString, System.Globalization.NumberStyles.Float)));

                    // Move on appropriate number of characters
                    index += readString.Length - 1;
                }
                // Truthify
                else if (current == '\u00A1')
                {
                    result.Enqueue(Token.Get.Truthify());
                }
                // Untruthify
                else if (current == '\u00AC')
                {
                    result.Enqueue(Token.Get.UnTruthify());
                }
                // Single argument
                else if ("\uDFD8\uDFD9\uDFDA\uDFDB\uDFDC\uDFDD\uDFDE\uDFDF\uDFE0\uDFE1".Contains(current))
                {
                    var argumentIndex = current - 0xDFD8;
                    result.Enqueue(Token.Get.SingleArgument(arguments, argumentIndex));
                }
                // Argument array
                else if (current == '\u00AE')
                {
                    result.Enqueue(Token.Get.ArgumentArray(arguments));
                }
                // Add
                else if (current == '+')
                {
                    result.Enqueue(Token.Get.Add());
                }
                // Space
                else if (current == ' ')
                {
                    // Add nothing, move on
                    // TODO: Consider adding flag to indicate that token has space preceding it?
                }
                else
                {
                    throw new PangolinInvalidTokenException($"Unrecognised character in code: {current}");
                }

                // Move on to next character
                index++;
            }

            return result;
        }
    }
}
