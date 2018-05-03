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

        public static IReadOnlyList<DataValue> ParseArguments(string arguments, bool argumentParseLogging)
        {
            var valueListStack = new Stack<List<DataValue>>();
            valueListStack.Push(new List<DataValue>());

            arguments = arguments.Trim();

            while (arguments.Length > 0)
            {
                // New array
                if (arguments[0] == '[')
                {
                    valueListStack.Push(new List<DataValue>());
                    arguments = arguments.Substring(1).Trim();
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
                }
                // String surrounded by quotes
                else if (Regex.IsMatch(arguments, REGEX_QUOTED_STRING))
                {
                    var match = Regex.Match(arguments, REGEX_QUOTED_STRING);

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
                    valueListStack.Peek().Add(new NumericValue(decimal.Parse(match.Value, System.Globalization.NumberStyles.Float)));

                    // Remove matched string from argument string
                    arguments = arguments.Substring(match.Length).Trim();
                }
                // Unquoted string, add all up until the next space
                else
                {
                    var match = Regex.Match(arguments, REGEX_UNQUOTED_STRING);

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

            return valueListStack.Pop();
        }
    }
}
