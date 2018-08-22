using Moq;
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
    public class ConversionsTests
    {
        [Fact]
        public void Arrayify_should_wrap_any_value_in_an_array()
        {
            // Arrange
            var mockDataValue = MockFactory.MockDataValue().Object;

            var mockProgramState = MockFactory.MockProgramState(mockDataValue);

            var token = new Arrayify();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareArrayTo(mockDataValue);
        }

        [Fact]
        public void ArrayPair_should_wrap_any_two_values_in_an_array()
        {
            // Arrange
            var mockDataValue1 = MockFactory.MockDataValue().Object;
            var mockDataValue2 = MockFactory.MockDataValue().Object;

            var mockProgramState = MockFactory.MockProgramState(mockDataValue1, mockDataValue2);

            var token = new ArrayPair();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareArrayTo(mockDataValue1, mockDataValue2);
        }

        [Fact]
        public void ArrayTriple_should_wrap_any_three_values_in_an_array()
        {
            // Arrange
            var mockDataValue1 = MockFactory.MockDataValue().Object;
            var mockDataValue2 = MockFactory.MockDataValue().Object;
            var mockDataValue3 = MockFactory.MockDataValue().Object;

            var mockProgramState = MockFactory.MockProgramState(mockDataValue1, mockDataValue2, mockDataValue3);

            var token = new ArrayTriple();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<ArrayValue>().CompareArrayTo(mockDataValue1, mockDataValue2, mockDataValue3);
        }

        [Fact]
        public void Elements_should_get_digits_of_positive_integral()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(1234);

            var token = new Elements();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(1, 2, 3, 4);
        }

        [Fact]
        public void Elements_should_get_0_for_0_numeric()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(0);

            var token = new Elements();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith(0);
        }

        [Fact]
        public void Elements_should_throw_exception_for_negatvie_or_non_integral_numerics()
        {
            // Arrange
            var ps1 = MockFactory.MockProgramState(-5);
            var ps2 = MockFactory.MockProgramState(1.2);

            var token = new Elements();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps1.Object)).Message.ShouldBe("Invalid argument type passed to \u03B4 command - negative value: -5");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps2.Object)).Message.ShouldBe("Invalid argument type passed to \u03B4 command - non-integral value: 1.2");
        }

        [Fact]
        public void Elements_should_get_characters_from_string()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(MockFactory.MockStringValue("abc").Object);

            var token = new Elements();

            // Act
            var result = token.Evaluate(ps.Object);

            // Assert
            result.ShouldBeArrayWhichStartsWith("a", "b", "c");
        }

        [Fact]
        public void Elements_should_throw_exception_for_array_value()
        {
            // Arrange
            var ps = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new Elements();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps.Object)).Message.ShouldBe("Invalid argument type passed to \u03B4 command - Array");
        }

        [Fact]
        public void Transform_Transpose_should_convert_numeric_to_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(-4);
            var mockProgramState3 = MockFactory.MockProgramState(1.2);
            var mockProgramState4 = MockFactory.MockProgramState(0);

            var token = new Transform_Transpose();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("10");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("-4");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("1.2");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("0");
        }

        [Fact]
        public void Transform_Transpose_should_parse_string_as_numeric()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("10");
            var mockProgramState2 = MockFactory.MockProgramState("-4");
            var mockProgramState3 = MockFactory.MockProgramState("1.2");
            var mockProgramState4 = MockFactory.MockProgramState("1E6");
            var mockProgramState5 = MockFactory.MockProgramState("0");

            var token = new Transform_Transpose();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(10);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-4);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1.2);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1000000);
            result5.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Transform_Transpose_should_throw_exception_for_poorly_formatted_numeric_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abc");
            var mockProgramState2 = MockFactory.MockProgramState("123abc");
            var mockProgramState3 = MockFactory.MockProgramState("1   2");
            var mockProgramState4 = MockFactory.MockProgramState("");

            var token = new Transform_Transpose();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Failed to parse string \"abc\" as numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Failed to parse string \"123abc\" as numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Failed to parse string \"1   2\" as numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Failed to parse string \"\" as numeric");
        }

        [Fact]
        public void Transform_Transpose_should_transpose_array_of_strings()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abcd", "efg", "hijk").Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abcd", "", "hijk").Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abcd").Complete());

            var token = new Transform_Transpose();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith("aeh", "bfi", "cgj").End();
            result2.ShouldBeEmptyArray();
            result3.ShouldBeArrayWhichStartsWith("a", "b", "c", "d").End();
        }

        [Fact]
        public void Transform_Transpose_should_transpose_array_of_arrays()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4).Complete())
                .WithArray(MockFactory.MockArrayBuilder.StartingNumerics(5, 6, 7).Complete())
                .WithArray(MockFactory.MockArrayBuilder.StartingStrings("a", "b", "c", "d").Complete())
                .Complete());

            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4).Complete())
                .WithArray(MockFactory.MockArrayBuilder.Empty)
                .WithArray(MockFactory.MockArrayBuilder.StartingStrings("a", "b", "c", "d").Complete())
                .Complete());

            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3, 4).Complete())
                .Complete());

            var token = new Transform_Transpose();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1
                .ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1, 5).ThenShouldContinueWith("a").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2, 6).ThenShouldContinueWith("b").End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3, 7).ThenShouldContinueWith("c").End()).End();

            result2.ShouldBeEmptyArray();

            result3
                .ShouldBeArrayWhichStartsWith(v => v.ShouldBeArrayWhichStartsWith(1).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(2).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(3).End())
                .ThenShouldContinueWith(v => v.ShouldBeArrayWhichStartsWith(4).End()).End();
        }

        [Fact]
        public void Transform_Transpose_should_throw_exception_if_array_passed_with_numerics()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());

            var token = new Transform_Transpose();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState.Object)).Message.ShouldBe("Transform_Transpose can only be evaluated on array if none of the elements are non-numeric - arg=[1,2,3]");
        }

        [Fact]
        public void BaseConversion_should_convert_positive_integrals_into_positive_integral_base()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10, 123);
            var mockProgramState2 = MockFactory.MockProgramState(20, 123);
            var mockProgramState3 = MockFactory.MockProgramState(2, 10);
            var mockProgramState4 = MockFactory.MockProgramState(1, 10);
            var mockProgramState5 = MockFactory.MockProgramState(10, 0);

            var token = new BaseConversion();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(1, 2, 3).End();
            result2.ShouldBeArrayWhichStartsWith(6, 3).End();
            result3.ShouldBeArrayWhichStartsWith(1, 0, 1, 0).End();
            result4.ShouldBeArrayWhichStartsWith(0, 0, 0, 0, 0, 0, 0, 0, 0, 0).End();
            result5.ShouldBeEmptyArray();
        }

        [Fact]
        public void BaseConversion_should_convert_integrals_into_negative_integral_base()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(-10, 123);
            var mockProgramState2 = MockFactory.MockProgramState(-10, -123);
            var mockProgramState3 = MockFactory.MockProgramState(-20, 123);
            var mockProgramState4 = MockFactory.MockProgramState(-20, -123);
            var mockProgramState5 = MockFactory.MockProgramState(-2, 10);
            var mockProgramState6 = MockFactory.MockProgramState(-2, -10);

            var token = new BaseConversion();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);
            var result6 = token.Evaluate(mockProgramState6.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(2, 8, 3).End(); // 200 - 80 + 3
            result2.ShouldBeArrayWhichStartsWith(1, 9, 3, 7).End(); // -1000 + 900 -30 + 7
            result3.ShouldBeArrayWhichStartsWith(1, 14, 3).End(); // 400 - 280 + 3
            result4.ShouldBeArrayWhichStartsWith(7, 17).End(); // - 140 + 17
            result5.ShouldBeArrayWhichStartsWith(1, 1, 1, 1, 0).End(); // + 16 - 8 + 4 - 2
            result6.ShouldBeArrayWhichStartsWith(1, 0, 1, 0).End(); // - 8 - 2
        }

        [Fact]
        public void BaseConversion_should_throw_exception_for_base_0_or_neg1()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(0, 10);
            var mockProgramState2 = MockFactory.MockProgramState(0, -10);
            var mockProgramState3 = MockFactory.MockProgramState(0, 2.5);
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.Zero, MockFactory.MockStringValue("abc").Object);
            var mockProgramState5 = MockFactory.MockProgramState(MockFactory.Zero, MockFactory.EmptyString);
            var mockProgramState6 = MockFactory.MockProgramState(MockFactory.Zero, MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState7 = MockFactory.MockProgramState(MockFactory.Zero, MockFactory.MockArrayBuilder.StartingNumerics(-1, 2, -3).Complete());
            var mockProgramState8 = MockFactory.MockProgramState(MockFactory.Zero, MockFactory.MockArrayBuilder.StartingStrings("a", "bc").Complete());
            var mockProgramState9 = MockFactory.MockProgramState(MockFactory.Zero, MockFactory.MockArrayBuilder.Empty);
            var mockProgramState10 = MockFactory.MockProgramState(-1, 10);
            var mockProgramState11 = MockFactory.MockProgramState(-1, -10);
            var mockProgramState12 = MockFactory.MockProgramState(-1, 2.5);
            var mockProgramState13 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-1).Object, MockFactory.MockStringValue("abc").Object);
            var mockProgramState14 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-1).Object, MockFactory.EmptyString);
            var mockProgramState15 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-1).Object, MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState16 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-1).Object, MockFactory.MockArrayBuilder.StartingNumerics(-1, 2, -3).Complete());
            var mockProgramState17 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-1).Object, MockFactory.MockArrayBuilder.StartingStrings("a", "bc").Complete());
            var mockProgramState18 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-1).Object, MockFactory.MockArrayBuilder.Empty);

            var token = new BaseConversion();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Conversion not possible with specified base - base=0, value=10");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Conversion not possible with specified base - base=0, value=-10");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Conversion not possible with specified base - base=0, value=2.5");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Conversion not possible with specified base - base=0, value=abc");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState5.Object)).Message.ShouldBe("Conversion not possible with specified base - base=0, value=");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState6.Object)).Message.ShouldBe("Conversion not possible with specified base - base=0, value=[1,2,3]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState7.Object)).Message.ShouldBe("Conversion not possible with specified base - base=0, value=[-1,2,-3]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState8.Object)).Message.ShouldBe("Conversion not possible with specified base - base=0, value=[a,bc]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState9.Object)).Message.ShouldBe("Conversion not possible with specified base - base=0, value=[]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState10.Object)).Message.ShouldBe("Conversion not possible with specified base - base=-1, value=10");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState11.Object)).Message.ShouldBe("Conversion not possible with specified base - base=-1, value=-10");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState12.Object)).Message.ShouldBe("Conversion not possible with specified base - base=-1, value=2.5");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState13.Object)).Message.ShouldBe("Conversion not possible with specified base - base=-1, value=abc");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState14.Object)).Message.ShouldBe("Conversion not possible with specified base - base=-1, value=");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState15.Object)).Message.ShouldBe("Conversion not possible with specified base - base=-1, value=[1,2,3]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState16.Object)).Message.ShouldBe("Conversion not possible with specified base - base=-1, value=[-1,2,-3]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState17.Object)).Message.ShouldBe("Conversion not possible with specified base - base=-1, value=[a,bc]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState18.Object)).Message.ShouldBe("Conversion not possible with specified base - base=-1, value=[]");
        }

        [Fact]
        public void BaseConversion_should_throw_exception_for_float_base()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(2.5, 10);
            var mockProgramState2 = MockFactory.MockProgramState(2.5, -10);
            var mockProgramState3 = MockFactory.MockProgramState(2.5, 2.5);
            var mockProgramState4 = MockFactory.MockProgramState(2.5, 0);
            var mockProgramState5 = MockFactory.MockProgramState(MockFactory.MockNumericValue(2.5).Object, MockFactory.MockStringValue("abc").Object);
            var mockProgramState6 = MockFactory.MockProgramState(MockFactory.MockNumericValue(2.5).Object, MockFactory.EmptyString);
            var mockProgramState7 = MockFactory.MockProgramState(MockFactory.MockNumericValue(2.5).Object, MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState8 = MockFactory.MockProgramState(MockFactory.MockNumericValue(2.5).Object, MockFactory.MockArrayBuilder.StartingNumerics(-1, 2, -3).Complete());
            var mockProgramState9 = MockFactory.MockProgramState(MockFactory.MockNumericValue(2.5).Object, MockFactory.MockArrayBuilder.StartingStrings("a", "bc").Complete());
            var mockProgramState10 = MockFactory.MockProgramState(MockFactory.MockNumericValue(2.5).Object, MockFactory.MockArrayBuilder.Empty);

            var token = new BaseConversion();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Non-integral bases not implemented yet - base=2.5, value=10");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Non-integral bases not implemented yet - base=2.5, value=-10");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Non-integral bases not implemented yet - base=2.5, value=2.5");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Non-integral bases not implemented yet - base=2.5, value=0");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState5.Object)).Message.ShouldBe("Non-integral bases not implemented yet - base=2.5, value=abc");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState6.Object)).Message.ShouldBe("Non-integral bases not implemented yet - base=2.5, value=");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState7.Object)).Message.ShouldBe("Non-integral bases not implemented yet - base=2.5, value=[1,2,3]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState8.Object)).Message.ShouldBe("Non-integral bases not implemented yet - base=2.5, value=[-1,2,-3]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState9.Object)).Message.ShouldBe("Non-integral bases not implemented yet - base=2.5, value=[a,bc]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState10.Object)).Message.ShouldBe("Non-integral bases not implemented yet - base=2.5, value=[]");
        }

        [Fact]
        public void BaseConversion_should_throw_exception_for_positive_base_and_negative_value()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10, -10);
            var mockProgramState2 = MockFactory.MockProgramState(1, -10);

            var token = new BaseConversion();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Can't convert negative number into positive base - newBase=10, number=-10");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Can't convert negative number into positive base - newBase=1, number=-10");
        }

        [Fact]
        public void BaseConversion_should_throw_exception_for_integral_base_and_non_integral_value()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10, 2.5);
            var mockProgramState2 = MockFactory.MockProgramState(-10, 2.5);
            var mockProgramState3 = MockFactory.MockProgramState(1, 2.5);

            var token = new BaseConversion();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Base conversion of non-integral numbers not implemented yet - newBase=10, number=2.5");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Base conversion of non-integral numbers not implemented yet - newBase=-10, number=2.5");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Can't convert non-integral number into unary - newBase=1, number=2.5");
        }

        [Fact]
        public void BaseConversion_should_return_empty_array_for_integral_base_and_0_values()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10, 0);
            var mockProgramState2 = MockFactory.MockProgramState(-10, 0);
            var mockProgramState3 = MockFactory.MockProgramState(1, 0);

            var token = new BaseConversion();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeEmptyArray();
            result2.ShouldBeEmptyArray();
            result3.ShouldBeEmptyArray();
        }

        [Fact]
        public void BaseConversion_should_throw_exception_for_numeric_base_and_string_value()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockNumericValue(10).Object, MockFactory.MockStringValue("abc").Object);
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockNumericValue(10).Object, MockFactory.MockStringValue("").Object);
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-10).Object, MockFactory.MockStringValue("abc").Object);
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-10).Object, MockFactory.MockStringValue("").Object);
            var mockProgramState5 = MockFactory.MockProgramState(MockFactory.MockNumericValue(1).Object, MockFactory.MockStringValue("abc").Object);
            var mockProgramState6 = MockFactory.MockProgramState(MockFactory.MockNumericValue(1).Object, MockFactory.MockStringValue("").Object);

            var token = new BaseConversion();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to BaseConversion command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to BaseConversion command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("Invalid argument types passed to BaseConversion command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("Invalid argument types passed to BaseConversion command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState5.Object)).Message.ShouldBe("Invalid argument types passed to BaseConversion command - Numeric,String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState6.Object)).Message.ShouldBe("Invalid argument types passed to BaseConversion command - Numeric,String");
        }

        [Fact]
        public void BaseConversion_should_convert_positive_integer_array_into_integer_base()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockNumericValue(10).Object, MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockNumericValue(20).Object, MockFactory.MockArrayBuilder.StartingNumerics(6, 3).Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockNumericValue(2).Object, MockFactory.MockArrayBuilder.StartingNumerics(1, 0, 1, 0).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockNumericValue(1).Object, MockFactory.MockArrayBuilder.StartingNumerics(0, 0, 0, 0, 0, 0, 0, 0, 0, 0).Complete());
            var mockProgramState5 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-10).Object, MockFactory.MockArrayBuilder.StartingNumerics(2, 8, 3).Complete());
            var mockProgramState6 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-10).Object, MockFactory.MockArrayBuilder.StartingNumerics(1, 9, 3, 7).Complete());
            var mockProgramState7 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-20).Object, MockFactory.MockArrayBuilder.StartingNumerics(1, 14, 3).Complete());
            var mockProgramState8 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-20).Object, MockFactory.MockArrayBuilder.StartingNumerics(7, 17).Complete());
            var mockProgramState9 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-2).Object, MockFactory.MockArrayBuilder.StartingNumerics(1, 1, 1, 1, 0).Complete());
            var mockProgramState10 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-2).Object, MockFactory.MockArrayBuilder.StartingNumerics(1, 0, 1, 0).Complete());

            var token = new BaseConversion();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);
            var result6 = token.Evaluate(mockProgramState6.Object);
            var result7 = token.Evaluate(mockProgramState7.Object);
            var result8 = token.Evaluate(mockProgramState8.Object);
            var result9 = token.Evaluate(mockProgramState9.Object);
            var result10 = token.Evaluate(mockProgramState10.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(123);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(123);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(10);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(10);
            result5.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(123);
            result6.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-123);
            result7.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(123);
            result8.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-123);
            result9.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(10);
            result10.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(-10);
        }

        [Fact]
        public void BaseConversion_should_throw_exception_for_integral_base_and_array_with_element_gte_abs_base()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockNumericValue(10).Object, MockFactory.MockArrayBuilder.StartingNumerics(8, 9, 10).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockNumericValue(-10).Object, MockFactory.MockArrayBuilder.StartingNumerics(8, 9, 10).Complete());

            var token = new BaseConversion();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("When converting to decimal from numeric base, all digit values must be less than base - base=10, value=[8,9,10]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("When converting to decimal from numeric base, all digit values must be less than base - base=-10, value=[8,9,10]");
        }

        [Fact]
        public void BaseConversion_should_convert_non_negative_integral_to_string_base()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockStringValue("abcde").Object, MockFactory.MockNumericValue(123).Object);
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockStringValue("aA").Object, MockFactory.MockNumericValue(10).Object);
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockStringValue("x").Object, MockFactory.MockNumericValue(5).Object);
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockStringValue("abcde").Object, MockFactory.MockNumericValue(0).Object);

            var token = new BaseConversion();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("eed");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("AaAa");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("xxxxx");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void BaseConversion_should_convert_string_from_string_base_to_decimal()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abcde", "eed");
            var mockProgramState2 = MockFactory.MockProgramState("aA", "AaAa");
            var mockProgramState3 = MockFactory.MockProgramState("x", "xxxxx");
            var mockProgramState4 = MockFactory.MockProgramState("abcde", "");

            var token = new BaseConversion();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(123);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(10);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(5);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void BinaryConversion_should_convert_non_negative_integral_to_binary()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(123);
            var mockProgramState3 = MockFactory.MockProgramState(1);
            var mockProgramState4 = MockFactory.MockProgramState(0);

            var token = new BinaryConversion();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(1, 0, 1, 0).End();
            result2.ShouldBeArrayWhichStartsWith(1, 1, 1, 1, 0, 1, 1).End();
            result3.ShouldBeArrayWhichStartsWith(1).End();
            result4.ShouldBeEmptyArray();
        }

        [Fact]
        public void BinaryConversion_should_throw_exception_for_negative_int_or_float()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(-10);
            var mockProgramState2 = MockFactory.MockProgramState(1.23);

            var token = new BinaryConversion();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Can't convert negative number into positive base - newBase=2, number=-10");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Base conversion of non-integral numbers not implemented yet - newBase=2, number=1.23");
        }
        
        [Fact]
        public void BinaryConversion_should_throw_exception_for_strings()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abc");
            var mockProgramState2 = MockFactory.MockProgramState("");

            var token = new BinaryConversion();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument type passed to BinaryConversion command - String");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument type passed to BinaryConversion command - String");
        }
        

        [Fact]
        public void BinaryConversion_should_convert_binary_array_into_decimal()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 0, 1, 0).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 1, 1, 1, 0, 1, 1).Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new BinaryConversion();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(10);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(123);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void BinaryConversion_should_throw_exception_for_improperly_populated_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 0).WithStrings("abc").Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingArray(MockFactory.MockArrayBuilder.Empty).Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 0.5).Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState5 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(-1, 0, 1).Complete());

            var token = new BinaryConversion();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("When converting to decimal from base 2, all digit values must be numeric - value=[1,0,abc]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("When converting to decimal from base 2, all digit values must be numeric - value=[[]]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState3.Object)).Message.ShouldBe("When converting to decimal from base 2, all digit values must be integral - value=[1,0.5]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState4.Object)).Message.ShouldBe("When converting to decimal from base 2, all digit values must be less either 0 or 1 - value=[1,2,3]");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState5.Object)).Message.ShouldBe("When converting to decimal from base 2, all digit values must be less either 0 or 1 - value=[-1,0,1]");
        }
    }
}

