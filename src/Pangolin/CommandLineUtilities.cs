using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Pangolin
{
    static class CommandLineUtilities
    {
        private const string REGEX_BINARY_STRING = @"^[01]{8}$";
        private const string REGEX_DECIMAL_STRING = @"^\d{3}$";
        private const string REGEX_HEXADECIMAL_STRING = @"^(\d|[A-Fa-f]){2}$";

        public static (bool, string) ParseSimpleCode(string simpleCode, bool logToConsole)
        {
            Action<string> log = s => { if (logToConsole) Console.WriteLine($"SimpleCodeParseLog: {s}"); };

            log($"Parse simple code {simpleCode} start");

            var codeQueue = new Queue<char>(simpleCode);
            var sb = new StringBuilder();
            try
            {
                while (codeQueue.Count > 0)
                {
                    log($"Remaining code: {new String(codeQueue.ToArray())}");

                    var current = codeQueue.Dequeue();
                    try
                    {
                        log($"Processing character {current}");

                        // `## indicates character combination
                        if (current == '`')
                        {
                            log("` - combination");

                            // 2 chars required
                            var combination = $"{codeQueue.Dequeue()}{codeQueue.Dequeue()}";

                            log($"Combination string found - {combination}");

                            char token;
                            try
                            {
                                token = Core.CodePage.GetCharacterFromCombination(combination);
                            }
                            catch (Common.PangolinInvalidTokenException pite)
                            {
                                return (false, $"Simple encoding parse failed - unrecognised combination {combination}");
                            }

                            log($"Combination resolved as token {token}");

                            sb.Append(token);
                        }
                        // Token index - b=binary, d=decimal, h=hex
                        else if ("bdx".Contains(current))
                        {
                            int index;
                            if (current == 'b')
                            {
                                log("b - binary index");

                                // 8 chars required
                                var binaryString = $"{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}";

                                log($"Binary string found - {binaryString}");

                                if (!Regex.IsMatch(binaryString, REGEX_BINARY_STRING))
                                {
                                    return (false, $"Simple encoding parse failed - 8 characters following 'b' must be in [01] - ");
                                }

                                index = Convert.ToInt32(binaryString, 2);
                            }
                            else if (current == 'd')
                            {
                                log("d - decimal index");

                                // 3 chars required
                                var decimalString = $"{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}";

                                log($"Decimal string found - {decimalString}");

                                if (!Regex.IsMatch(decimalString, @"^\d{3}$"))
                                {
                                    return (false, $"Simple encoding parse failed - 3 characters following 'd' must be in [0-9]");
                                }

                                index = int.Parse(decimalString);
                            }
                            else // Must be hex
                            {
                                log("x - hex index");

                                // 2 chars required
                                var hexString = $"{codeQueue.Dequeue()}{codeQueue.Dequeue()}";

                                log($"Hex string found - {hexString}");

                                if (!Regex.IsMatch(hexString, REGEX_HEXADECIMAL_STRING))
                                {
                                    return (false, $"Simple encoding parse failed - 2 characters following 'x' must be in [0-9A-Fa-f]");
                                }

                                index = Convert.ToInt32(hexString, 16);
                            }

                            log($"Index resolved as decimal {index}");

                            var token = Core.CodePage.GetCharacterFromIndex(index);

                            log($"Index mapped to token {token}");

                            sb.Append(token);
                        }
                        // Literal
                        else if (current == '\\')
                        {
                            log("\\ - escaped literal");

                            var token = codeQueue.Dequeue();
                            log($"Following token is {token}");

                            sb.Append(token);
                        }
                        else
                        {
                            log($"Token {current}");
                            sb.Append(current);
                        }
                    }
                    catch (InvalidOperationException ioe)
                    {
                        return (false, $"Simple encoding parse failed - insufficient number of characters found after {current} character at end of code string");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Simple encoding parse failed - {ex.GetType().Name} thrown, message: {ex.Message}");
            }

            return (true, sb.ToString());
        }
    }
}
