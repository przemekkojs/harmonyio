using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public abstract class Rule
    {
        public int Id { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public bool OneFunction { get; protected set; }

        protected Rule(int id, string name, string description, bool oneFunction = false)
        {            
            Id = id;
            Name = name;
            Description = description;
            OneFunction = oneFunction;
        }

        // Przy wejściu do zasady, dowolny stos ma GWARANTOWANE 4 głosy ustawione,
        // ale część z nich może być null-em
        public abstract bool IsSatisfied(List<Function> functions, List<Stack> stacks);

        protected bool ValidateParametersCount(List<Stack> stacks)
        {
            if (OneFunction && stacks.Count != 1)
                return false;
            else if (!OneFunction && stacks.Count != 2)
                return false;

            return true;
        }

        protected static bool ValidateEmptyStacks(List<Stack> stacks)
        {
            foreach (var stack in stacks)
            {
                var allNull = stack.Notes.All(x => x == null);

                if (allNull)
                    return false;
            }

            return true;
        }
    }
}
