using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Core.TokenImplementations;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pangolin.Core.Test.Tokens.ImplementationUnitTests
{
    public class PowersTests
    {
        [Fact]
        public void SquareRoot_should_throw_exception_if_non_numeric_passed()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object);

            var token = new SquareRoot();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps.Object)).Message.ShouldBe("Invalid argument type passed to \u221A command - String");
        }

        [Fact]
        public void SquareRoot_should_throw_exception_if_negative_value_passed()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(-25);

            var token = new SquareRoot();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(ps.Object)).Message.ShouldBe("Complex numbers not implemented yet, SquareRoot failed for negative value -25");
        }

        [Fact]
        public void SquareRoot_should_compute_real_square_roots()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(25);

            var token = new SquareRoot();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(5);
        }

        [Fact]
        public void Root_should_throw_exception_if_either_argument_non_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(123).Object,
                MockFactory.MockStringValue("abc").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(123).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("abc").Object,
                MockFactory.MockNumericValue(123).Object);
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete(),
                MockFactory.MockNumericValue(123).Object);

            var token = new Root();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to Root command - Numeric,String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to Root command - Numeric,Array");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to Root command - String,Numeric");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to Root command - Array,Numeric");
        }

        [Fact]
        public void Root_should_compute_ath_root_of_b()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(2, 9);
            var mockProgramState2 = MockFactory.MockProgramState(10, 0);
            var mockProgramState3 = MockFactory.MockProgramState(1, 10);

            var token = new Root();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(3);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(10);
        }

        [Fact]
        public void Power_RepeatedCartesianProduct_should_compute_b_to_power_a_for_numerics()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(2, 3);
            var mockProgramState2 = MockFactory.MockProgramState(0.5, 16);
            var mockProgramState3 = MockFactory.MockProgramState(-2, 2);
            var mockProgramState4 = MockFactory.MockProgramState(0, 3);
            var mockProgramState5 = MockFactory.MockProgramState(2, 0);

            var token = new Power_RepeatedCartesianProduct();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(9);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(4);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0.25);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result5.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Power_RepeatedCartesianProduct_should_throw_exception_if_both_arguments_0()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(0, 0);

            var token = new Power_RepeatedCartesianProduct();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Can't raise 0 to the power 0");
        }

        [Fact]
        public void Power_RepeatedCartesianProduct_should_throw_exception_if_first_argument_non_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.EmptyString,
                MockFactory.MockNumericValue(1).Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.Empty,
                MockFactory.MockNumericValue(1).Object);

            var token = new Power_RepeatedCartesianProduct();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to Power_RepeatedCartesianProduct command - String,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to Power_RepeatedCartesianProduct command - Array,Numeric");
        }

        [Fact]
        public void Power_RepeatedCartesianProduct_should_throw_exception_if_second_argument_iterable_and_first_argument_non_integral_or_negative()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1.5).Object,
                MockFactory.MockStringValue("abc").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(-2).Object,
                MockFactory.MockStringValue("abc").Object);
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1.5).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(-2).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());

            var token = new Power_RepeatedCartesianProduct();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Can't perform repeated cartesian product a non-integral number of times - repeat=1.5, set=abc");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Can't perform repeated cartesian product a negative number of times - repeat=-2, set=abc");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Can't perform repeated cartesian product a non-integral number of times - repeat=1.5, set=[1,2,3]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Can't perform repeated cartesian product a negative number of times - repeat=-2, set=[1,2,3]");
        }

        [Fact]
        public void Power_RepeatedCartesianProduct_should_take_cartesian_product_of_b_with_itself_a_times()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(3).Object,
                MockFactory.MockStringValue("abc").Object);
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(0).Object,
                MockFactory.MockStringValue("abc").Object);
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(2).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(0).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());

            var token = new Power_RepeatedCartesianProduct();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith("aaa", "aab", "aac", "aba", "abb", "abc", "aca", "acb", "acc", 
                                                 "baa", "bab", "bac", "bba", "bbb", "bbc", "bca", "bcb", "bcc", 
                                                 "caa", "cab", "cac", "cba", "cbb", "cbc", "cca", "ccb", "ccc").End();
            result2.ShouldBeArrayWhichStartsWith("").End();
            result3.ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1, 1).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1, 2).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(1, 3).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2, 1).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2, 2).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2, 3).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3, 1).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3, 2).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3, 3).End()).End();
            result4.ShouldBeArrayWhichStartsWith(v => v.ShouldBeEmptyArray()).End();
        }
    }
}
