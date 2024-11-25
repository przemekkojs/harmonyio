using Algorithm.New;
using Algorithm.New.Algorithm.Generators;
using Algorithm.New.Algorithm.Parsers.ProblemParser;

namespace AlgorithmTests.New
{
    public class SolutionGeneratorTests
    {
        [Fact]
        public void Test4Functions()
        {
            var problem = Parser.ParseJsonToProblem(Constants.PROBLEM_STRING_4_FUNCTIONS);
            var solution = SolutionGenerator.Generate(problem);

            Assert.True(solution.Count == 4);
        }
    }
}
