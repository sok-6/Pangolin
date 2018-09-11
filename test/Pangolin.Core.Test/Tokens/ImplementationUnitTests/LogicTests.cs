using Moq;
using Pangolin.Common;
using Pangolin.Core.DataValueImplementations;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pangolin.Core.Test.Tokens.ImplementationUnitTests
{
    public class LogicTests
    {
        [Fact]
        public void LogicAnd_should_return_truthy_when_both_operands_truthy()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockDataValue(true).Object);

            var token = new TokenImplementations.LogicAnd();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void LogicAnd_should_return_falsey_when_second_operand_falsey()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockDataValue(false).Object);

            var token = new TokenImplementations.LogicAnd();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void LogicAnd_should_return_falsey_when_first_operand_falsey_without_evaluating_second_operand()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockDataValue(false).Object);
            mockProgramState.Setup(p => p.StepOverNextFunction());

            var token = new TokenImplementations.LogicAnd();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            mockProgramState.Verify(p => p.DequeueAndEvaluate(), Times.Once);
            mockProgramState.Verify(p => p.StepOverNextFunction(), Times.Once);
        }

        [Fact]
        public void LogicOr_should_return_falsey_when_both_operands_falsey()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object, 
                MockFactory.MockDataValue(false).Object);

            var token = new TokenImplementations.LogicOr();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void LogicOr_should_return_truthy_when_second_operand_truthy()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object,
                MockFactory.MockDataValue(true).Object);

            var token = new TokenImplementations.LogicOr();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void LogicOr_should_return_truthy_when_first_operand_truthy_without_evaluating_second_operand()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(MockFactory.MockDataValue(true).Object);
            mockProgramState.Setup(p => p.StepOverNextFunction());

            var token = new TokenImplementations.LogicOr();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            mockProgramState.Verify(p => p.DequeueAndEvaluate(), Times.Once);
            mockProgramState.Verify(p => p.StepOverNextFunction(), Times.Once);
        }

        [Fact]
        public void LogicXor_should_follow_xor_truth_table()
        {
            // Arrange
            // Test 1 - false,false => false
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object,
                MockFactory.MockDataValue(false).Object);

            // Test 2 - false,true => true
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object,
                MockFactory.MockDataValue(true).Object);

            // Test 3 - true,false => true
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockDataValue(false).Object);

            // Test 4 - true,true => false
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockDataValue(true).Object);

            var token = new TokenImplementations.LogicXor();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void LogicXnor_should_follow_xnor_truth_table()
        {
            // Arrange
            // Test 1 - false,false => true
            var mockProgramState1 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object,
                MockFactory.MockDataValue(false).Object);

            // Test 2 - false,true => false
            var mockProgramState2 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(false).Object,
                MockFactory.MockDataValue(true).Object);

            // Test 3 - true,false => false
            var mockProgramState3 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockDataValue(false).Object);

            // Test 4 - true,true => true
            var mockProgramState4 = MockFactory.MockProgramState(
                MockFactory.MockDataValue(true).Object,
                MockFactory.MockDataValue(true).Object);

            var token = new TokenImplementations.LogicXnor();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void Disjunction_ToLower_should_check_if_any_digits_of_non_negative_integral_are_non_zero()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(99999999);
            var mockProgramState3 = MockFactory.MockProgramState(0);

            var token = new TokenImplementations.Disjunction_ToLower();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Disjunction_ToLower_should_throw_exception_if_negative_or_float()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(-10);
            var mockProgramState2 = MockFactory.MockProgramState(2.5);

            var token = new TokenImplementations.Disjunction_ToLower();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Disjunction_ToLower not defined for negative values - arg=-10");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Disjunction_ToLower not defined for float values - arg=2.5");
        }

        [Fact]
        public void Disjunction_ToLower_should_convert_string_to_lower_case()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("ABC");
            var mockProgramState2 = MockFactory.MockProgramState("abc");
            var mockProgramState3 = MockFactory.MockProgramState("");
            var mockProgramState4 = MockFactory.MockProgramState("XyZ\u0394");

            var token = new TokenImplementations.Disjunction_ToLower();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abc");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("abc");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("xyz\u03B4");
        }

        [Fact]
        public void Disjunction_ToLower_should_check_if_any_elements_of_array_are_truthy()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1,2,0).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(0, 0, 0).Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abc", "def", "").Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("", "").Complete());
            var mockProgramState5 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete())
                .WithArray(MockFactory.MockArrayBuilder.Empty).Complete());
            var mockProgramState6 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.Empty).Complete());
            var mockProgramState7 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new TokenImplementations.Disjunction_ToLower();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);
            var result6 = token.Evaluate(mockProgramState6.Object);
            var result7 = token.Evaluate(mockProgramState7.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result5.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result6.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result7.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }
        
        [Fact]
        public void Conjunction_ToUpper_should_check_if_all_digits_of_non_negative_integral_are_non_zero()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(99999999);
            var mockProgramState3 = MockFactory.MockProgramState(0);

            var token = new TokenImplementations.Conjunction_ToUpper();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Conjuction_ToUpper_should_throw_exception_if_negative_or_float()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(-10);
            var mockProgramState2 = MockFactory.MockProgramState(2.5);

            var token = new TokenImplementations.Conjunction_ToUpper();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Conjunction_ToUpper not defined for negative values - arg=-10");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Conjunction_ToUpper not defined for float values - arg=2.5");
        }

        [Fact]
        public void Conjunction_ToUpper_should_convert_string_to_lower_case()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("ABC");
            var mockProgramState2 = MockFactory.MockProgramState("abc");
            var mockProgramState3 = MockFactory.MockProgramState("");
            var mockProgramState4 = MockFactory.MockProgramState("XyZ\u03B4");

            var token = new TokenImplementations.Conjunction_ToUpper();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("ABC");
            result2.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("ABC");
            result3.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("");
            result4.ShouldBeAssignableTo<StringValue>().Value.ShouldBe("XYZ\u0394");
        }

        [Fact]
        public void Conjunction_ToUpper_should_check_if_all_elements_of_array_are_truthy()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(0, 0, 0).Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abc", "def", "ghi").Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("", "").Complete());
            var mockProgramState5 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete()).Complete());
            var mockProgramState6 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.Empty).Complete());
            var mockProgramState7 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new TokenImplementations.Conjunction_ToUpper();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);
            var result6 = token.Evaluate(mockProgramState6.Object);
            var result7 = token.Evaluate(mockProgramState7.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result5.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result6.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result7.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void InverseDisjunction_should_check_if_any_digits_of_non_negative_integral_are_zero()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(99999999);
            var mockProgramState3 = MockFactory.MockProgramState(0);

            var token = new TokenImplementations.InverseDisjunction();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void InverseDisjunction_should_throw_exception_if_negative_or_float()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(-10);
            var mockProgramState2 = MockFactory.MockProgramState(2.5);

            var token = new TokenImplementations.InverseDisjunction();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("InverseDisjunction not defined for negative values - arg=-10");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("InverseDisjunction not defined for float values - arg=2.5");
        }

        [Fact]
        public void InverseDisjunction_should_throw_exception_for_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("ABC");

            var token = new TokenImplementations.InverseDisjunction();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument type passed to InverseDisjunction command - String");
        }

        [Fact]
        public void InverseDisjunction_should_check_if_any_elements_of_array_are_falsey()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(0, 4, 0).Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abc", "def", "def").Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abc", "").Complete());
            var mockProgramState5 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete()).Complete());
            var mockProgramState6 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.Empty).Complete());
            var mockProgramState7 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new TokenImplementations.InverseDisjunction();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);
            var result6 = token.Evaluate(mockProgramState6.Object);
            var result7 = token.Evaluate(mockProgramState7.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result5.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result6.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result7.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void InverseConjunction_should_check_if_all_digits_of_non_negative_integral_are_zero()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10);
            var mockProgramState2 = MockFactory.MockProgramState(99999999);
            var mockProgramState3 = MockFactory.MockProgramState(0);

            var token = new TokenImplementations.InverseConjunction();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
        }

        [Fact]
        public void InverseConjunction_should_throw_exception_if_negative_or_float()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(-10);
            var mockProgramState2 = MockFactory.MockProgramState(2.5);

            var token = new TokenImplementations.InverseConjunction();

            // Act/Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("InverseConjunction not defined for negative values - arg=-10");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("InverseConjunction not defined for float values - arg=2.5");
        }

        [Fact]
        public void InverseConjunction_should_throw_exception_for_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("ABC");

            var token = new TokenImplementations.InverseConjunction();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument type passed to InverseConjunction command - String");
        }

        [Fact]
        public void InverseConjunction_should_check_if_all_elements_of_array_are_falsey()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 0).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(0, 0, 0).Complete());
            var mockProgramState3 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("abc", "def", "def").Complete());
            var mockProgramState4 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingStrings("", "").Complete());
            var mockProgramState5 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 0).Complete()).Complete());
            var mockProgramState6 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder
                .StartingArray(MockFactory.MockArrayBuilder.Empty).Complete());
            var mockProgramState7 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new TokenImplementations.InverseConjunction();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);
            var result6 = token.Evaluate(mockProgramState6.Object);
            var result7 = token.Evaluate(mockProgramState7.Object);

            // Assert
            result1.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result4.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result5.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
            result6.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(1);
            result7.ShouldBeAssignableTo<NumericValue>().Value.ShouldBe(0);
        }
    }
}
