using Algorithm.New.Algorithm.Mistake.Solution;
using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Checkers
{
    public static class StackPairChecker
    {
        public static List<Mistake.Solution.Mistake> CheckRules(
            List<Function> functions, List<Stack> stacks, Settings settings)
        {
            if (functions.Count == 0)
                throw new ArgumentException("Empty funcitons count");

            List<Mistake.Solution.Mistake> result = [];
            var rules = settings.ActiveRules;

            var stack1 = stacks[0];
            var stack2 = stacks.Count > 1 ? stacks[1] : null;

            var function1 = functions.Count == 1 ? 
                functions[0] :
                functions.FirstOrDefault(x => x != null);

            var function2 = functions.Count > 1 ? functions[1] : null;

            foreach (var rule in rules)
            {
                if (rule.OneFunction)
                {
                    if (stack1 != null)
                    {
                        var satisfied1 = rule.IsSatisfied(functions: [function1], stacks: [stack1]);

                        if (!satisfied1)
                        {
                            var toAdd = new StackMistake([stack1], rule);
                            result.Add(toAdd);
                        }
                    }
                    
                    if (stack2 != null)
                    {
                        var satisfied2 = rule.IsSatisfied(functions: [function2!], stacks: [stack2]);

                        if (!satisfied2)
                        {
                            var toAdd = new StackMistake([stack2], rule);
                            result.Add(toAdd);
                        }
                    }                    
                }
                else
                {
                    if (stack1 != null && stack2 != null)
                    {
                        var satisfied = rule.IsSatisfied(functions: [function1, function2!], stacks: [stack1, stack2]);

                        if (!satisfied)
                        {
                            var toAdd = new StackMistake([stack1, stack2], rule);
                            result.Add(toAdd);
                        }
                    }
                }
            }

            return result;
        }
    }
}
