using Algorithm.New.Algorithm.Mistake;
using Algorithm.New.Music;
using Algorithm.New.Utils;
using System.Diagnostics;

namespace Algorithm.New.Algorithm.Checkers
{
    /// <summary>
    /// Klasa sprawdzająca rozwiązanie
    /// </summary>
    public static class SolutionChecker
    {
        /// <summary>
        /// Metoda główna
        /// </summary>
        /// <param name="solution">Rozwiązanie</param>
        /// <param name="settings">Ustawienia sprawdzania</param>
        /// <returns>Lista błędów</returns>
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
                var stackNotes = stack.Notes;

                var function = solution.Problem.Functions[index];
                var possibleVersions = PossibleNotes.GeneratePossibleNotes(function);

                var uniqueNotes = possibleVersions
                    .SelectMany(x => x)
                    .Distinct()
                    .ToList();

                int noteIndex = 0;

                foreach (var note in stackNotes)
                {
                    if (note == null)
                    {
                        var voice = noteIndex switch { 0 => "S", 1 => "A", 2 => "T", _ => "B" };
                        var toAppend = NoteMistake.CreateEmptyNoteMistake(stack.Index.Bar, stack.Index.Position, voice);
                        result.Add(toAppend);
                    }
                    else
                    {
                        var noteName = note?.Name;
                        noteName ??= string.Empty;

                        if (!uniqueNotes.Contains(noteName))
                        {
                            var toAppend = new NoteMistake(note, stack);
                            result.Add(toAppend);
                        }
                    }

                    noteIndex++;
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
                        
                        if (!rule.IsSatisfied(stacks: stack))
                        {
                            var toAdd = new StackMistake([stack], rule);
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

                        if (!rule.IsSatisfied(stacks: [stack1, stack2]))
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
