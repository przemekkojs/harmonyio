using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Generators
{
    internal sealed record StackNode(Stack stack)
    {
        public Stack Stack { get; private set; } = stack;
        public List<StackNode> Nexts { get; private set; } = []; // Może słownik? Wtedy się uda z odległościami też
        public StackNode? Previous { get; set; } = null;
    }

    internal sealed class StackTree
    {
        public StackNode Root { get; private set; }
        public Settings Settings { get; private set; }
        public StackTree(Stack stack) : this(stack, Constants.Settings) { }
        public Dictionary<List<StackNode>, int> Solutions { get; private set; }
        public List<StackNode> All { get; private set; }

        private StackTree(Stack stack, Settings settings)
        {
            if (stack == null)
                throw new ArgumentException("Cannot begin tree from null");

            Root = new StackNode(stack);
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
                .OrderBy(x => x.Key)
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

            var result = GetPreviousList(current);
            Solutions[result] = prevMistakes;
        }

        private int EvaluatePair(StackNode prev, StackNode next)
        {
            var checkResult = StackPairChecker.CheckRules(prev.Stack, next.Stack, Settings);

            return checkResult
                .Sum(x => x.Quantity);
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
            } while (current != null);

            return result;
        }
    }

    public static class SolutionGenerator
    {
        public static List<Stack> Generate(Problem problem)
        {
            List<Stack> result = [];
            var functions = problem.Functions;

            Stack current = null;

            foreach (var function in functions)
            {

            }

            return result;
        }
    }
}
