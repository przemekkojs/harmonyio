using Algorithm.New.Music;

namespace Algorithm.New.Algorithm
{
    public class Solution
    {
        public Problem Problem { get; private set; }
        public List<Stack> Stacks { get; private set; }

        public Solution(Problem problem, List<Stack> stacks)
        {
            Problem = problem;
            Stacks = stacks;
        }

        public Solution(Problem problem) : this(problem, []) { }
    }
}
