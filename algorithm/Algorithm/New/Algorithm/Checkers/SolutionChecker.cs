﻿using Algorithm.New.Algorithm.Mistake.Solution;
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

            MapNotesToComponents(solution);

            List<Mistake.Solution.Mistake> result = [];
            var noteMistakes = CheckNoteMistakes(solution);
            var stackMistakes = CheckStackMistakes(solution, settings);

            result.AddRange(noteMistakes);
            result.AddRange(stackMistakes);

            return result;
        }

        // TODO: Implementacja tego
        private static void MapNotesToComponents(Solution solution)
        {

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

            ValidateStackToFunctionMapping(solution);

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

            ValidateStackToFunctionMapping(solution);

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
