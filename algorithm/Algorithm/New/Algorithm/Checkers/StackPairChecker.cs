using Algorithm.New.Algorithm.Mistake.Solution;
using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Checkers
{
    public static class StackPairChecker
    {
        public static List<Mistake.Solution.Mistake> CheckRules(Stack stack1, Stack stack2, Settings settings)
        {
            List<Mistake.Solution.Mistake> result = [];
            var rules = settings.ActiveRules;

            foreach (var rule in rules)
            {
                if (rule.OneFunction)
                {
                    var satisfied1 = rule.IsSatisfied(stacks: [stack1]);
                    var satisfied2 = rule.IsSatisfied(stacks: [stack2]);

                    if (!satisfied1)
                    {
                        var toAdd = new StackMistake([stack1], rule);
                        result.Add(toAdd);
                    }

                    if (!satisfied2)
                    {
                        var toAdd = new StackMistake([stack2], rule);
                        result.Add(toAdd);
                    }
                }
                else
                {
                    var satisfied = rule.IsSatisfied(stacks: [stack1, stack2]);

                    if (!satisfied)
                    {
                        var toAdd = new StackMistake([stack1, stack2], rule);
                        result.Add(toAdd);
                    }
                }
            }

            return result;
        }
    }
}
