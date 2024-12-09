using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public class DominantSeventhResolution : Rule
    {
        public DominantSeventhResolution() : base(
            id: 117,
            name: "Rozwiązanie dominanty septymowej",
            description: "Czy jest właściwe, w zależności od postaci",
            oneFunction: false)
        { }

        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            if (!ValidateEmptyStacks(stacks))
                return false;

            if (!ValidateParametersCount(stacks))
                return false;

            // TODO: Implementacja, tego będzie dużo niestety, ale trzeba

            return true;
        }
    }
}
