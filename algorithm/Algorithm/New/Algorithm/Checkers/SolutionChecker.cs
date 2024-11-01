using Algorithm.New.Algorithm.Mistake;
using Algorithm.New.Utils;

namespace Algorithm.New.Algorithm.Checkers
{
    public static class SolutionChecker
    {
        public static List<Mistake.Mistake> CheckSolution(Solution solution, Settings settings)
        {
            List<Mistake.Mistake> result = [];
            var noteMistakes = CheckNoteMistakes(solution);
            var stackMistakes = CheckStackMistakes(solution, settings);

            result.AddRange(noteMistakes);
            result.AddRange(stackMistakes);

            return result;
        }

        private static List<NoteMistake> CheckNoteMistakes(Solution solution)
        {
            List<NoteMistake> result = [];

            for (int index = 0; index < solution.Stacks.Count; index++)
            {
                var stack = solution.Stacks[index];
                var stackNotes = stack.Notes
                    .Select(x => x.Name)
                    .ToList();

                var function = solution.Problem.Functions[index];
                var possibleVersions = PossibleNotes.GeneratePossibleNotes(function);

                var uniqueNotes = possibleVersions
                    .SelectMany(x => x)
                    .Distinct()
                    .ToList();

                foreach (var note in stackNotes)
                {
                    List<string> tmpList = [];

                    if (!uniqueNotes.Contains(note))
                        tmpList.Add(note);

                    if (tmpList.Count > 0)
                    {
                        var toAppend = new NoteMistake();
                        toAppend.Notes.AddRange(tmpList);
                        toAppend.Stack = stack;
                        result.Add(toAppend);
                    }
                }
            }

            return result;
        }

        private static List<StackMistake> CheckStackMistakes(Solution solution, Settings settings)
        {
            List<StackMistake> result = [];

            foreach (var rule in settings.ActiveRules)
            { 
                if (rule.OneFunction)
                {
                    for (int index = 0; index < solution.Stacks.Count; index++)
                    {
                        var stack = solution.Stacks[index];
                        
                        if (!rule.IsSatisfied(stack))
                        {
                            var toAdd = new StackMistake();
                            toAdd.Stacks.Add(stack);
                            toAdd.Rule = rule;
                            result.Add(toAdd);
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < solution.Stacks.Count - 1; index++)
                    {
                        var stack1 = solution.Stacks[index];
                        var stack2 = solution.Stacks[index + 1];

                        if (!rule.IsSatisfied(stack1, stack2))
                        {
                            var toAdd = new StackMistake();
                            toAdd.Stacks.Add(stack1);
                            toAdd.Stacks.Add(stack2);
                            toAdd.Rule = rule;
                            result.Add(toAdd);
                        }
                    }
                }
            }    

            return result;
        }
    }
}
