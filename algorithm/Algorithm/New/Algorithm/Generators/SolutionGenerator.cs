﻿using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Music;
using Algorithm.New.Utils;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

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

        /// <summary>
        /// TA FUNKCJA NIE DZIAŁA - NIE KORZYSTAĆ
        /// </summary>
        /// <param name="prev"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public int EvaluatePair(StackNode prev, StackNode next)
        {
            var checkResult = StackPairChecker.CheckRules(functions: [], stacks: [prev.Stack, next.Stack], Settings);

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
        public static (Solution, int) GenerateLinear(Problem problem, int tolerance = 0)
        {
            List<Stack> stacks = [];
            List<Function> functionsTmp = [];
            List<int> usedNotesIndexes = [];
            Dictionary<Function, List<List<(string, Component)>>> mappings = [];

            var functions = problem.Functions;
            var currentIndex = 0;
            var functionsCount = functions.Count;

            UInt128 maxIterations = 1;
            UInt128 currentIteration = 0;

            foreach (var function in functions)
            {
                var possibleNotes = PossibleNotes.GeneratePossibleNotes(function);
                var notesSet = new List<List<(string, Component)>>();

                foreach (var possibleSet in possibleNotes)
                {
                    var combinations = Combinations
                        .Generate(possibleSet)
                        .Distinct();

                    notesSet.AddRange(combinations);
                }

                notesSet = ValidatedResult(function, notesSet);

                notesSet = notesSet
                    .Distinct()
                    .ToList();

                mappings[function] = notesSet;
                usedNotesIndexes.Add(0);
            }

            foreach (var key in mappings.Keys)
            {
                var valueList = mappings[key];
                uint count = (UInt32)valueList.Count;

                maxIterations += count != 0 ?
                    count * 10 :
                    1;
            }

            while (currentIndex < functionsCount && currentIteration < maxIterations)
            {
                var currentFunction = functions[currentIndex];
                var usedNotesIndex = usedNotesIndexes[currentIndex];
                var mapping = mappings[currentFunction];
                var possibleNotes = new List<(string, Component)>();

                try
                {
                    possibleNotes = mapping[usedNotesIndex];
                }
                catch (Exception ex)
                {
                    currentIndex--;
                    tolerance++;

                    if (currentIndex >= 0)
                    {
                        usedNotesIndexes[currentIndex + 1] = 0;
                        stacks.RemoveAt(currentIndex);
                        functionsTmp.RemoveAt(currentIndex);
                    }
                    else
                    {
                        currentIndex = 0;
                        usedNotesIndexes[currentIndex] = 0;
                    }

                    currentFunction = functions[currentIndex];
                    usedNotesIndex = usedNotesIndexes[currentIndex];
                    mapping = mappings[currentFunction];
                    possibleNotes = mapping[usedNotesIndex];
                }

                currentIteration++;
                usedNotesIndexes[currentIndex]++;

                var possibleNoteNames = possibleNotes
                    .Select(x => x.Item1)
                    .ToList();

                var possibleComponents = possibleNotes
                    .Select(x => x.Item2)
                    .ToList();

                var currentStack =
                    new Stack(currentFunction.Index, possibleNoteNames);

                for (int i = 0; i < possibleComponents.Count; i++)
                {
                    var note = currentStack.Notes[i];

                    if (note == null)
                        continue;

                    note.Component = possibleComponents[i];
                }

                if (stacks.Count == 0)
                {
                    stacks.Add(currentStack);
                    functionsTmp.Add(currentFunction);
                    currentIndex++;

                    continue;
                }

                var prevStack = stacks.Last();
                var prevFunction = functionsTmp.Last();

                var checkResult = StackPairChecker
                    .CheckRules([prevFunction, currentFunction], [prevStack, currentStack], Constants.Settings);

                var mistakesCount = checkResult.Count != 0 ?
                    checkResult.Sum(x => x.Quantity) :
                    0;

                if (mistakesCount <= tolerance)
                {
                    stacks.Add(currentStack);
                    functionsTmp.Add(currentFunction);
                    currentIndex++;
                }
                else
                {
                    if (currentIteration >= maxIterations)
                    {
                        currentIteration = 0;
                        currentIndex--;
                        tolerance++;

                        if (currentIndex >= 0)
                        {
                            usedNotesIndexes[currentIndex + 1] = 0;
                            stacks.RemoveAt(currentIndex);
                            functionsTmp.RemoveAt(currentIndex);
                        }
                        else
                        {
                            currentIndex = 0;
                            usedNotesIndexes[currentIndex] = 0;
                        }

                        currentFunction = functions[currentIndex];
                        usedNotesIndex = usedNotesIndexes[currentIndex];
                        mapping = mappings[currentFunction];
                        possibleNotes = mapping[usedNotesIndex];
                    }

                    while (usedNotesIndexes[currentIndex] >= mapping.Count)
                    {
                        currentIndex--;

                        if (currentIndex >= 0)
                        {
                            usedNotesIndexes[currentIndex + 1] = 0;
                            stacks.RemoveAt(currentIndex);
                            functionsTmp.RemoveAt(currentIndex);
                        }
                        else
                        {                            
                            currentIndex = 0;
                            usedNotesIndexes[currentIndex] = 0;
                            tolerance++;
                        }
                    }
                }
            }

            var result = new Solution(problem, stacks);
            Rhytmize(result);
            RemoveUnnecessaryAccidentals(result);

            return (result, tolerance);
        }

        private static List<List<(string, Component)>> ValidatedResult(Function function, List<List<(string, Component)>> result)
        {
            List<List<(string, Component)>> toRemove = [];

            var functionPosition = function.Position;
            var functionRoot = function.Root;

            foreach (var possible in result)
            {
                var position = possible[0].Item2;
                var root = possible[3].Item2;

                // Wszystkie, gdzie pozycja się nie zgadza
                if (functionPosition != null && position != null)
                {
                    var equals = functionPosition.Equals(position);

                    if (!equals)
                        toRemove.Add(possible);
                }

                // Wszystkie, gdzie oparcie się nie zgadza
                if (functionRoot != null && root != null)
                {
                    var equals = functionRoot.Equals(root);

                    if (!equals)
                        toRemove.Add(possible);
                }
            }

            if (toRemove.Count > 0)
            {
                return result
                    .Where(x => !toRemove.Contains(x))
                    .ToList();
            }

            return result;
        }

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
                var scheme = Rhytm.GetRhytmicScheme(functionsAmount, metreCount, metreValue);
                var stacks = StacksInBars[key];

                for (int stackIndex = 0; stackIndex < stacks.Count; stackIndex++)
                {
                    var stack = stacks[stackIndex];
                    var duration = scheme[stackIndex];
                    stack.Index.Duration = duration;
                }
            }
        }

        private static void RemoveUnnecessaryAccidentals(Solution solution)
        {
            var tonation = solution.Problem.Tonation;
            var sharpsCount = tonation.SharpsCount;
            var flatsCount = tonation.FlatsCount;

            var accidentalsCount = sharpsCount + flatsCount;

            // Dla C-dur i a-moll nie ma co robić
            if (sharpsCount == 0 && flatsCount == 0)
                return;

            var accidentalsList = sharpsCount > 0 ?
                Constants.SharpsQueue :
                Constants.FlatsQueue;

            var accidentals = accidentalsList
                .GetRange(0, accidentalsCount);

            if (accidentals == null)
                return;

            foreach (var stack in solution.Stacks)
            {
                foreach (var note in stack.Notes)
                {
                    if (note == null)
                        continue;

                    var accidental = note.Accidental;
                    var noteName = note.Name[0].ToString();

                    if (accidentals.Contains(noteName))
                        note.Accidental = "";
                }
            }
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

            return new Solution(problem, stacks);
        }

        private static void GenerateRecursive(
            List<Function> functions, StackNode prev, StackTree tree,
            int functionIndex = 0, int tolerance = 10, int currentMistakes = 0)
        {
            if (functionIndex >= functions.Count)
                return;

            var prevFunction = functionIndex > 0 ? functions[functionIndex - 1] : null;
            var function = functions[functionIndex];
            var possibleNotes = PossibleNotes.GeneratePossibleNotes(function);

            var notesSet = new List<List<(string, Component)>>();

            foreach (var possibleSet in possibleNotes)
            {
                var combinations = Combinations
                    .Generate(possibleSet);

                notesSet.AddRange(combinations);
            }

            foreach (var notes in notesSet)
            {
                var noteNames = notes
                    .Select(x => x.Item1)
                    .ToList();

                var components = notes
                    .Select(x => x.Item2)
                    .ToList();

                var stack = new Stack(function.Index, noteNames);

                for (int i = 0; i < stack.Notes.Count; i++)
                {
                    stack.Notes[i].Component = components[i];
                }

                var next = new StackNode(stack);


                var checkResult = StackPairChecker
                    .CheckRules([prevFunction, function], [prev.Stack, next.Stack], tree.Settings);

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