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
    public class OrderTests
    {
        [Fact]
        public void Ascend_should_increment_numerics()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(0);
            var mockProgramState3 = MockFactory.MockProgramState(-10);
            var mockProgramState4 = MockFactory.MockProgramState(1.23);

            var token = new Ascend();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(11);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-9);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(2.23);
        }

        [Fact]
        public void Ascend_should_sort_characters_in_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("cebda");
            var mockProgramState2 = MockFactory.MockProgramState("aaAAaAAaaA");
            var mockProgramState3 = MockFactory.MockProgramState("abc123!$%");
            var mockProgramState4 = MockFactory.MockProgramState("");

            var token = new Ascend();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abcde");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("AAAAAaaaaa");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("!$%123abc");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Ascend_should_sort_elements_of_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(4, 2, 7).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(4, -1, -7).Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("xyz", "123", "abc").Complete());

            var token = new Ascend();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(2, 4, 7).End();
            result2.ShouldBeArrayWhichStartsWith(-7, -1, 4).End();
            result3.ShouldBeArrayWhichStartsWith("123", "abc", "xyz").End();
        }

        [Fact]
        public void Ascend_should_throw_exception_if_array_types_are_not_consistent()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(4, 2, 7).WithStrings("abc").Complete());

            var token = new Ascend();

            // Act/Assert
            Should.Throw<ArgumentException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Comparison failed between types Numeric and String");
        }

        [Fact]
        public void Descend_should_decrement_numerics()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(0);
            var mockProgramState3 = MockFactory.MockProgramState(-10);
            var mockProgramState4 = MockFactory.MockProgramState(1.23);

            var token = new Descend();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(9);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-1);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-11);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0.23, 0.01);
        }

        [Fact]
        public void Decend_should_sort_characters_in_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("cebda");
            var mockProgramState2 = MockFactory.MockProgramState("aaAAaAAaaA");
            var mockProgramState3 = MockFactory.MockProgramState("abc123!$%");
            var mockProgramState4 = MockFactory.MockProgramState("");

            var token = new Descend();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("edcba");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("aaaaaAAAAA");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("cba321%$!");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Descend_should_sort_elements_of_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(4, 2, 7).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(4, -1, -7).Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("xyz", "123", "abc").Complete());

            var token = new Descend();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(7, 4, 2).End();
            result2.ShouldBeArrayWhichStartsWith(4, -1, -7).End();
            result3.ShouldBeArrayWhichStartsWith("xyz", "abc", "123").End();
        }

        [Fact]
        public void Descend_should_throw_exception_if_array_types_are_not_consistent()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(4, 2, 7).WithStrings("abc").Complete());

            var token = new Descend();

            // Act/Assert
            Should.Throw<ArgumentException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Comparison failed between types Numeric and String");
        }
    }
}
