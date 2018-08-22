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
    public class PartOfTests
    {
        [Fact]
        public void AllButFirst_ModTen_should_perform_mod_ten_on_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(123);
            var mockProgramState2 = MockFactory.MockProgramState(-15);
            var mockProgramState3 = MockFactory.MockProgramState(0);
            var mockProgramState4 = MockFactory.MockProgramState(12.3);

            var token = new AllButFirst_ModTen();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(3);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-5);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(2.3, 0.1);
        }

        [Fact]
        public void AllButFirst_ModTen_should_return_tail_for_iterable_longer_than_1()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abcde");
            var mockProgramState2 = MockFactory.MockProgramState("ab");
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2).Complete());

            var token = new AllButFirst_ModTen();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("bcde");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("b");
            result3.ShouldBeArrayWhichStartsWith(2, 3, 4, 5).End();
            result4.ShouldBeArrayWhichStartsWith(2).End();
        }

        [Fact]
        public void AllButFirst_ModTen_should_return_empty_for_iterable_shorter_than_2()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("a");
            var mockProgramState2 = MockFactory.MockProgramState("");
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new AllButFirst_ModTen();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
            result3.ShouldBeEmptyArray();
            result4.ShouldBeEmptyArray();
        }

        [Fact]
        public void AllButLast_LogTen_should_perform_log_ten_on_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(123);
            var mockProgramState2 = MockFactory.MockProgramState(15);

            var token = new AllButLast_LogTen();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(2.0899, 0.0001);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1.1761, 0.0001);
        }

        [Fact]
        public void AllButLast_LogTen_should_return_head_for_iterable_longer_than_1()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abcde");
            var mockProgramState2 = MockFactory.MockProgramState("ab");
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2).Complete());

            var token = new AllButLast_LogTen();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abcd");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("a");
            result3.ShouldBeArrayWhichStartsWith(1, 2, 3, 4).End();
            result4.ShouldBeArrayWhichStartsWith(1).End();
        }

        [Fact]
        public void AllButLast_LogTen_should_return_empty_for_iterable_shorter_than_2()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("a");
            var mockProgramState2 = MockFactory.MockProgramState("");
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new AllButLast_LogTen();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
            result3.ShouldBeEmptyArray();
            result4.ShouldBeEmptyArray();
        }
    }
}
