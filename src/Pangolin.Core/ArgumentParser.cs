using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;

namespace Pangolin.Core
{
    public static class ArgumentParser
    {
        private const string REGEX_NUMERIC = @"^-?\d+(\.\d+)?([eE]-?\d+)?(?=( |]|$))";
        private const string REGEX_QUOTED_STRING = @"^((""(?<contents>.*?)"")|('(?<contents>.*?)'))(?=( |]|$))";
        private const string REGEX_UNQUOTED_STRING = @"^\S*";

        public static IReadOnlyList<DataValue> ParseArguments(string arguments, Action<string> log)
        {
            var valueListStack = new Stack<List<DataValue>>();
            valueListStack.Push(new List<DataValue>());

            arguments = arguments.Trim();

            while (arguments.Length > 0)
            {
                log($"Remaining argument string = {arguments}");

                // New array
                if (arguments[0] == '[')
                {
                    valueListStack.Push(new List<DataValue>());
                    arguments = arguments.Substring(1).Trim();
                    log($"Opening new array, array depth = {valueListStack.Count - 1}");
                }
                // Close previous array
                else if (arguments[0] == ']')
                {
                    // Check that matching [ present - i.e. stack is more than 1 deep
                    if (valueListStack.Count == 1)
                    {
                        throw new PangolinInvalidArgumentStringException("Found ] without matching [");
                    }

                    var a = valueListStack.Pop();
                    var x = new ArrayValue(a);
                    valueListStack.Peek().Add(x);
                    arguments = arguments.Substring(1).Trim();

                    log($"Closed array, array depth = {valueListStack.Count - 1}");
                }
                // String surrounded by quotes
                else if (Regex.IsMatch(arguments, REGEX_QUOTED_STRING))
                {
                    var match = Regex.Match(arguments, REGEX_QUOTED_STRING);
                    log($"Quoted string, match = {match.Value}, contents = {match.Groups["contents"].Value}");

                    // Add contents to current list
                    valueListStack.Peek().Add(new StringValue(match.Groups["contents"].Value));

                    // Remove matched string from argument string
                    arguments = arguments.Substring(match.Length).Trim();
                }
                // Check if numeric at start
                else if (Regex.IsMatch(arguments, REGEX_NUMERIC))
                {
                    var match = Regex.Match(arguments, REGEX_NUMERIC);

                    // Add contents to current list
                    var parsedValue = double.Parse(match.Value, System.Globalization.NumberStyles.Float);
                    log($"Numeric, match = {match.Value}, value = {parsedValue}");
                    valueListStack.Peek().Add(new NumericValue(parsedValue));

                    // Remove matched string from argument string
                    arguments = arguments.Substring(match.Length).Trim();
                }
                // Unquoted string, add all up until the next space
                else
                {
                    var match = Regex.Match(arguments, REGEX_UNQUOTED_STRING);
                    log($"Unquoted string, match = {match.Value}, contents = {match.Groups["contents"].Value}");

                    // Add contents to current list
                    valueListStack.Peek().Add(new StringValue(match.Value));

                    // Remove matched string from argument string
                    arguments = arguments.Substring(match.Length).Trim();
                }
            }

            // Check all braces matched - i.e. stack is exactly 1 deep
            if (valueListStack.Count != 1)
            {
                throw new PangolinInvalidArgumentStringException("Unmatched [ found - no matching ] before end of input");
            }

            var result = valueListStack.Pop();

            log($"Parse complete, result: {(result.Count == 0 ? "<none>" : "\n" + String.Join("\n", result.Select((r,i) => String.Format("\t{0}: {1}", i, r.ToString()))))}");

            return result;
        }
    }
}
