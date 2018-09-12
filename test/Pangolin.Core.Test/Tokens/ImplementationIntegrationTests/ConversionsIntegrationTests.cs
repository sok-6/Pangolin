using Pangolin.Core.TokenImplementations;
using Xunit;

namespace Pangolin.Core.Test.Tokens.ImplementationIntegrationTests
{
    public class ConversionsIntegrationTests
    {
        [Fact]
        public void ArrayConstruction_should_build_array_from_literals()
        {
            // Arrange
            var programState = new ProgramState(
                new DataValue[]
                {

                },
                new Token[]
                {
                    new ArrayConstruction(),
                    new NumericLiteral(3),
                    new NumericLiteral(1),
                    new NumericLiteral(2)
                });

            // Act
            var result = programState.DequeueAndEvaluate();

            // Assert
            result.ShouldBeArrayWhichStartsWith(3, 1, 2).End();
        }
    }
}
