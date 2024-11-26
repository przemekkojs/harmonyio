﻿using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Music;
using Algorithm.New.Utils;
using System.Collections.Generic;
using System.Security;
using System.Security.AccessControl;
using System.Transactions;
using System.Windows.Markup;
using static System.Net.Mime.MediaTypeNames;

namespace Algorithm.New.Algorithm.Generators
{
    internal sealed record StackNode(Stack? stack)
    {
        public Stack? Stack { get; private set; } = stack;
        public List<StackNode> Nexts { get; private set; } = []; // Może słownik? Wtedy się uda z odległościami też
        public StackNode? Previous { get; set; } = null;
    }

    internal sealed class StackTree
    {
        public StackNode Root { get; private set; }
        public Settings Settings { get; private set; }
        public Dictionary<List<StackNode>, int> Solutions { get; private set; }
        public List<StackNode> All { get; private set; }

        private static readonly StackNode _empty = new(null);

        public StackTree() : this(Constants.Settings) { }

        public StackTree(Settings settings)
        {
            Root = _empty;
            Settings = settings;
            Solutions = [];
            All = [Root];
        }

        public void AddNext(StackNode node, StackNode child)
        {
            node.Nexts.Add(child);
            child.Previous = node;

            Solutions = [];
            All.Add(child);
        }

        public void AddNext(StackNode node, Stack child)
        {
            var childNode = new StackNode(child);
            AddNext(node, childNode);
        }

        public void EvaluateTree()
        {
            EvaluateRecursive(Root);

            Solutions = Solutions
                .OrderBy(x => x.Value)
                .ToDictionary();
        }

        private void EvaluateRecursive(StackNode current, int prevMistakes = 0, int tolerance = 15)
        {
            var nexts = current.Nexts;

            foreach (var next in nexts)
            {
                var eval = EvaluatePair(current, next);
                var currentMistakes = prevMistakes + eval;

                if (currentMistakes > tolerance)
                    continue;

                EvaluateRecursive(next, currentMistakes, tolerance);
            }

            if (current.Nexts.Count == 0)
            {
                var result = GetPreviousList(current);
                Solutions[result] = prevMistakes;
            }
        }

        public int EvaluatePair(StackNode prev, StackNode next)
        {
            var checkResult = StackPairChecker.CheckRules(prev.Stack, next.Stack, Settings);

            if (checkResult.Count == 0)
                return 0;
            else
                return checkResult.Sum(x => x.Quantity);
        }

        private static List<StackNode> GetPreviousList(StackNode start)
        {
            StackNode? current = start;
            List<StackNode> result = [];

            // Poszalałem cholibka, nie spodziewałem się, że kiedykolwiek tej pętli użyję
            do
            {
                result.Add(current);
                current = current.Previous;
            } while (current != null && current.stack != null);

            result.Reverse();
            return result;
        }
    }

    public static class SolutionGenerator
    {
        /// <summary>
        /// Z tej funkcji NALEŻY korzystać, ponieważ jest nieporównywalnie szybsza. Jedyne co, to otrzymujemy pierwsze
        /// znalezione poprawne rozwiązanie, a nie najlepsze.
        /// </summary>
        /// <param name="problem">Problem, na bazie którego będzie generowane rozwiązanie</param>
        /// <param name="tolerance">Tolerancja dla błędów. Domyślnie = 0</param>
        /// <returns>Wygenerowane rozwiązanie</returns>
        public static Solution GenerateLinear(Problem problem, int tolerance = 0)
        {
            var functions = problem.Functions;
            var maxIterations = 0;
            List<Stack> stacks = [];
            Dictionary<Function, List<List<string>>> mappings = [];
            List<int> usedNotesIndexes = [];

            foreach (var function in functions)
            {
                var possibleNotes = PossibleNotes.GeneratePossibleNotes(function);
                var notesSet = new List<List<string>>();


                foreach (var possibleSet in possibleNotes)
                {
                    var combinations = Combinations
                        .Generate(possibleSet);

                    notesSet.AddRange(combinations.Distinct());
                }

                maxIterations += notesSet.Count;
                mappings[function] = notesSet;
                usedNotesIndexes.Add(0);
            }

            var currentIndex = 0;
            var currentIteration = 0;
            var functionsCount = functions.Count;            

            while (currentIndex < functionsCount)
            {
                if (currentIteration >= maxIterations)
                    return new Solution(problem);

                currentIteration++;

                var function = functions[currentIndex];
                var usedNotesIndex = usedNotesIndexes[currentIndex];
                usedNotesIndexes[currentIndex]++;

                var possibleNotes = mappings[function][usedNotesIndex];
                var current = new Stack(function.Index, possibleNotes);

                if (stacks.Count == 0)
                {
                    stacks.Add(current);
                    currentIndex++;
                }
                else
                {
                    var prev = stacks.Last();

                    var checkResult = StackPairChecker
                        .CheckRules(prev, current, Constants.Settings);

                    var mistakesCount = checkResult.Count != 0 ?
                        checkResult.Sum(x => x.Quantity) :
                        0;

                    if (mistakesCount <= tolerance)
                    {
                        stacks.Add(current);
                        currentIndex++;
                    }
                    else if (usedNotesIndexes[currentIndex] >= mappings[function].Count)
                    {
                        currentIndex--;
                        usedNotesIndexes[currentIndex] = 0;
                        stacks.RemoveAt(currentIndex);
                    }
                }
            }

            var result = new Solution(problem, stacks);
            Rhytmize(result);

            return result;
        }

