using Algorithm.New.Music;
using Algorithm.New.Utils;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public sealed class AugmentedInterval : Rule
    {
        public AugmentedInterval() : base(
            id: 107,
            name: "Ruch o interwał zwiększony",
            description: "Ruch o interwał zwiększony w dowolnym głosie nie jest dozwolony",
            oneFunction: false) { }

        public override bool IsSatisfied(string additionalParamsJson = "", params Stack[] stacks)
        {
            if (!ValidateParametersCount(stacks))
                return false;

            if (!ValidateEmptyStacks(stacks))
                return false;

            var stack1 = stacks[0];
            var stack2 = stacks[1];

            for (int noteIndex = 0; noteIndex < stack1.Notes.Count; noteIndex++)
            {
                var note1 = stack1.Notes[noteIndex];
                var note2 = stack2.Notes[noteIndex];

                var intervalName = Interval.IntervalBetween(note1, note2);

                if (intervalName == IntervalName.WRONG)
                    return false;

                if (intervalName.ToString().Contains("Augmented"))
                    return false;
            }

            return true;
        }
    }
}
