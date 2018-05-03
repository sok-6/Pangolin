using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Pangolin.Core.DataValueImplementations;

namespace Pangolin.Core.Test
{
    public class ArgumentParserTests
    {
        [Fact]
        public void ArgumentParser_should_parse_empty_string_as_0_arguments()
        {
            // Arrange
            var argumentString = "";

            // Act
            var result = ArgumentParser.ParseArguments(argumentString, false);

            // Assert
            result.Count.ShouldBe(0);
        }

        [Fact]
        public void ArgumentParser_should_parse_whitespace_only_string_as_0_arguments()
        {
            // Arrange
            var argumentString = " \t";

            // Act
            var result = ArgumentParser.ParseArguments(argumentString, false);

            // Assert
            result.Count.ShouldBe(0);
        }

        [Fact]
        public void ArgumentParser_should_correctly_parse_numerics()
        {
            // Arrange
            var argumentStrings = new Dictionary<string, decimal>()
            {
                { "123", 123 },
                { "-123", -123 },
                { "12.3", 12.3m },
                { "-12.3", -12.3m },
                { "1e3", 1000m },
                { "-1e3", -1000m },
                { "1.2e3", 1200m },
                { "-1.2e3", -1200m },
                { "1e-3", 0.001m },
                { "-1e-3", -0.001m },
                { "1.2e-3", 0.0012m },
                { "-1.2e-3", -0.0012m }
            };

            // Act
            var results = argumentStrings.Select(a => new { Code = a.Key, Expected = a.Value, ParsedArguments = ArgumentParser.ParseArguments(a.Key, false) });

            // Assert
            foreach (var r in results)
            {
                r.ParsedArguments.Count.ShouldBe(1);
                r.ParsedArguments[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(r.Expected, () => $"Code = \"{r.Code}\", Expected = {r.Expected}");
            }
        }

        [Fact]
        public void ArgumentParser_should_correctly_parse_numerics_separated_by_spaces()
        {
            // Arrange
            var argumentStrings = new Dictionary<string, decimal[]>()
            {
                { "123 456", new decimal[] { 123, 456 } },
                { "123 1e3", new decimal[] { 123, 1000 } },
                { "1.23 1e3 1.2e-3", new decimal[] { 1.23m, 1000, 0.0012m } }
            };

            // Act
            var results = argumentStrings.Select(a => new { Code = a.Key, Expected = a.Value, ParsedArguments = ArgumentParser.ParseArguments(a.Key, false) });

            // Assert
            foreach (var r in results)
            {
                r.ParsedArguments.Count.ShouldBe(r.Expected.Length);
                for (int i = 0; i < r.Expected.Length; i++)
                {
                    r.ParsedArguments[i].ShouldBeOfType<NumericValue>().Value.ShouldBe(r.Expected[i], () => $"Code = \"{r.Code}\", argument index = {i}, Expected = {r.Expected[i]}");
                }
            }
        }

        [Fact]
        public void ArgumentParser_should_correctly_parse_quoted_strings()
        {
            // Arrange
            var argumentStrings = new Dictionary<string, string>()
            {
                { "'abc'", "abc" }, // Single quote
                { "'a''bc'", "a''bc" }, // Single quote containing single quotes
                { "'a\"\"bc'", "a\"\"bc" }, // Single quote containing double quotes
                { "'a\"\"b'c'", "a\"\"b'c" }, // Single quote containing both
                { "\"abc\"", "abc" }, // Double quote
                { "\"a''bc\"", "a''bc" }, // Double quote containing single quotes
                { "\"a\"\"bc\"", "a\"\"bc" }, // Double quote containing double quotes
                { "\"a\"\"b'c\"", "a\"\"b'c" } // Double quote containing both
            };

            // Act
            var results = argumentStrings.Select(a => new { Code = a.Key, Expected = a.Value, ParsedArguments = ArgumentParser.ParseArguments(a.Key, false) });

            // Assert
            foreach (var r in results)
            {
                r.ParsedArguments.Count.ShouldBe(1);
                r.ParsedArguments[0].ShouldBeOfType<StringValue>().Value.ShouldBe(r.Expected, () => $"Code = \"{r.Code}\", Expected = {r.Expected}");
            }
        }

        [Fact]
        public void ArgumentParser_should_correctly_parse_quoted_strings_separated_by_spaces()
        {
            // Arrange
            var argumentStrings = new Dictionary<string, string[]>()
            {
                { "'abc' \"def\"", new string[] { "abc", "def" } }
            };

            // Act
            var results = argumentStrings.Select(a => new { Code = a.Key, Expected = a.Value, ParsedArguments = ArgumentParser.ParseArguments(a.Key, false) });

            // Assert
            foreach (var r in results)
            {
                r.ParsedArguments.Count.ShouldBe(r.Expected.Length);

                for (int i = 0; i < r.ParsedArguments.Count; i++)
                {
                    r.ParsedArguments[i].ShouldBeOfType<StringValue>().Value.ShouldBe(r.Expected[i], () => $"Code = \"{r.Code}\", argument index = {i}, Expected = {r.Expected[i]}"); 
                }
            }
        }

        [Fact]
        public void ArgumentParser_should_correctly_parse_empty_array()
        {
            // Arrange
            var argumentString = "[]";

            // Act
            var result = ArgumentParser.ParseArguments(argumentString, false);

            // Assert
            result.Count.ShouldBe(1);
            var arrayResult = result[0].ShouldBeOfType<ArrayValue>();
            arrayResult.Value.Count.ShouldBe(0);
        }

        [Fact]
        public void ArgumentParser_should_correctly_parse_empty_array_with_whitespace()
        {
            // Arrange
            var argumentString = "[ \t]";

            // Act
            var result = ArgumentParser.ParseArguments(argumentString, false);

            // Assert
            result.Count.ShouldBe(1);
            var arrayResult = result[0].ShouldBeOfType<ArrayValue>();
            arrayResult.Value.Count.ShouldBe(0);
        }

        [Fact]
        public void ArgumentParser_should_correctly_parse_array_containing_single_element()
        {
            // Arrange
            var argumentString0 = "[123]";
            var argumentString1 = "['abc']";

            // Act
            var result0 = ArgumentParser.ParseArguments(argumentString0, false);
            var result1 = ArgumentParser.ParseArguments(argumentString1, false);

            // Assert
            result0.Count.ShouldBe(1);
            var arrayResult0 = result0[0].ShouldBeOfType<ArrayValue>();
            arrayResult0.Value.Count.ShouldBe(1);
            arrayResult0.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(123);

            result1.Count.ShouldBe(1);
            var arrayResult1 = result1[0].ShouldBeOfType<ArrayValue>();
            arrayResult1.Value.Count.ShouldBe(1);
            arrayResult1.Value[0].ShouldBeOfType<StringValue>().Value.ShouldBe("abc");
        }


        [Fact]
        public void ArgumentParser_should_correctly_parse_array_containing_multiple_elements()
        {
            // Arrange
            var argumentString = "[123 'abc' 456]";

            // Act
            var result = ArgumentParser.ParseArguments(argumentString, false);

            // Assert
            result.Count.ShouldBe(1);
            var arrayResult = result[0].ShouldBeOfType<ArrayValue>();
            arrayResult.Value.Count.ShouldBe(3);
            arrayResult.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(123);
            arrayResult.Value[1].ShouldBeOfType<StringValue>().Value.ShouldBe("abc");
            arrayResult.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(456);
        }


        [Fact]
        public void ArgumentParser_should_correctly_parse_multiple_arrays_separated_by_spaces()
        {
            // Arrange
            var argumentString = "[] [\"abc\"] [123 456]";

            // Act
            var result = ArgumentParser.ParseArguments(argumentString, false);

            // Assert
            result.Count.ShouldBe(3);
            var arrayResult0 = result[0].ShouldBeOfType<ArrayValue>();
            arrayResult0.Value.Count.ShouldBe(0);
            var arrayResult1 = result[1].ShouldBeOfType<ArrayValue>();
            arrayResult1.Value.Count.ShouldBe(1);
            arrayResult1.Value[0].ShouldBeOfType<StringValue>().Value.ShouldBe("abc");
            var arrayResult2 = result[2].ShouldBeOfType<ArrayValue>();
            arrayResult2.Value.Count.ShouldBe(2);
            arrayResult2.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(123);
            arrayResult2.Value[1].ShouldBeOfType<NumericValue>().Value.ShouldBe(456);
        }

        [Fact]
        public void ArgumentParser_should_correctly_parse_nested_arrays()
        {
            // Arrange
            var argumentString = "[1 [[2  [3]] 4] 5]";

            // Act
            var result = ArgumentParser.ParseArguments(argumentString, false);

            // Assert
            result.Count.ShouldBe(1);

            var array1 = result[0].ShouldBeOfType<ArrayValue>();
            array1.Value.Count.ShouldBe(3);
            array1.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            var array2 = array1.Value[1].ShouldBeOfType<ArrayValue>();
            array1.Value[2].ShouldBeOfType<NumericValue>().Value.ShouldBe(5);

            array2.Value.Count.ShouldBe(2);
            var array3 = array2.Value[0].ShouldBeOfType<ArrayValue>();
            array2.Value[1].ShouldBeOfType<NumericValue>().Value.ShouldBe(4);

            array3.Value.Count.ShouldBe(2);
            array3.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(2);
            var array4 = array3.Value[1].ShouldBeOfType<ArrayValue>();

            array4.Value.Count.ShouldBe(1);
            array4.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(3);
        }

        [Fact]
        public void ArgumentParser_should_interpret_other_values_as_strings()
        {
            // Arrange
            var argumentString = "ab se 2[ a'a    \t2ee";

            // Act
            var result = ArgumentParser.ParseArguments(argumentString, false);

            // Assert
            result.Count.ShouldBe(5);
            result[0].ShouldBeOfType<StringValue>().Value.ShouldBe("ab");
            result[1].ShouldBeOfType<StringValue>().Value.ShouldBe("se");
            result[2].ShouldBeOfType<StringValue>().Value.ShouldBe("2[");
            result[3].ShouldBeOfType<StringValue>().Value.ShouldBe("a'a");
            result[4].ShouldBeOfType<StringValue>().Value.ShouldBe("2ee");
        }

        [Fact]
        public void ArgumentParser_should_throw_error_when_unmatched_open_array_found()
        {
            // Arrange
            var argumentString = "[";

            // Act / Assert
            var exception = Should.Throw(() => ArgumentParser.ParseArguments(argumentString, false), typeof(Pangolin.Common.PangolinInvalidArgumentStringException));
            exception.Message.ShouldBe("Unmatched [ found - no matching ] before end of input");
        }

        [Fact]
        public void ArgumentParser_should_throw_error_when_unmatched_close_array_found()
        {
            // Arrange
            var argumentString = "]";

            // Act / Assert
            var exception = Should.Throw(() => ArgumentParser.ParseArguments(argumentString, false), typeof(Pangolin.Common.PangolinInvalidArgumentStringException));
            exception.Message.ShouldBe("Found ] without matching [");
        }
    }
}
