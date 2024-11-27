using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public sealed class DoubledFifthOnStrongBarPart : Rule
    {
        public DoubledFifthOnStrongBarPart() : base(
            id: 100,
            name: "Dwojenie kwinty na mocnej części taktu",
            description: "Czy w ramach jednej funkcji, przypadającej na mocnej części taktu, podwojona została kwinta, przy oparciu na kwincie?",
            oneFunction: true)
        { }

        // TODO: Jakoś trzeba wsm przekazać metrum... XD
        // Wsm przekazywanie który składnik jest kwintą też się cholibka przyda
        public override bool IsSatisfied(string additionalParamsJson = "", params Stack[] stacks)
        {
            if (!ValidateParametersCount(stacks))
                return false;

            if (!ValidateEmptyStacks(stacks))
                return false;

            var stack = stacks[0];

            int verticalIndex = stack.Index.Position;

            if (verticalIndex == 0)
            {
                // TODO: Add logic here
            }

            return true;
        }
    }
}
