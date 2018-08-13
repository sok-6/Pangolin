using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;
using Pangolin.Core.TokenImplementations;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Common;

namespace Pangolin.Core.Test.Tokens.ImplementationUnitTests
{
    public class PrimeTests
    {
        [Fact]
        public void PrimesLessThanOneMillion_should_return_complete_list_of_primes()
        {
            // Arrange
            var token = new PrimesLessThanOneMillion();

            // Act
            var result = token.Evaluate(MockFactory.EmptyProgramState);

            // Assert
            var arrayResult = result.ShouldBeOfType<ArrayValue>();
            arrayResult.Value.Count.ShouldBe(78498); // Source - http://www.mathematical.com/primes0to1000k.html
            arrayResult.Value[0].ShouldBeOfType<NumericValue>().Value.ShouldBe(2);
            arrayResult.Value[78497].ShouldBeOfType<NumericValue>().Value.ShouldBe(999983);
        }

        [Fact]
        public void PrimeFactorisation_should_correctly_factorise_non_negative_integrals()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10); // Normal composite
            var mockProgramState2 = MockFactory.MockProgramState(13); // Normal prime
            var mockProgramState3 = MockFactory.MockProgramState(123456); // Composite larger than sqrt(limit)
            var mockProgramState4 = MockFactory.MockProgramState(123457); // Prime larger than sqrt(limit)
            var mockProgramState5 = MockFactory.MockProgramState(1234567); // Composite larger than limit
            var mockProgramState6 = MockFactory.MockProgramState(1234577); // Prime larger than limit
            var mockProgramState7 = MockFactory.MockProgramState(0);
            var mockProgramState8 = MockFactory.MockProgramState(1);

            var token = new PrimeFactorisation();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);
            var result6 = token.Evaluate(mockProgramState6.Object);
            var result7 = token.Evaluate(mockProgramState7.Object);
            var result8 = token.Evaluate(mockProgramState8.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(2, 5).End();
            result2.ShouldBeArrayWhichStartsWith(13).End();
            result3.ShouldBeArrayWhichStartsWith(2, 2, 2, 2, 2, 2, 3, 643).End();
            result4.ShouldBeArrayWhichStartsWith(123457).End();
            result5.ShouldBeArrayWhichStartsWith(127, 9721).End();
            result6.ShouldBeArrayWhichStartsWith(1234577).End();
            result7.ShouldBeEmptyArray();
            result8.ShouldBeEmptyArray();
        }

        [Fact]
        public void PrimeFactorisation_should_error_on_floats_and_negative_integrals()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(1.5);
            var mockProgramState2 = MockFactory.MockProgramState(-10);

            var token = new PrimeFactorisation();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("K not defined for non-integrals - arg=1.5");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("K not defined for negative integrals - arg=-10");
        }

        [Fact]
        public void PrimeFactorisation_should_error_on_non_numerics()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abc");
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new PrimeFactorisation();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument type passed to K command - String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument type passed to K command - Array");
        }

        [Fact]
        public void IsPrime_should_correctly_identify_non_negative_integrals()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(10); // Normal composite
            var mockProgramState2 = MockFactory.MockProgramState(13); // Normal prime
            var mockProgramState3 = MockFactory.MockProgramState(123456); // Composite larger than sqrt(limit)
            var mockProgramState4 = MockFactory.MockProgramState(123457); // Prime larger than sqrt(limit)
            var mockProgramState5 = MockFactory.MockProgramState(1234567); // Composite larger than limit
            var mockProgramState6 = MockFactory.MockProgramState(1234577); // Prime larger than limit
            var mockProgramState7 = MockFactory.MockProgramState(0);
            var mockProgramState8 = MockFactory.MockProgramState(1);

            var token = new IsPrime();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);
            var result5 = token.Evaluate(mockProgramState5.Object);
            var result6 = token.Evaluate(mockProgramState6.Object);
            var result7 = token.Evaluate(mockProgramState7.Object);
            var result8 = token.Evaluate(mockProgramState8.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result5.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result6.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result7.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result8.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void IsPrime_should_error_on_floats_and_negative_integrals()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(1.5);
            var mockProgramState2 = MockFactory.MockProgramState(-10);

            var token = new IsPrime();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("\u1E32 not defined for non-integrals - arg=1.5");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("\u1E32 not defined for negative integrals - arg=-10");
        }

        [Fact]
        public void IsPrime_should_error_on_non_numerics()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abc");
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new IsPrime();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument type passed to \u1E32 command - String");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument type passed to \u1E32 command - Array");
        }

        [Fact]
        public void Palindromise_PrimesList_should_return_primes_list_for_numerics()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(20);
            var mockProgramState2 = MockFactory.MockProgramState(20.5);
            var mockProgramState3 = MockFactory.MockProgramState(2);
            var mockProgramState4 = MockFactory.MockProgramState(-10);

            var token = new Palindromise_PrimesList();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);
            var result3 = token.Evaluate(mockProgramState3.Object);
            var result4 = token.Evaluate(mockProgramState4.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(2, 3, 5, 7, 11, 13, 17, 19).End();
            result2.ShouldBeArrayWhichStartsWith(2, 3, 5, 7, 11, 13, 17, 19).End();
            result3.ShouldBeEmptyArray();
            result4.ShouldBeEmptyArray();
        }

        [Fact]
        public void Palindromise_PrimesList_should_return_palindromised_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState("abcde");
            var mockProgramState2 = MockFactory.MockProgramState("");

            var token = new Palindromise_PrimesList();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeOfType<StringValue>().Value.ShouldBe("abcdedcba");
            result2.ShouldBeOfType<StringValue>().Value.ShouldBe("");
        }

        [Fact]
        public void Palindromise_PrimesList_should_return_palindromised_array()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty);

            var token = new Palindromise_PrimesList();

            // Act
            var result1 = token.Evaluate(mockProgramState1.Object);
            var result2 = token.Evaluate(mockProgramState2.Object);

            // Assert
            result1.ShouldBeArrayWhichStartsWith(1, 2, 3, 2, 1);
            result2.ShouldBeEmptyArray();
        }
    }
}

