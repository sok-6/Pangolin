﻿using Pangolin.Common;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using System.Linq;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Pangolin.Core
{
    public class Runner : IRunner
    {
        private const string REGEX_BINARY_STRING = @"^[01]{8}$";
        private const string REGEX_DECIMAL_STRING = @"^\d{3}$";
        private const string REGEX_HEXADECIMAL_STRING = @"^(\d|[A-Fa-f]){2}$";

        public void Run(string code, string arguments, IRunOptions runOptions, IEnvironmentHandleContainer environmentHandles)
        {
            // Set up log handlers
            Action<string> argumentLogHandler = NullLogger;
            if (runOptions.ArgumentParseLogging) argumentLogHandler = environmentHandles.WriteLineToArgumentLog;

            Action<string> tokenisationLogHandler = NullLogger;
            if (runOptions.TokenisationLogging) tokenisationLogHandler = environmentHandles.WriteLineToTokenLog;

            Action<string> executionPlanLogHandler = NullLogger;
            if (runOptions.ShowExecutionPlan) executionPlanLogHandler = environmentHandles.WriteLineToExecutionPlanLog;

            Action<string> outputHandler = environmentHandles.WriteToOutput;

            // Interpret program
            try
            {
                // Parse arguments
                var parsedArguments = ArgumentParser.ParseArguments(arguments, argumentLogHandler);

                // Tokenise code
                var tokens = Tokeniser.Tokenise(code, tokenisationLogHandler);
                var programState = new ProgramState(parsedArguments, tokens);

                // Execute code until program end reached
                while (programState.ExecutionInProgress)
                {
                    System.Console.WriteLine(programState.DequeueAndEvaluate());
                }
            }
            catch (Exception ex)
            {
                outputHandler($"Exception occurred, halting program run. Message: {ex.Message}");
            }
        }

        private void NullLogger(string s) { }

        public (bool, string, string) ParseSimpleCode(string simpleCode)
        {
            var logBuilder = new StringBuilder();

            logBuilder.AppendLine($"Parse simple code {simpleCode} start");

            var codeQueue = new Queue<char>(simpleCode);
            var sb = new StringBuilder();
            try
            {
                while (codeQueue.Count > 0)
                {
                    logBuilder.AppendLine($"Remaining code: {new String(codeQueue.ToArray())}");

                    var current = codeQueue.Dequeue();
                    try
                    {
                        logBuilder.AppendLine($"Processing character {current}");

                        // `## indicates character combination
                        if (current == '`')
                        {
                            logBuilder.AppendLine("` - combination");

                            // 2 chars required
                            var combination = $"{codeQueue.Dequeue()}{codeQueue.Dequeue()}";

                            logBuilder.AppendLine($"Combination string found - {combination}");

                            char token;
                            try
                            {
                                token = Core.CodePage.GetCharacterFromCombination(combination);
                            }
                            catch (Common.PangolinInvalidTokenException pite)
                            {
                                return (false, $"Simple encoding parse failed - unrecognised combination {combination}", logBuilder.ToString());
                            }

                            logBuilder.AppendLine($"Combination resolved as token {token}");

                            sb.Append(token);
                        }
                        // Token index - b=binary, d=decimal, h=hex
                        else if ("%d#".Contains(current))
                        {
                            int index;
                            if (current == '%')
                            {
                                logBuilder.AppendLine("% - binary index");

                                // 8 chars required
                                var binaryString = $"{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}";

                                logBuilder.AppendLine($"Binary string found - {binaryString}");

                                if (!Regex.IsMatch(binaryString, REGEX_BINARY_STRING))
                                {
                                    return (false, $"Simple encoding parse failed - 8 characters following '%' must be in [01]", logBuilder.ToString());
                                }

                                index = Convert.ToInt32(binaryString, 2);
                            }
                            else if (current == 'd')
                            {
                                logBuilder.AppendLine("d - decimal index");

                                // 3 chars required
                                var decimalString = $"{codeQueue.Dequeue()}{codeQueue.Dequeue()}{codeQueue.Dequeue()}";

                                logBuilder.AppendLine($"Decimal string found - {decimalString}");

                                if (!Regex.IsMatch(decimalString, @"^\d{3}$"))
                                {
                                    return (false, $"Simple encoding parse failed - 3 characters following 'd' must be in [0-9]", logBuilder.ToString());
                                }

                                index = int.Parse(decimalString);
                            }
                            else // Must be hex
                            {
                                logBuilder.AppendLine("# - hex index");

                                // 2 chars required
                                var hexString = $"{codeQueue.Dequeue()}{codeQueue.Dequeue()}";

                                logBuilder.AppendLine($"Hex string found - {hexString}");

                                if (!Regex.IsMatch(hexString, REGEX_HEXADECIMAL_STRING))
                                {
                                    return (false, $"Simple encoding parse failed - 2 characters following '#' must be in [0-9A-Fa-f]", logBuilder.ToString());
                                }

                                index = Convert.ToInt32(hexString, 16);
                            }

                            logBuilder.AppendLine($"Index resolved as decimal {index}");

                            var token = Core.CodePage.GetCharacterFromIndex(index);

                            logBuilder.AppendLine($"Index mapped to token {token}");

                            sb.Append(token);
                        }
                        // Literal
                        else if (current == '\\')
                        {
                            logBuilder.AppendLine("\\ - escaped literal");

                            var token = codeQueue.Dequeue();
                            logBuilder.AppendLine($"Following token is {token}");

                            sb.Append(token);
                        }
                        else
                        {
                            logBuilder.AppendLine($"Token {current}");
                            sb.Append(current);
                        }
                    }
                    catch (InvalidOperationException ioe)
                    {
                        return (false, $"Simple encoding parse failed - insufficient number of characters found after {current} character at end of code string", logBuilder.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Simple encoding parse failed - {ex.GetType().Name} thrown, message: {ex.Message}", logBuilder.ToString());
            }

            return (true, sb.ToString(), logBuilder.ToString());
        }

        public IEnumerable<string> GetAlternateStringRepresentations(string target)
        {
            // TODO: Populate
            return new string[] { target };
        }

        public IEnumerable<string> GetAlternateIntegralRepresentations(int target)
        {
            var result = new List<Tuple<string, string>>();

            // Determine if less than 0, so need to preprend negation
            var negatePrefix = target < 0 ? "\u042F" : "";
            var negateSimplePrefix = target < 0 ? "`R<" : "";
            target = Math.Abs(target);

            // Plain numeric
            if (negatePrefix != "")
            {
                result.Add(new Tuple<string, string>(
                    "Plain numeric:",
                    $"\u042F{target}"));

                result.Add(new Tuple<string, string>(
                    "Plain numeric (simple encoding):",
                    $"`R<{target}"));
            }
            else
            {
                result.Add(new Tuple<string, string>(
                    "Plain numeric:",
                    target.ToString()));
            }

            // Single digit
            if (target < 11)
            {
                char singleDigit = target == 10 ? '\u23E8' : (char)('\u2080' + target);
                var singleDigitSimple = target == 10 ? "`10" : $"`{target}v";

                result.Add(new Tuple<string, string>(
                    "Single digit:",
                    $"{negatePrefix}{singleDigit}"));

                result.Add(new Tuple<string, string>(
                    "Single digit (simple encoding):",
                    $"{negateSimplePrefix}{singleDigitSimple}"));
            }

            // Single code point
            if (target < 256)
            {
                var codePoint = CodePage.GetCodePointFromIndex(target);
                
                result.Add(new Tuple<string, string>(
                    "Single code point:",
                    $"{negatePrefix}\u2020{codePoint.HexValue}"));

                result.Add(new Tuple<string, string>(
                    "Single code point:",
                    $"{negateSimplePrefix}`|-{codePoint.SimpleCombination}"));
            }

            // Double code point
            if (target < 65536)
            {
                var codeIndex1 = (target >> 8);
                var codeIndex2 = (target & 0xFF);

                var codePoint1 = CodePage.GetCodePointFromIndex(codeIndex1);
                var codePoint2 = CodePage.GetCodePointFromIndex(codeIndex2);

                result.Add(new Tuple<string, string>(
                    "Double code point:",
                    $"{negatePrefix}\u2021{codePoint1.HexValue}{codePoint2.HexValue}"));

                result.Add(new Tuple<string, string>(
                    "Double code point (simple encoding):",
                    $"{negatePrefix}`|={codePoint1.SimpleCombination}{codePoint2.SimpleCombination}"));
            }

            if (target > 0)
            {
                // Compressed numeric
                var base254Digits = new List<int>();
                var targetCopy = target;
                while (targetCopy > 0)
                {
                    base254Digits.Add(targetCopy % 254);
                    targetCopy /= 254;
                }

                // Reverse to bit in most-significant first order
                base254Digits.Reverse();
                var codePoints = base254Digits.Select(d => CodePage.GetCodePointFromIndex(d));

                result.Add(new Tuple<string, string>(
                    "Compressed numeric:",
                    $"{negatePrefix}\u00AB{String.Join("", codePoints.Select(c => c.HexValue))}\u00AB"));

                result.Add(new Tuple<string, string>(
                    "Compressed numeric (simple encoding):",
                    $"{negateSimplePrefix}`<<{String.Join("", codePoints.Select(c => c.SimpleCombination))}`<<"));

                // Exponent
                targetCopy = target;
                var power = 0;
                while (targetCopy % 10 == 0)
                {
                    power++;
                    targetCopy /= 10;
                }

                // If power == 0, no need to add exponent notation
                if (power > 0)
                {
                    var coefficient = targetCopy == 1 ? "" : targetCopy.ToString();
                    var power10 = CodePage.GetCodePointFromHex("23E8");
                    var exponent = power == 1 ? "" : power.ToString();

                    result.Add(new Tuple<string, string>(
                        "Exponent:",
                        $"{negatePrefix}{coefficient}{power10.HexValue}{exponent}"));

                    result.Add(new Tuple<string, string>(
                        "Exponent (simple encoding):",
                        $"{negateSimplePrefix}{coefficient}{power10.SimpleCombination}{exponent}"));
                }
            }
            
            // Format for return
            var padLength1 = result.Max(t => t.Item1.Length) + 1;
            var padLength2 = result.Max(t => t.Item2.Length) + 1;

            return result.Select(t => $"{t.Item1.PadRight(padLength1)}{t.Item2.PadRight(padLength2)}{t.Item2.Length} bytes{(t.Item2.Contains("\n") ? "(note newline character, U+0A)" : "")}{(t.Item2.EndsWith(' ') ? " (note trailing space)" : "")}");
        }
        /*
         * Plain numeric:
         * Plain numeric (simple encoding):
         * Single digit:
         * Single digit (simple encoding):
         * Single code point:
         * Single code point (simple encoding):
         * Double code point: 
         * Double code point (simple encoding):
         * Compressed numeric:
         * Compressed numeric (simple encoding):
         * Exponent:
         * Exponent (simple encoding):
         */
    }
}
