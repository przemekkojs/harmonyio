using Algorithm.Old.Algorithm;

namespace Algorithm.Old.Algorithm.Rules
{
    public abstract class Rule
    {
        public int ExpectedParametersCount { get => expectedParametersCount; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        protected readonly int expectedParametersCount;

        protected Rule(string name, string description = "", int expectedParametersCount = 2)
        {
            Name = name;
            Description = description;
            this.expectedParametersCount = expectedParametersCount;
        }

        public abstract bool IsSatisfied(params Stack[] stacks);
    }
}
