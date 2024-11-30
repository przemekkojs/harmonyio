using Algorithm.New.Algorithm.Mistake.Solution;
using Algorithm.New.Music;
using Algorithm.New.Utils;

namespace Algorithm.New.Algorithm.Checkers
{
    /// <summary>
    /// Klasa sprawdzająca rozwiązanie
    /// </summary>
    public static class SolutionChecker
    {
        /// <summary>
        /// Metoda sprawdzająca. Sprawdza błędy nutowe i funkcyjne.
        /// </summary>
        /// <param name="solution">Rozwiązanie użytkownika.</param>
        /// <param name="settings">Ustawienia sprawdzania - czyli wszystkie aktywne zasady</param>
        /// <returns>Lista błędów nutowych (NoteMistake) i funkcyjnych (StackMistake). Wszystko w jednej liście
        /// typu Mistake.</returns>
        public static List<Mistake.Solution.Mistake>? CheckSolution(Solution solution, Settings settings)
        {
            if (solution == null)
                return null;

            if (SolutionEmpty(solution))
                return null;

            ValidateStackToFunctionMapping(solution);
            MapNotesToComponents(solution);

            List<Mistake.Solution.Mistake> result = [];
            var noteMistakes = CheckNoteMistakes(solution);
            var stackMistakes = CheckStackMistakes(solution, settings);

            result.AddRange(noteMistakes);
            result.AddRange(stackMistakes);

            return result;
        }

        private static void MapNotesToComponents(Solution solution)
        {
            var stacks = solution.Stacks;
            var functions = solution.Problem.Functions;

            for (int stackIndex = 0; stackIndex < stacks.Count; stackIndex++)
            {
                var stack = stacks[stackIndex];
                var function = functions[stackIndex];

                var stackNotes = stack.Notes;
                var symbolOffset = FunctionSymbolToOffset(function);

                var possibleNotes = PossibleNotes.GeneratePossibleNotes(function);
                var possibleNotesFlattened = possibleNotes
                    .SelectMany(x => x)
                    .ToList();

                foreach (var note in stackNotes)
                {
                    if (note == null)
                        continue;

                    var matchingComponent = possibleNotesFlattened
                        .FirstOrDefault(x => x.Item1.Equals(note.Name))
                        .Item2;

                    note.Component = matchingComponent;
                }
            }
        }

        private static int FunctionSymbolToOffset(Symbol symbol)
        {
            return symbol switch
            {
                Symbol.T => 0,
                Symbol.Sii => 1,
                Symbol.Tiii => 2,
                Symbol.Diii => 2,
                Symbol.S => 3,
                Symbol.D => 4,
                Symbol.Tvi => 5,
                Symbol.Svi => 5,
                Symbol.Dvii => 6,
                _ => 6
            };
        }

        private static int FunctionSymbolToOffset(Function function)
        {
            var symbol = function.Symbol;
            return FunctionSymbolToOffset(symbol);
        }

        private static bool SolutionEmpty(Solution solution)
        {
            var stacks = solution.Stacks;

            foreach (var stack in stacks)
            {
                var nullCount = 0;

                foreach(var note in stack.Notes)
                {
                    if (note == null)
                        nullCount++;
                    else
                        break;
                }

                if (nullCount != 4)
                    return false;
            }

            return true;
        }

        private static void ValidateStackToFunctionMapping(Solution solution)
        {
            if (solution.Stacks.Count != solution.Problem.Functions.Count)
                throw new ArgumentException("Invalid solution to function mapping.");
        }

        private static List<NoteMistake> CheckNoteMistakes(Solution solution)
        {
            List<NoteMistake> result = [];            

            for (int index = 0; index < solution.Stacks.Count; index++)
            {
                var stack = solution.Stacks[index];
                var stackNotes = stack.Notes;

                var function = solution.Problem.Functions[index];
                var possibleVersions = PossibleNotes
                    .GeneratePossibleNotes(function);

                var uniqueNotes = possibleVersions
                    .SelectMany(x => x)
                    .Distinct()
                    .ToList();

                int noteIndex = 0;

                foreach (var note in stackNotes)
                {                    
                    var noteName = note?.Name;
                    noteName ??= string.Empty;

                    var uniqueNoteNames = uniqueNotes
                        .Select(x => x.Item1);

                    if (!uniqueNoteNames.Contains(noteName))
                    {
                        var toAppend = new NoteMistake(note, stack);
                        result.Add(toAppend);
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
