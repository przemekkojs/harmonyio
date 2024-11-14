using Algorithm.New;
using Algorithm.New.Algorithm;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Algorithm.Mistake.Solution;
using Algorithm.New.Music;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Main.GradingAlgorithm
{
    public class GradingAlgorithm : IGradingAlgorithm
    {
        public (int, int, string) Grade(string question, string answer, int maxPoints)
        {            
            var solutionParseResult = Algorithm.New.Algorithm.Parsers.SolutionParser.Parser.ParseJsonToSolutionParseResult(answer);
            var problem = Algorithm.New.Algorithm.Parsers.ProblemParser.Parser.ParseJsonToProblem(question);
            var solution = new Solution(problem, solutionParseResult.Stacks);
            var settings = Constants.Settings;

            PopulateEmptyStacks(problem, solution);

            var checkResult = SolutionChecker
                .CheckSolution(solution, settings)
                .Distinct()
                .ToList();

            var maxMistakesCount = GetMaxMistakesCount(problem, settings);
            var opinion = GenerateOpinion2(checkResult);
            var mistakesCount = checkResult.Sum(x => x.Quantity);

            var algorithmPoints = maxMistakesCount - mistakesCount > 0 ? maxMistakesCount - mistakesCount : 0;
            var pointsPercent = DivAsPercentage(algorithmPoints, maxMistakesCount);
            var points = Convert.ToInt32(pointsPercent * maxPoints / 100);

            return (points, maxPoints, opinion);
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
            maxMistakesCount += twoFunctionSettingsCount * functionsCount - functionsCount;
            // -functionsCount, ponieważ ostatniej nie uwzględniamy

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

        private static string GenerateOpinion2(List<Mistake> mistakes)
        {
            // Takty, funkcje w taktach, ID błędu
            var resultList = new List<List<List<int>>>();

            foreach (var mistake in mistakes)
            {
                var barIndexes = mistake.Description.Item1;
                var functionIndexes = mistake.Description.Item2;
                var code = mistake.MistakeCode;

                var bar1 = barIndexes.Count > 0 ? barIndexes[0] : -1;
                var bar2 = barIndexes.Count > 1 ? barIndexes[1] : bar1;

                var function1 = functionIndexes.Count > 0 ? functionIndexes[0] : -1;
                var function2 = functionIndexes.Count > 1 ? functionIndexes[1] : function1;

                List<List<int>> toAdd = [[bar1, bar2], [function1, function2], [code]];
                resultList.Add(toAdd);
            }

            var jsonResult = JsonConvert.SerializeObject(resultList);
            return jsonResult;
        }

        private static string CodeToMistake(int code)
        {
            string result = "Nieokreślony błąd.";

            return result;
        }

        public static string OpinionToDescription(string opinionString)
        {
            List<List<List<int>>>? serialized;

            try
            {
                serialized = JsonConvert.DeserializeObject<List<List<List<int>>>>(opinionString);
            }
            catch (Exception)
            {
                return "Coś poszło nie tak...";
            }

            if (serialized == null)
                return "Coś poszło nie tak...";

            var tmp = new Dictionary<(int, (int, int, int)), List<string>>();

            foreach (var item in serialized)
            {
                var barIndexes = item[0];
                var functionIndexes = item[1];
                var description = CodeToMistake(item[2][0]);

                var bar1 = barIndexes.Count > 0 ? barIndexes[0] : -1;
                var bar2 = barIndexes.Count > 1 ? barIndexes[1] : bar1;

                var function1 = functionIndexes.Count > 0 ? functionIndexes[0] : -1;
                var function2 = functionIndexes.Count > 1 ? functionIndexes[1] : function1;

                var key = (bar1, (function1, function2, bar2));

                if (!tmp.ContainsKey(key))
                    tmp[key] = [];

                tmp[key].Add(description);
            }

            var sortedKeys = tmp.Keys
                .OrderBy(key => key.Item1)
                    .ThenBy(key => key.Item2.Item1)
                        .ThenBy(key => key.Item2.Item2)
                            .ThenBy(key => key.Item2.Item3)
                .ToList();

            int lastBar = 0;
            var opinion = "";

            foreach (var key in sortedKeys)
            {
                var bar = key.Item1 + 1;

                var function1 = key.Item2.Item1 + 1;
                var function2 = key.Item2.Item2 + 1;
                var bar2 = key.Item2.Item3 + 1;

                if (bar != lastBar)
                {
                    if (lastBar != 0)
                        opinion += $"</details>";

                    lastBar = bar;

                    opinion += $"<details><summary>Takt {bar}</summary>";
                }

                if (function1 == function2)
                {
                    opinion += $"<details><summary>Funkcja na miarę {function1}</summary>";
                }
                else
                {
                    if (bar2 != bar)
                    {
                        opinion += $"<details><summary>Funkcje na miary {function1}, {function2} w takcie {bar2})</summary>";
                    }
                    else
                    {
                        opinion += $"<details><summary>Funkcje na miary {function1}, {function2}</summary>";
                    }
                }

                foreach (var o in tmp[key])
                {
                    opinion += $"<span>{o}</span><br>";
                }

                opinion += "</details>";
            }

            return opinion;
        }

        private static string GenerateOpinion(List<Mistake> mistakes)
        {
            var tmp = new Dictionary<(int, (int, int, int)), List<string>>();

            foreach (var mistake in mistakes)
            {
                var barIndexes = mistake.Description.Item1;
                var functionIndexes = mistake.Description.Item2;
                var description = mistake.Description.Item3;

                var bar1 = barIndexes.Count > 0 ? barIndexes[0] : -1;
                var bar2 = barIndexes.Count > 1 ? barIndexes[1] : bar1;

                var function1 = functionIndexes.Count > 0 ? functionIndexes[0] : -1;
                var function2 = functionIndexes.Count > 1 ? functionIndexes[1] : function1;

                var key = (bar1, (function1, function2, bar2));

                if (!tmp.ContainsKey(key))
                    tmp[key] = [];

                tmp[key].Add(description);
            }

            var sortedKeys = tmp.Keys
                .OrderBy(key => key.Item1)
                    .ThenBy(key => key.Item2.Item1)
                        .ThenBy(key => key.Item2.Item2)
                            .ThenBy(key => key.Item2.Item3)
                .ToList();

            int lastBar = 0;
            var opinion = "";

            foreach (var key in sortedKeys)
            {
                var bar = key.Item1 + 1;

                var function1 = key.Item2.Item1 + 1;
                var function2 = key.Item2.Item2 + 1;
                var bar2 = key.Item2.Item3 + 1;

                if (bar != lastBar)
                {
                    if (lastBar != 0)
                        opinion += $"</details>";

                    lastBar = bar;

                    opinion += $"<details><summary>Takt {bar}</summary>";
                }                

                if (function1 == function2)
                {
                    opinion += $"<details><summary>Funkcja na miarę {function1}</summary>";
                }
                else
                {
                    if (bar2 != bar)
                    {
                        opinion += $"<details><summary>Funkcje na miary {function1}, {function2} w takcie {bar2})</summary>";
                    }
                    else
                    {
                        opinion += $"<details><summary>Funkcje na miary {function1}, {function2}</summary>";
                    }
                }

                foreach (var o in tmp[key])
                {
                    opinion += $"<span>{o}</span><br>";
                }

                opinion += "</details>";
            }

            return opinion;
        }
    }
}
