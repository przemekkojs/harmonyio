using Algorithm.New;
using Algorithm.New.Algorithm;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Algorithm.Mistake.Solution;
using Algorithm.New.Music;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Newtonsoft.Json;

namespace Main.GradingAlgorithm
{
    public class GradingAlgorithm : IGradingAlgorithm
    {
        public (int, int, Dictionary<(int, (int, int, int)), List<int>>) Grade(string question, string answer, int maxPoints)
        {
            var questionEmpty = question == string.Empty;
            var answerEmpty = answer == string.Empty;

            if (questionEmpty || answerEmpty)
                return (0, maxPoints, []);

            var solutionParseResult = Algorithm.New.Algorithm.Parsers.SolutionParser.Parser
                .ParseJsonToSolutionParseResult(answer);

            var problem = Algorithm.New.Algorithm.Parsers.ProblemParser.Parser
                .ParseJsonToProblem(question);

            var solution = new Solution(problem, solutionParseResult.Stacks);
            var settings = Constants.Settings;

            PopulateEmptyStacks(problem, solution);

            var checkResult = SolutionChecker
                .CheckSolution(solution, settings);

            var maxMistakesCount = GetMaxMistakesCount(problem, settings);
            var opinion = GenerateOpinion(checkResult);
            var mistakesCount = checkResult.Sum(x => x.Quantity);

            var algorithmPoints = maxMistakesCount - mistakesCount > 0 ? maxMistakesCount - mistakesCount : 0;
            var pointsPercent = DivAsPercentage(algorithmPoints, maxMistakesCount);
            var points = Convert.ToInt32(pointsPercent * maxPoints / 100);

            var emptyCount = 0;
            var stacksCount = solutionParseResult.Stacks.Count;

            foreach (var stack in solutionParseResult.Stacks)
            {
                var innerCount = 0;

                foreach (var note in stack.Notes)
                {
                    if (note == null)
                        innerCount++;
                }

                if (innerCount == 4)
                    emptyCount++;
            }

            if (emptyCount == stacksCount)
                return (0, maxPoints, opinion);

            return (points, maxPoints, opinion);
        }

        private static float DivAsPercentage(int numerator, int denominator)
        {
            if (denominator == 0)
                throw new DivideByZeroException("Nie można dzielić przez 0.");

            float result = (float)numerator / denominator * 100;
            return result;
        }

        private static int GetMaxMistakesCount(Problem problem, Settings settings)
        {            
            var functionsCount = problem.Functions.Count;
            var maxMistakesCount = functionsCount * 4;

            var oneFunctionSettingsCount = settings.ActiveRules
                .Where(s => s.OneFunction == true)
                .Count();

            var twoFunctionSettingsCount = settings.ActiveRules
                .Where(s => s.OneFunction == false)
                .Count();

            maxMistakesCount += oneFunctionSettingsCount * functionsCount;
            maxMistakesCount += twoFunctionSettingsCount * functionsCount - functionsCount; // -functionsCount, ponieważ ostatniej nie uwzględniamy

            return maxMistakesCount;
        }

        private static void PopulateEmptyStacks(Problem problem, Solution solution)
        {
            Dictionary<(int, int), Stack> tmpStacksMap = [];

            foreach (var stack in solution.Stacks)
            {
                var key = (stack.Index.Bar, stack.Index.Position);
                tmpStacksMap[key] = stack;
            }

            foreach (var function in problem.Functions)
            {
                var key = (function.Index.Bar, function.Index.Position);
                if (!tmpStacksMap.ContainsKey(key))
                    tmpStacksMap[key] = new Stack(
                        index: new Algorithm.New.Music.Index()
                        {
                            Bar = function.Index.Bar,
                            Position = function.Index.Position,
                            Duration = function.Index.Duration
                        },
                        notes: [null, null, null, null]
                    );
            }

            List<Stack> tmpStacks = [.. tmpStacksMap.Values];
            solution.Stacks.Clear();
            solution.Stacks.AddRange(tmpStacks);
        }

        private static Dictionary<(int, (int, int, int)), List<int>> GenerateOpinion(List<Mistake> mistakes)
        {
            // Takty, funkcje w taktach, ID błędu
            var tmp = new Dictionary<(int, (int, int, int)), List<int>>();

            foreach (var mistake in mistakes)
            {
                var barIndexes = mistake.Description.Item1;
                var functionIndexes = mistake.Description.Item2;
                var description = mistake.MistakeCode;

                var bar1 = barIndexes.Count > 0 ? barIndexes[0] : -1;
                var bar2 = barIndexes.Count > 1 ? barIndexes[1] : bar1;

                var function1 = functionIndexes.Count > 0 ? functionIndexes[0] : -1;
                var function2 = functionIndexes.Count > 1 ? functionIndexes[1] : function1;

                var key = (bar1, (function1, function2, bar2));

                if (!tmp.ContainsKey(key))
                    tmp[key] = [];

                tmp[key].Add(description);
            }

            return tmp;
        }
    }
}
