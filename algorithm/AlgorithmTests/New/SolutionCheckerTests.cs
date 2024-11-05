using Algorithm.New;
using Algorithm.New.Algorithm;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Algorithm.Rules;
using System.Reflection.Metadata;

namespace AlgorithmTests.New
{
    /* IMPORTANT!
     * These tests work only, if CheckParsing() test from SolutionParserTests goes through!
     * Otherwise, you should not trust its results.
    */
    public class SolutionCheckerTests
    {
        private static Solution GetSolution()
        {
            var solutionString = Constants.SOLUTION_STRING;
            var problemString = Constants.PROBLEM_STRING;

            var parsedSolutionResult = Algorithm.New.Algorithm.Parsers.SolutionParser.Parser.ParseJsonToSolutionParseResult(solutionString);
            var parsedProblemResult = Algorithm.New.Algorithm.Parsers.ProblemParser.Parser.ParseJsonToProblem(problemString);

            var parsedSolution = new Solution(parsedProblemResult, parsedSolutionResult.Stacks);

            return parsedSolution;
        }

        private static readonly Solution solution = GetSolution();

        [Fact]
        public void CheckNoMistakes()
        {
            var settings = new Settings([new VoiceCrossing()]);
            var checkResult = SolutionChecker.CheckSolution(solution, settings);

            var mistakesCount = checkResult
                .Sum(x => x.Quantity);

            Assert.Equal(0, mistakesCount);
        }

        [Fact]
        public void Check1Mistake()
        {

        }
    }
}
