using Algorithm.New.Music;
using Algorithm.Old.Music;

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

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is Solution casted)
            {
                if (casted.Stacks.Count != Stacks.Count)
                    return false;

                if (!casted.Problem.Equals(Problem))
                    return false;

                for (int index = 0; index < casted.Stacks.Count; index++)
                {
                    var stack1 = Stacks[index];
                    var stack2 = casted.Stacks[index];

                    if (stack1 != null)
                    {
                        var stacksEqual = stack1.Equals(stack2);

                        if (!stacksEqual)
                            return false;
                    }
                }
            }
            else
                return false;

            return true;
        }
    }
}
