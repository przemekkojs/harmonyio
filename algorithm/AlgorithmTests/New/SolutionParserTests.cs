using Algorithm.New;
using Algorithm.New.Algorithm;
using Algorithm.New.Music;
using Index = Algorithm.New.Music.Index;

namespace AlgorithmTests.New
{
    public class SolutionParserTests
    {
        [Fact]
        public void CheckParsing()
        {
            var metre = Metre.Meter2_4;
            var tonation = Tonation.CMajor;

            var functions = new List<Function>()
            {
                new(new Index() { Bar = 0, Position = 0, Duration = 4 }, Symbol.T, false, tonation),
                new(new Index() { Bar = 0, Position = 1, Duration = 4 }, Symbol.S, false, tonation),
                new(new Index() { Bar = 1, Position = 0, Duration = 4 }, Symbol.D, false, tonation),
                new(new Index() { Bar = 1, Position = 1, Duration = 4 }, Symbol.T, false, tonation)
            };

            var problem = new Problem(functions, metre, tonation);

            var stacks = new List<Stack>()
            {
                new (new Index() { Bar = 0, Position = 0, Duration = 4 }, ["C", "E", "G", "C"]),
                new (new Index() { Bar = 0, Position = 1, Duration = 4 }, ["C", "F", "A", "C"]),
                new (new Index() { Bar = 1, Position = 0, Duration = 4 }, ["B", "G", "G", "D"]),
                new (new Index() { Bar = 1, Position = 1, Duration = 4 }, ["C", "E", "G", "C"])
            };

            var solution = new Solution(problem, stacks);

            var solutionString = Constants.SOLUTION_STRING_4_FUNCTIONS;
            var problemString = Constants.PROBLEM_STRING_4_FUNCTIONS;

            var parsedSolutionResult = Algorithm.New.Algorithm.Parsers.SolutionParser.Parser.ParseJsonToSolutionParseResult(solutionString);
            var parsedProblemResult = Algorithm.New.Algorithm.Parsers.ProblemParser.Parser.ParseJsonToProblem(problemString);

            var parsedSolution = new Solution(parsedProblemResult, parsedSolutionResult.Stacks);

            Assert.Equal(solution, parsedSolution);
        }
    }
}
