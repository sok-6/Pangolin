using Pangolin.Common;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pangolin.Core.Test
{
    public class CodePageTests
    {
        // Ignored for now as json is _hard_
        //[Fact]
        //public void CodePage_Provide_should_set_correct_string()
        //{
        //    // Arrange
        //    var codePoints = @"{characters}" new string[] { "a", "b", "c" };

        //    // Act
        //    CodePage.ProvideCodePoints(codePoints);

        //    // Assert
        //    CodePage.CodePoints.ShouldBe(codePoints, Case.Sensitive);
        //}

        //[Fact]
        //public void CodePage_Provide_should_set_correct_string_with_newline_and_space()
        //{
        //    // Arrange
        //    var codePoints = new string[] { "a", "newline", "space", "c" };

        //    // Act
        //    CodePage.ProvideCodePoints(codePoints);

        //    // Assert
        //    CodePage.CodePoints.ShouldBe(new string[] { "a", "\n", " ", "c" });
        //}

        //[Fact]
        //public void CodePage_Provide_should_throw_exception_if_duplicate_characters_present()
        //{
        //    // Arrange
        //    var codePoints = new string[] { "a", "a" };

        //    // Act/Assert
        //    Should.Throw<PangolinException>(() => CodePage.ProvideCodePoints(codePoints)).Message.ShouldBe("Character a present more than once in code points");
        //}

        //[Fact]
        //public void CodePage_should_get_correct_index()
        //{
        //    // Arrange
        //    var codePoints = new string[] { "a", "b", "c" };
        //    CodePage.ProvideCodePoints(codePoints);

        //    // Act
        //    var result = CodePage.GetIndexFromCharacter("b");

        //    // Assert
        //    result.ShouldBe(1);
        //}

        //[Fact]
        //public void CodePage_should_throw_exception_if_invalid_token_requested()
        //{
        //    // Arrange
        //    var codePoints = new string[] { "a", "b", "c" };
        //    CodePage.ProvideCodePoints(codePoints);

        //    // Act
        //    Should.Throw<PangolinException>(() => CodePage.GetIndexFromCharacter("z")).Message.ShouldBe("Unrecognised token z");
        //}

        //[Fact]
        //public void CodePage_should_get_correct_character()
        //{
        //    // Arrange
        //    var codePoints = new string[] { "a", "b", "c" };
        //    //CodePage.ProvideCodePoints(codePoints);

        //    // Act
        //    var result = CodePage.GetCharacterFromIndex(1);

        //    // Assert
        //    result.ShouldBe("b");
        //}

        //[Fact]
        //public void CodePage_should_throw_exception_if_invalid_index_requested()
        //{
        //    // Arrange
        //    var codePoints = new string[] { "a", "b", "c" };
        //    CodePage.ProvideCodePoints(codePoints);

        //    // Act
        //    Should.Throw<PangolinException>(() => CodePage.GetCharacterFromIndex(100)).Message.ShouldBe("Invalid code point index 100");
        //}
    }
}
