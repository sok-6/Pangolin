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
    public class InterpolationTests
    {
        [Fact]
        public void Interpolation_should_insert_correctly_indexed_array_values()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState(
                MockFactory.MockStringValue("test\u24EAtest\u2460test\u24EAtest").Object,
                MockFactory.MockArrayBuilder.StartingNumerics(4.5).WithStrings("abc").Complete());

            var token = new Interpolation();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("test4.5testabctest4.5test");
        }

        [Fact]
        public void Interpolation_should_insert_use_single_value_repeatedly_if_not_array()
        {
            // Arrange
            var mockProgramState = MockFactory.MockProgramState("test\u24EAtest\u2460test\u24EAtest", "abc");

            var token = new Interpolation();

            // Act
            var result = token.Evaluate(mockProgramState.Object);

            // Assert
            result.ShouldBeOfType<StringValue>().Value.ShouldBe("testabctestabctestabctest");
        }

        [Fact]
        public void Interpolation_should_throw_exception_when_1st_argument_not_string()
        {
            // Arrange
            var mockProgramState1 = MockFactory.MockProgramState(0, 0);
            var mockProgramState2 = MockFactory.MockProgramState(MockFactory.MockArrayBuilder.Empty, MockFactory.Zero);

            var token = new Interpolation();

            // Act / Assert
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState1.Object)).Message.ShouldBe("Invalid argument types passed to $ command - Numeric,Numeric");
            Should.Throw<PangolinException>(() => token.Evaluate(mockProgramState2.Object)).Message.ShouldBe("Invalid argument types passed to $ command - Array,Numeric");
        }

    }
}
