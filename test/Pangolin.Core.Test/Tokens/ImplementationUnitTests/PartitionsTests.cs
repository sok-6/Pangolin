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
    public class PartitionsTests
    {
        [Fact(Skip = "Multiplicative partition not implemented yet")]
        public void MultiplicativePartitions_PowerSet_should_return_multiplicative_partition_for_non_negative_integral()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(30);
            var mockProgramState2 = MockFactory.MockProgramState(200);
            var mockProgramState3 = MockFactory.MockProgramState(0);
            var mockProgramState4 = MockFactory.MockProgramState(1);

            var token = new MultiplicativePartitions_PowerSet();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState1.Object);
            var result3 = token.Evaluate(mockProgramState1.Object);
            var result4 = token.Evaluate(mockProgramState1.Object);

            // Assert
            var array1 = result1.ShouldBeOfType<ArrayValue>();
            array1.Value.Count.ShouldBe(5);
            array1.Value[0].ShouldBeArrayWhichStartsWith(2, 3, 5);
            array1.Value[1].ShouldBeArrayWhichStartsWith(2, 15);
            array1.Value[2].ShouldBeArrayWhichStartsWith(3, 10);
            array1.Value[3].ShouldBeArrayWhichStartsWith(5, 6);
            array1.Value[4].ShouldBeArrayWhichStartsWith(30);

            var array2 = result2.ShouldBeOfType<ArrayValue>();
            array2.Value.Count.ShouldBe(5);
            array2.Value[0].ShouldBeArrayWhichStartsWith(2, 2, 2, 5, 5);
            Assert.Equal("Fill out rest of partition to test", "");

            var array3 = result3.ShouldBeOfType<ArrayValue>();
            array3.Value.Count.ShouldBe(1);
            array3.Value[0].ShouldBeArrayWhichStartsWith(0);

            var array4 = result4.ShouldBeOfType<ArrayValue>();
            array4.Value.Count.ShouldBe(1);
            array4.Value[0].ShouldBeArrayWhichStartsWith(1);
        }

        [Fact]
        public void MultiplicativePartitions_PowerSet_should_throw_exception_on_float_or_negative_integral()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(1.5);
            var mockProgramState2 = MockFactory.MockProgramState(-5);

            var token = new MultiplicativePartitions_PowerSet();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("# not defined for non-integrals - arg=1.5");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("# not defined for negative integrals - arg=-5");
        }

        [Fact]
        public void MultiplicativePartitions_PowerSet_should_return_powerset_of_iterable()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abcd");
            var mockProgramState2 = MockFactory.MockProgramState("");
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new MultiplicativePartitions_PowerSet();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith("", "a", "b", "c", "d", "ab", "ac", "ad", "bc", "bd", "cd", "abc", "abd", "acd", "bcd", "abcd");
            result2.ShouldBeArrayWhichStartsWith("");
            result3.ShouldBeArrayWhichStartsWith(v => v.ShouldBeEmptyArray())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1, 2).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1, 3).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1, 4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2, 3).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2, 4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3, 4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1, 2, 3).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1, 2, 4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1, 3, 4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2, 3, 4).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1, 2, 3, 4).End())
                .End();
            result4.ShouldBeArrayWhichStartsWith(v => v.ShouldBeEmptyArray()).End();
        }
    }
}
