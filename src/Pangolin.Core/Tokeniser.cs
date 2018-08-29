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

            log($"Sanitising code string");

            code = String.Concat(code.Where(c => CodePage.CharacterExistsInCodePage(c)));

            // Iterate over characters until all processed
            var index = 0;
            while (index < code.Length)
            {
                log($"Remaining tokens = {code.Substring(index)}");

                var current = code[index];
                if (current == ' ')
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
                                break;
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
                            result.Add(Token.GetNumericLiteral(DecodeCompressedNumeric(sb.ToString(), true, log)));
                        }
                    }
                }
                // 2 char string/single dictionary entry
                else if (current == '\u00BB')
                {
                    log("2 char string/single dictionary entry");

                    // Check there are 2 more characters to use
                    if (index + 2 >= code.Length)
                    {
                        throw new PangolinInvalidTokenException("Need 2 characters to follow \u00BB token");
                    }

                    var characters = code.Substring(index + 1, 2);
                    index += 2;

                    log($"Characters = {characters}");

                    result.Add(Token.GetStringLiteral(DecodeCompressedString(characters, log)));
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
                    var readString = Regex.Match(code.Substring(index), @"^((\d+(\.\d+)?|\.\d+)(\u23E8-?)?\d*|\u23E8-?\d*)").Value;

                    // Replace sub-10 with e, parse
                    var fettledString = readString.Replace('\u23E8', 'e');

                    // If starts with e, prepend 1
                    if (fettledString.StartsWith('e')) fettledString = $"1{fettledString}";

                    // If ends with e, append 1
                    if (fettledString.EndsWith('e')) fettledString = $"{fettledString}1";

                    var literalValue = double.Parse(fettledString, System.Globalization.NumberStyles.Float);
                    log($"Plain numeric constant, match = {readString},  value = {literalValue}");
                    result.Add(Token.GetNumericLiteral(literalValue));

                    // Move on appropriate number of characters - last +1 is at end of loop
                    index += readString.Length - 1;
                }
                // Single digit code page numeric
                else if (current == '\u2020')
                {
                    if (index + 1 == code.Length)
                    {
                        throw new PangolinInvalidTokenException("Must be at least 1 character following \u2020");
                    }

                    index++;
                    result.Add(Token.GetNumericLiteral(DecodeCompressedNumeric(code.Substring(index, 1), false, log)));
                }
                // Double digit code page numeric
                else if (current == '\u2021')
                {
                    if (index + 2 >= code.Length)
                    {
                        throw new PangolinInvalidTokenException("Must be at least 2 characters following \u2021");
                    }

                    result.Add(Token.GetNumericLiteral(DecodeCompressedNumeric(code.Substring(index + 1, 2), false, log)));
                    index += 2;
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
            const int DICTIONARY_ISDICTIONARY_MASK = 0x80;
            const int DICTIONARY_CASE_MASK = 0x60;
            const int DICTIONARY_CASE_LOWER = 0x00;
            const int DICTIONARY_CASE_UPPER = 0x20;
            const int DICTIONARY_CASE_TITLE = 0x40;
            const int DICTIONARY_SPACE_MASK = 0x10;
            const int DICTIONARY_NUMBER_BITS = 0x0F;

            var sb = new StringBuilder();

            var codePointQueue = new Queue<int>(compressedString.Select(c => CodePage.GetIndexFromCharacter(c)));

            while (codePointQueue.Count > 0)
            {
                var current = codePointQueue.Dequeue();

                log($"Processing {current}");

                // if < 128, get code page char
                if ((current & DICTIONARY_ISDICTIONARY_MASK) == 0)
                {
                    log($"Plain character, appending");
                    sb.Append(CodePage.GetCharacterFromIndex(current));
                }
                else
                {
                    log($"Dictionary entry");
                    // Try to dequeue next char, error if none left
                    if (!codePointQueue.TryDequeue(out int next))
                    {
                        throw new PangolinException($"In compressed string, dictionary pair missing second element at end of string");
                    }

                    log($"Pair is {current}, {next}");

                    // Get case and space info
                    var caseInfo = (current & DICTIONARY_CASE_MASK);
                    var trailingSpace = (current & DICTIONARY_SPACE_MASK) != 0;

                    // Get dictionary entry
                    var dictionaryIndex = ((current & DICTIONARY_NUMBER_BITS) * 254) + next;
                    var dictionaryEntry = Dictionary.GetDictionaryEntryByIndex(dictionaryIndex);
                    log($"Index is {dictionaryIndex}, entry is {dictionaryEntry}");

                    // Format case
                    if (caseInfo == DICTIONARY_CASE_LOWER)
                    {
                        dictionaryEntry = dictionaryEntry.ToLower();
                    }
                    else if (caseInfo == DICTIONARY_CASE_UPPER)
                    {
                        dictionaryEntry = dictionaryEntry.ToUpper();
                    }
                    else if (caseInfo == DICTIONARY_CASE_TITLE)
                    {
                        dictionaryEntry = dictionaryEntry.Substring(0, 1).ToUpper() + dictionaryEntry.Substring(1).ToLower();
                    }

                    log($"Formatting applied, entry now {dictionaryEntry}");

                    // Append space if required
                    if (trailingSpace)
                    {
                        log($"Appending space");
                        dictionaryEntry = dictionaryEntry + " ";
                    }

                    sb.Append(dictionaryEntry);
                }
            }

            return sb.ToString();
        }

        public static double DecodeCompressedNumeric(string compressedNumeric, bool base254, Action<string> log)
        {
            var codePoints = new Queue<int>(compressedNumeric.Select(c => CodePage.GetIndexFromCharacter(c)));

            log($"Code points are {String.Join(",", codePoints)}");

            double result = 0;
            while (codePoints.Count > 0)
            {
                result *= base254 ? 254 : 256;
                result += codePoints.Dequeue();
            }

            log($"Numeric resoved as {result}");

            return result;
        }
    }
}
