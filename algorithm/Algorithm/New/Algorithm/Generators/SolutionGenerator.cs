using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Music;
using Algorithm.New.Utils;
using System.Security;
using System.Transactions;
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

        public StackTree() : this( Constants.Settings) { }        

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

        private void EvaluateRecursive(StackNode current, int prevMistakes=0, int tolerance=15)
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
        public static Solution GenerateLinear(Problem problem, int tolerance = 2)
        {
            var functions = problem.Functions;
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

                    notesSet.AddRange(combinations);
                }

                mappings[function] = notesSet;
                usedNotesIndexes.Add(0);
            }

            var currentIndex = 0;
            var functionsCount = functions.Count;

            while (currentIndex < functionsCount)
            {
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

                    if (checkResult.Count <= tolerance)
                    {
                        stacks.Add(current);
                        currentIndex++;
                    }
                    else
                    {                        
                        currentIndex--;
                        stacks.RemoveAt(currentIndex);                        
                    }
                }
            }

            return new Solution(problem, stacks);
        }

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
