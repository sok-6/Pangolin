using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Shouldly;
using Pangolin.Core.TokenImplementations;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Common;

namespace Pangolin.Core.Test.Tokens.ImplementationUnitTests
{
    public class PairTests
    {
        [Fact]
        public void SimplePair_should_throw_exception_on_numeric()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(1);

            var token = new SimplePair();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Invalid argument type passed to , command - Numeric");
        }

        [Fact]
        public void SimplePair_should_return_pairs_of_iterables()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abcde");
            var mockProgramState2 = MockFactory.MockProgramState("a");
            var mockProgramState3 = MockFactory.MockProgramState("");
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4, 5).Complete());
            var mockProgramState5 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1).Complete());
            var mockProgramState6 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new SimplePair();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);
            var result6 = token.Evaluate(mockProgramState6.Object);

            //Assert
            var array1 = result1.ShouldBeOfType<ArrayValue>().Value;
            array1.Count.ShouldBe(4);
            array1[0].ShouldBeArrayWhichStartsWith("a", "b");
            array1[1].ShouldBeArrayWhichStartsWith("b", "c");
            array1[2].ShouldBeArrayWhichStartsWith("c", "d");
            array1[3].ShouldBeArrayWhichStartsWith("d", "e");

            var array2 = result2.ShouldBeOfType<ArrayValue>().Value;
            array2.Count.ShouldBe(0);

            var array3 = result3.ShouldBeOfType<ArrayValue>().Value;
            array3.Count.ShouldBe(0);

            var array4 = result4.ShouldBeOfType<ArrayValue>().Value;
            array4.Count.ShouldBe(4);
            array4[0].ShouldBeArrayWhichStartsWith(1, 2);
            array4[1].ShouldBeArrayWhichStartsWith(2, 3);
            array4[2].ShouldBeArrayWhichStartsWith(3, 4);
            array4[3].ShouldBeArrayWhichStartsWith(4, 5);

            var array5 = result5.ShouldBeOfType<ArrayValue>().Value;
            array5.Count.ShouldBe(0);

            var array6 = result6.ShouldBeOfType<ArrayValue>().Value;
            array6.Count.ShouldBe(0);
        }
    }
}
