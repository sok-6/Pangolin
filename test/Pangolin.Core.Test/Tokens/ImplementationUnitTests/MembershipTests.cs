using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Pangolin.Core.TokenImplementations;
using Shouldly;
using Pangolin.Core.DataValueImplementations;
using Pangolin.Common;

namespace Pangolin.Core.Test.Tokens.ImplementationUnitTests
{
    public class MembershipTests
    {
        [Fact]
        public void Membership_should_check_array_membership()
        {
            // Arrange
            var ps1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(2).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());
            var ps2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(5).Object,
                MockFactory.MockArrayBuilder.StartingNumerics(1, 2, 3).Complete());

            var token = new Membership();

            // Act
            var result1 = token.Evaluate(ps1.Object);
            var result2 = token.Evaluate(ps2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Membership_should_check_substring()
        {
            // Arrange
            var ps1 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(12).Object,
                MockFactory.MockStringValue("abc123").Object);
            var ps2 = MockFactory.MockProgramState(
                MockFactory.MockNumericValue(1.2).Object,
                MockFactory.MockStringValue("abc123").Object);
            var ps3 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("c1").Object,
                MockFactory.MockStringValue("abc123").Object);
            var ps4 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("d").Object,
                MockFactory.MockStringValue("abc123").Object);

            var token = new Membership();

            // Act
            var result1 = token.Evaluate(ps1.Object);
            var result2 = token.Evaluate(ps2.Object);
            var result3 = token.Evaluate(ps3.Object);
            var result4 = token.Evaluate(ps4.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
            result3.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result4.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Membership_should_check_divisors()
        {
            // Arrange
            var ps1 = MockFactory.MockProgramState(3, 12);
            var ps2 = MockFactory.MockProgramState(5, 12);

            var token = new Membership();

            // Act
            var result1 = token.Evaluate(ps1.Object);
            var result2 = token.Evaluate(ps2.Object);

            // Assert
            result1.ShouldBeOfType<NumericValue>().Value.ShouldBe(1);
            result2.ShouldBeOfType<NumericValue>().Value.ShouldBe(0);
        }

        [Fact]
        public void Membership_should_throw_type_errors_with_invalid_type_pairs()
        {
            // Arrange
            var ps1 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.Empty,
                MockFactory.MockNumericValue(0).Object);
            var ps2 = MockFactory.MockProgramState(
                MockFactory.MockStringValue("").Object,
                MockFactory.MockNumericValue(0).Object);
            var ps3 = MockFactory.MockProgramState(
                MockFactory.MockArrayBuilder.Empty,
                MockFactory.MockStringValue("").Object);

            var token = new Membership();

            // Act/Assert
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps1.Object)).Message.ShouldBe("Invalid argument types passed to \u2208 command - Array,Numeric");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps2.Object)).Message.ShouldBe("Invalid argument types passed to \u2208 command - String,Numeric");
            Should.Throw<PangolinInvalidArgumentTypeException>(() => token.Evaluate(ps3.Object)).Message.ShouldBe("Invalid argument types passed to \u2208 command - Array,String");
        }
    }
}
