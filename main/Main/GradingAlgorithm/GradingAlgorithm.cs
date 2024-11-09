using Algorithm.New.Algorithm;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Algorithm.Rules;

namespace Main.GradingAlgorithm
{
    public class GradingAlgorithm : IGradingAlgorithm
    {
        public (int, int, string) Grade(string question, string answer)
        {
            var problem = Algorithm.New.Algorithm.Parsers.ProblemParser.Parser.ParseJsonToProblem(question);
            var solutionParseResult = Algorithm.New.Algorithm.Parsers.SolutionParser.Parser.ParseJsonToSolutionParseResult(answer);

            var solution = new Solution(problem, solutionParseResult.Stacks);

            // TODO: Make some constant rules and rules list
            var settings = new Settings([
                new VoiceCrossing()
            ]);

            var checkResult = SolutionChecker.CheckSolution(solution, settings);
            checkResult = checkResult
                .Distinct()
                .ToList();            

            var tmp = new Dictionary<((int, int), (int, int)), string>();

            foreach (var mistake in checkResult)
            {
                var barIndexes = mistake.Description.Item1;
                var functionIndexes = mistake.Description.Item2;
                var description = mistake.Description.Item3;

                var bar1 = barIndexes.Count > 0 ? barIndexes[0] : -1;
                var bar2 = barIndexes.Count > 1 ? barIndexes[1] : bar1;

                var function1 = functionIndexes.Count > 0 ? functionIndexes[0] : -1;
                var function2 = functionIndexes.Count > 1 ? functionIndexes[1] : function1;

                tmp[((bar1, bar2), (function1, function2))] = description;
            }

            var sortedKeys = tmp.Keys
                .OrderBy(key => key.Item1.Item1)
                .ThenBy(key => key.Item1.Item2)
                .ThenBy(key => key.Item2.Item1)
                .ThenBy(key => key.Item2.Item2)
                .ToList();

            // TODO: Co jeśli funkcje zaczynają się od np. 2 taktu, a nie 1.
            int lastBar = 0;

            // TODO: Zrobić to potem na HTML-a
            var opinion = "";

            foreach (var key in sortedKeys)
            {
                var bar1 = key.Item1.Item1 + 1;
                var bar2 = key.Item1.Item2 + 1;

                var function1 = key.Item2.Item1 + 1;
                var function2 = key.Item2.Item2 + 1;
                
                if (bar1 != lastBar)
                {
                    lastBar = bar1;

                    if (bar1 == bar2)
                    {
                        opinion += $"Takt {bar1}:\n<br>";                        
                    }
                }

                if (bar1 != bar2)
                {
                    opinion += $"Takty {bar1}, {bar2}:\n<br>";
                }

                if (function1 == function2)
                {
                    opinion += $"   Funkcja na miarę {function1}:\n<br>";
                }
                else
                {
                    if (bar1 != bar2)
                        opinion += $"   Funkcje na miary {function1} (takt {bar1}), {function2} (takt {bar2}):\n<br>";
                    else
                        opinion += $"   Funkcje na miary {function1}, {function2}:\n<br>";
                }

                opinion += "        " + tmp[key] + "\n<br>";
            }

            var maxMistakesCount = solution.Stacks.Count * 4;

            foreach (var setting in settings.ActiveRules)
            {
                maxMistakesCount += (setting.OneFunction ? 1 : 2) * solution.Stacks.Count;
            }

            var mistakesCount = checkResult.Sum(x => x.Quantity);

            var maxPoints = 10;
            var pointsPercent = DivAsPercentage(maxMistakesCount - mistakesCount, maxMistakesCount);
            var points = Convert.ToInt32(pointsPercent / maxPoints);

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
    }
}
