using Pangolin.Core.DataValueImplementations;
using Pangolin.Core.TokenImplementations;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Pangolin.Core.Test.Tokens.Implementations
{
    public class LiteralsTests
    {
        [Fact]
        public void NumericLiteral_should_evaluate_to_NumericValue_with_correct_contents()
        {
            // Arrange
            var token = new NumericLiteral(1.5);

            // Act
            var result = token.Evaluate(null);

            // Assert
            result.ShouldBeOfType(typeof(NumericValue));
            result.ToString().ShouldBe("1.5");
        }

        [Fact]
        public void StringLiteral_should_evaluate_to_StringValue_with_correct_contents()
        {
            // Arrange
            var token = new StringLiteral("abc");

            // Act
            var result = token.Evaluate(null);

            // Assert
            result.ShouldBeOfType(typeof(StringValue));
            result.ToString().ShouldBe("abc");
        }
    }
}