        // TODO: Zaimplementować
        private static void Rhytmize(Solution solution)
        {
            Dictionary<int, int> FunctionAmountsInBars = [];
            Dictionary<int, List<Stack>> StacksInBars = [];

            var metreCount = solution.Problem.Metre.Count;
            var metreValue = solution.Problem.Metre.Value;

            foreach (var stack in solution.Stacks)
            {
                var bar = stack.Index.Bar;

                if (!FunctionAmountsInBars.ContainsKey(bar))
                    FunctionAmountsInBars[bar] = 0;

                if (!StacksInBars.ContainsKey(bar))
                    StacksInBars[bar] = [];

                FunctionAmountsInBars[bar]++;
                StacksInBars[bar].Add(stack);
            }

            foreach (var key in FunctionAmountsInBars.Keys)
            {
                var functionsAmount = FunctionAmountsInBars[key];
                var scheme = GetRhytmicScheme(functionsAmount, metreCount, metreValue);
                var stacks = StacksInBars[key];

                for (int stackIndex = 0; stackIndex < stacks.Count; stackIndex++)
                {
                    var stack = stacks[stackIndex];
                    var duration = scheme[stackIndex];
                    stack.Index.Duration = duration;
                }
            }
        }

        private static List<int> GetRhytmicScheme(int functionsInBar, int metreCount, int metreValue)
        {
            List<int> result = [];

            var valueToMultipler = metreValue switch
            {                
                4 => 1,
                8 => 2,                
                _ => throw new ArgumentException("Invalid metre value")
            };

            var baseValue = (metreValue * metreCount) / valueToMultipler;
            result.Add(baseValue);

            for (int index = 0; index < functionsInBar - 1; index++)
            {
                var allSame = SameElementsInList(result);

                if (allSame)
                {
                    var last = result.Last();
                    var lastDivided = DivideNote(last, metreValue);

                    result[^1] = lastDivided.Item1; // [^1] to jak [-1] w pythonie
                    result.Add(lastDivided.Item2);
                }
                else
                {
                    var maxIndex = result.LastIndexOf(result.Max());
                    var max = result[maxIndex];
                    var divided = DivideNote(max, metreValue);

                    result[maxIndex] = divided.Item1;
                    result.Insert(maxIndex + 1, divided.Item2);
                }
            }

            if (result.Count != functionsInBar)
                throw new Exception("Something went wrong...");

            return result;
        }

        private static (int, int) DivideNote (int rhytmicValue, int metreValue)
        {
            if (rhytmicValue == 12 && metreValue == 8)
                return (6, 6);

            return rhytmicValue switch
            {
                16 => (8, 8),
                8 => (4, 4),
                6 => (4, 2),
                4 => (2, 2),
                3 => (2, 1),
                2 => (1, 1),
                _ => throw new ArgumentException("Invalid rhytmic value")
            };
        }

        private static bool SameElementsInList(List<int> list)
        {
            if (list.Count == 0)
                return true;

            var first = list[0];

            foreach (var value in list)
            {
                if (first != value)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Z tej funkcji NIE NALEŻY korzystać, chyba że do celów badawczych. Generuje ona wszystkie możliwe rozwiązania,
        /// z obcinaniem dla zadanej tolerancji. Ma jednak gwarantowane zwrócenie najlepszego możliwego rozwiązania, jeżeli
        /// takowie się znajdzie. <br/>
        /// 
        /// Funkcja <b>nie jest</b> zaimplementowana do końca, więc na razie proszę NIE RUSZAĆ.
        /// </summary>
        /// <param name="problem">Problem, na bazie którego będzie generowane rozwiązanie</param>
        /// <returns>Najlepsze rozwiązanie.</returns>
        public static Solution Generate(Problem problem)
        {
            List<Stack> stacks = [];
            StackTree tree = new();
            StackNode current = tree.Root;
            StackNode prev = current;

            var functions = problem.Functions;

            GenerateRecursive(functions, prev, tree, tolerance: 6);
            tree.EvaluateTree();

            var firstItem = tree.Solutions.First();
            stacks = firstItem.Key
                .Select(x => x.Stack!)
                .ToList();

            // TODO: Trzeba jeszcze poprawić wartości rytmiczne

            return new Solution(problem, stacks);
        }

        private static void GenerateRecursive(
            List<Function> functions, StackNode prev, StackTree tree,
            int functionIndex = 0, int tolerance = 10, int currentMistakes = 0)
        {
            if (functionIndex >= functions.Count)
                return;

            var function = functions[functionIndex];
            var possibleNotes = PossibleNotes.GeneratePossibleNotes(function);

            var notesSet = new List<List<string>>();

            foreach (var possibleSet in possibleNotes)
            {
                var combinations = Combinations
                    .Generate(possibleSet);

                notesSet.AddRange(combinations);
            }

            foreach (var notes in notesSet)
            {
                var stack = new Stack(function.Index, notes);
                var next = new StackNode(stack);

                var checkResult = StackPairChecker
                    .CheckRules(prev.Stack, next.Stack, tree.Settings);

                var newMistakes = currentMistakes + checkResult.Count;

                if (newMistakes < tolerance)
                {
                    tree.AddNext(prev, next);
                    GenerateRecursive(functions, next, tree,
                        functionIndex: functionIndex + 1,
                        tolerance: tolerance,
                        currentMistakes: newMistakes);
                }
            }
        }
    }
}