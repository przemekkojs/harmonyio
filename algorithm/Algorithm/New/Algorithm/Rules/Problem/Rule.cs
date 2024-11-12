using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Problem
{
    public abstract class Rule
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool OneFunction { get; set; }

        protected Rule(string name, string description, bool oneFunction = false)
        {
            Name = name;
            Description = description;
            OneFunction = oneFunction;
        }

        public abstract bool IsSatisfied(params Function[] functions);

        protected bool ValidateParametersCount(params Function[] functions)
        {
            if (OneFunction && functions.Length != 1)
                return false;
            else if (!OneFunction && functions.Length != 2)
                return false;

            return true;
        }
    }
}
