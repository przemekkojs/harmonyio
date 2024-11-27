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
        private static Solution GetSolution(string solutionString, string problemString)
        {
            var parsedSolutionResult = Algorithm.New.Algorithm.Parsers.SolutionParser.Parser
                .ParseJsonToSolutionParseResult(solutionString);

            var parsedProblemResult = Algorithm.New.Algorithm.Parsers.ProblemParser.Parser
                .ParseJsonToProblem(problemString);

            var parsedSolution = new Solution(parsedProblemResult, parsedSolutionResult.Stacks);

            return parsedSolution;
        }

        private static readonly Solution solutionNofunctions = GetSolution(Constants.SOLUTION_STRING_EMPTY, Constants.PROBLEM_STRING_EMPTY);
        private static readonly Solution solution2functions = GetSolution(Constants.SOLUTION_STRING_2_FUNCTIONS, Constants.PROBLEM_STRING_2_FUNCTIONS);
        private static readonly Solution solution4functions = GetSolution(Constants.SOLUTION_STRING_4_FUNCTIONS, Constants.PROBLEM_STRING_4_FUNCTIONS);
        private static readonly Solution solution8functions = GetSolution(Constants.SOLUTION_STRING_8_FUNCTIONS, Constants.PROBLEM_STRING_8_FUNCTIONS);
        private static readonly Solution solution16functions = GetSolution(Constants.SOLUTION_STRING_16_FUNCTIONS, Constants.PROBLEM_STRING_16_FUNCTIONS);

        [Fact]
        public void CheckNoMistakesNoFunctions()
        {
            var settings = Constants.Settings;
            var checkResult = SolutionChecker.CheckSolution(solutionNofunctions, settings);

            var mistakesCount = checkResult?.Count > 0 ?
                checkResult.Sum(x => x.Quantity) :
                0;

            Assert.Equal(0, mistakesCount);
        }

        [Fact]
        public void CheckNoMistakes2Functions()
        {
            var settings = Constants.Settings;
            var checkResult = SolutionChecker.CheckSolution(solution2functions, settings);

            var mistakesCount = checkResult.Count > 0 ?
                checkResult.Sum(x => x.Quantity) :
                0;

            Assert.Equal(0, mistakesCount);
        }

        [Fact]
        public void CheckNoMistakes4Functions()
        {
            var settings = Constants.Settings;
            var checkResult = SolutionChecker.CheckSolution(solution4functions, settings);

            var mistakesCount = checkResult.Count > 0 ?
                checkResult.Sum(x => x.Quantity) :
                0;

            Assert.Equal(0, mistakesCount);
        }

        [Fact]
        public void CheckNoMistakes8Functions()
        {
            var settings = Constants.Settings;
            var checkResult = SolutionChecker.CheckSolution(solution8functions, settings);

            var mistakesCount = checkResult.Count > 0 ?
                checkResult.Sum(x => x.Quantity) :
                0;

            Assert.Equal(0, mistakesCount);
        }

        [Fact]
        public void CheckNoMistakes16Functions()
        {
            var settings = Constants.Settings;
            var checkResult = SolutionChecker.CheckSolution(solution16functions, settings);

            var mistakesCount = checkResult.Count > 0 ?
                checkResult.Sum(x => x.Quantity) :
                0;

            Assert.Equal(0, mistakesCount);
        }

        //[Fact]
        //public void Check1Mistake()
        //{

        //}
    }
}
