using Algorithm.New.Algorithm;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Algorithm.Rules;

namespace Main.GradingAlgorithm
{
    public class GradingAlgorithm : IGradingAlgorithm
    {
        public (int, int) Grade(string question, string answer)
        {
            var problem = Algorithm.New.Algorithm.Parsers.ProblemParser.Parser.ParseJsonToProblem(question);
            var solutionParseResult = Algorithm.New.Algorithm.Parsers.SolutionParser.Parser.ParseJsonToSolutionParseResult(answer);

            var solution = new Solution(problem, solutionParseResult.Stacks);

            // TODO: Make some constant rules and rules list
            var settings = new Settings([
                new VoiceCrossing()
            ]);

            var checkResult = SolutionChecker.CheckSolution(solution, settings);

            var maxMistakesCount = solution.Stacks.Count * 4 * settings.ActiveRules.Count;
            var mistakesCount = checkResult.Sum(x => x.Quantity);

            var maxPoints = 10;
            var pointsPercent = DivAsPercentage(maxMistakesCount - mistakesCount, maxMistakesCount);
            var points = Convert.ToInt32(pointsPercent / maxPoints);

            return (points, maxPoints);
        }

        private static float DivAsPercentage(int numerator, int denominator)
        {
            if (denominator == 0)
            {
                throw new DivideByZeroException("Denominator cannot be zero.");
            }

            float result = (float)numerator / denominator * 100;
            return result;
        }
    }
}
