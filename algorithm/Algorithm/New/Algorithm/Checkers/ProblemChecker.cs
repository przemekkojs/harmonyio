using Algorithm.New.Algorithm.Mistake.Problem;
using Algorithm.New.Algorithm.Rules.Problem;
using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Checkers
{
    public static class ProblemChecker
    {        
        public static List<ProblemMistake> CheckProblem(Problem problem)
        {
            var result = new List<ProblemMistake>();
            var problemFunctions = problem.Functions;

            foreach (var rule in Constants.ProblemSettings)
            {
                if (rule.OneFunction)
                    CheckSingleFunctionRules(rule, problemFunctions, result);
                else
                    CheckConsecutiveFunctionRules(rule, problemFunctions, result);
            }

            return result;
        }

        private static void CheckSingleFunctionRules(Rule rule, List<Function> functions, List<ProblemMistake> result)
        {
            var functionsCount = functions.Count;

            for (int index = 0; index < functionsCount; index++)
            {
                var function = functions[index];

                if (!rule.IsSatisfied(function))
                    AddProblemMistake(result, rule, function.Index.Bar, function.Index.Position);
            }
        }

        private static void CheckConsecutiveFunctionRules(Rule rule, List<Function> functions, List<ProblemMistake> result)
        {
            var functionsCount = functions.Count;

            for (int index = 0; index < functionsCount - 1; index++)
            {
                var function1 = functions[index];
                var function2 = functions[index + 1];

                if (!rule.IsSatisfied(function1, function2))
                    AddProblemMistake(result, rule, function1.Index.Bar, function1.Index.Position);
            }
        }

        private static void AddProblemMistake(List<ProblemMistake> result, Rule rule, int bar, int position)
        {
            var mistake = new ProblemMistake(bar, position);
            mistake.GenerateDescription(rule);
            result.Add(mistake);
        }
    }
}
