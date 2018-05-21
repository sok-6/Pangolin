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
        public static IReadOnlyList<Token> Tokenise(string code)
        {
            var result = new List<Token>();

            //var result = new ProgramState(arguments);

            // Iterate over characters until all processed
            var index = 0;
            while (index < code.Length)
            {
                var current = code[index];

                if (current == ' ')
                {
                    // Do nothing, move on
                    // TODO: set flag?
                }
                // Single char string
                else if (current == '\\')
                {
                    // If at end of string, error
                    index++;
                    if (index == code.Length) throw new PangolinInvalidTokenException("\\ token encountered at end of string");

                    result.Add(Token.GetStringLiteral(code[index].ToString()));
                }
                // Arbitrary length string
                else if(current == '"' || current == '\'')
                {
                    // Check if at end of string
                    index++;
                    if (index == code.Length)
                    {
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

                        result.Add(Token.GetStringLiteral(sb.ToString()));
                    }                    
                }
                // Leading 0
                else if (current == '0')
                {
                    result.Add(Token.GetNumericLiteral(0));
                }
                // Other single digit 
                else if ("\u2080\u2081\u2082\u2083\u2084\u2085\u2086\u2087\u2088\u2089".Contains(current))
                {
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

                    result.Add(Token.GetNumericLiteral(decimal.Parse(fettledString, System.Globalization.NumberStyles.Float)));

                    // Move on appropriate number of characters - last +1 is at end of loop
                    index += readString.Length - 1;
                }
                // Single argument
                else if ("\uDFD8\uDFD9\uDFDA\uDFDB\uDFDC\uDFDD\uDFDE\uDFDF\uDFE0\uDFE1".Contains(current))
                {
                    var argumentIndex = current - 0xDFD8;
                    result.Add(Token.GetSingleArgument(argumentIndex));
                }
                // Get variable
                else if ("\uDD52\uDD53\uDD54\uDD55\uDD56\uDD57\uDD58\uDD59\uDD5A\uDD5B".Contains(current))
                {
                    result.Add(Token.GetGetVariable(current));
                }
                // Set variable
                else if ("\uDD38\uDD39\u2102\uDD3B\uDD3C\uDD3D\uDD3E\u210D\uDD40\uDD41".Contains(current))
                {
                    result.Add(Token.GetSetVariable(current));
                }
                // Single character token
                else
                {
                    result.Add(Token.Get(current));
                }

                // Move on to next character
                index++;
            }

            return result;
        }
    }
}
