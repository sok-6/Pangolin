using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Core.TokenImplementations;

namespace Pangolin.Core.Test.Tokens.ImplementationUnitTests
{
    public class JoinTests
    {
        [Fact]
        public void Join_should_throw_exception_if_second_argument_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1).Object,
                MockFactory.MockNumericValue(1).Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockNumericValue(1).Object);
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockNumericValue(1).Object);

            var token = new Join();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to Join command - Numeric,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to Join command - String,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to Join command - Array,Numeric");
        }

        [Fact]
        public void Join_should_join_array_on_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1).Object,
                MockFactory.MockArrayBuilder.StartingStrings("abc", "def", "ghi").Complete());
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(100000).Object,
                MockFactory.MockArrayBuilder.StartingStrings("abc", "def", "ghi").Complete());
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(5, 6, 7, 8, 9).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1).Object,
                MockFactory.MockArrayBuilder.Empty);

            var token = new Join();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abc1def1ghi");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abc100000def100000ghi");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("516171819");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Join_should_join_array_on_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("xyz").Object,
                MockFactory.MockArrayBuilder.StartingStrings("abc", "def", "ghi").Complete());
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("").Object,
                MockFactory.MockArrayBuilder.StartingStrings("abc", "def", "ghi").Complete());
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("xyz").Object,
                MockFactory.MockArrayBuilder.StartingNumerics(5, 6, 7, 8, 9).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("xyz").Object,
                MockFactory.MockArrayBuilder.Empty);

            var token = new Join();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abcxyzdefxyzghi");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abcdefghi");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("5xyz6xyz7xyz8xyz9");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Join_should_join_array_on_elements_of_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1,2,3).Complete(),
                MockFactory.MockArrayBuilder.StartingStrings("abc", "def", "ghi").Complete());
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockArrayBuilder.StartingStrings("abc", "def", "ghi", "jkl", "mno").Complete()); // Test modular indexing

            var token = new Join();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abc1def2ghi");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abc1def2ghi3jkl1mno");
        }

        [Fact]
        public void Join_should_join_characters_of_string_on_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1).Object,
                MockFactory.MockStringValue("abc").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1000000).Object,
                MockFactory.MockStringValue("abc").Object);
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1).Object,
                MockFactory.MockStringValue("").Object);

            var token = new Join();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a1b1c");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a1000000b1000000c");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Join_should_join_characters_of_string_on_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("xyz").Object,
                MockFactory.MockStringValue("abc").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("").Object,
                MockFactory.MockStringValue("abc").Object);
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("xyz").Object,
                MockFactory.MockStringValue("").Object);

            var token = new Join();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("axyzbxyzc");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abc");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Join_should_join_characters_of_string_on_elements_of_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockStringValue("abc").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockStringValue("abcde").Object); // Test modular indexing

            var token = new Join();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a1b2c");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a1b2c3d1e");
        }

        [Fact]
        public void JoinOnSpaces_should_throw_exception_if_argument_numeric()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1).Object);

            var token = new JoinOnSpaces();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Invalid argument type passed to JoinOnSpaces command - Numeric");
        }

        [Fact]
        public void JoinOnSpaces_should_join_characters_of_string_on_spaces()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abc");
            var mockProgramState2 = MockFactory.MockProgramState("");

            var token = new JoinOnSpaces();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a b c");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void JoinOnSpaces_should_join_array_on_spaces()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abc", "def", "ghi").Complete());

            var token = new JoinOnSpaces();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("1 2 3");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abc def ghi");
        }

        [Fact]
        public void JoinOnNewlines_should_throw_exception_if_argument_numeric()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1).Object);

            var token = new JoinOnNewlines();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Invalid argument type passed to JoinOnNewlines command - Numeric");
        }

        [Fact]
        public void JoinOnNewlines_should_join_characters_of_string_on_newlines()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abc");
            var mockProgramState2 = MockFactory.MockProgramState("");

            var token = new JoinOnNewlines();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a\nb\nc");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void JoinOnNewlines_should_join_array_on_newlines()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abc", "def", "ghi").Complete());

            var token = new JoinOnNewlines();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("1\n2\n3");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abc\ndef\nghi");
        }
    }
}
