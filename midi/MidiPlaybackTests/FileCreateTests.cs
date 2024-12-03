using Algorithm.New;
using Algorithm.New.Algorithm;
using Algorithm.New.Algorithm.Parsers.ProblemParser;
using Algorithm.New.Algorithm.Parsers.SolutionParser;
using MidiPlayback;

namespace MidiPlaybackTests
{
    public class FileCreateTests
    {
        [Fact]
        public void Test1()
        {
            var solutionString = Constants.SOLUTION_STRING_4_FUNCTIONS;
            var problemString = Constants.PROBLEM_STRING_4_FUNCTIONS;

            var problem = Algorithm.New.Algorithm.Parsers.ProblemParser.Parser
                .ParseJsonToProblem(problemString);

            var solutionParseResult = Algorithm.New.Algorithm.Parsers.SolutionParser.Parser
                .ParseJsonToSolutionParseResult(solutionString);

            var stacks = solutionParseResult.Stacks;
            var solution = new Solution(problem, stacks);

            var create = FileCreator.Create(solution);
            Assert.True(create.Length > 0);
        }
    }
}
