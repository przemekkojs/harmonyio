using Algorithm.New.Music;
using Algorithm.New.Utils;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public sealed class SemitonesCrossing : Rule
    {
        public SemitonesCrossing() : base(
            id: 112,
            name: "Skośne półtony",
            description: "Ruch o półton nie powinien występować między dwoma różnymi głosami.",
            oneFunction: false) { }

        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            if (!ValidateParametersCount(stacks))
                return false;

            if (!ValidateEmptyStacks(stacks))
                return false;

            var stack1 = stacks[0];
            var stack2 = stacks[1];

            for (int note1Index = 0; note1Index < stack1.Notes.Count; note1Index++)
            {
                var note1 = stack1.Notes[note1Index];
                var note1Name = note1?.Name ?? string.Empty;

                if (note1Name.Equals(string.Empty))
                    continue;

                for (int note2Index = 0; note2Index < stack2.Notes.Count; note2Index++)
                {
                    if (note1Index == note2Index)
                        continue;

                    var note2 = stack2.Notes[note2Index];
                    var note2Name = note2?.Name ?? string.Empty;

                    if (note2Name.Equals(string.Empty))
                        continue;

                    var tmp1 = new Note(note1Name, 4);
                    var tmp2 = new Note(note2Name, 4);

                    if (Interval.IsLower(tmp1, tmp2))
                        tmp1.Octave++;

                    var intervalName = Interval.IntervalBetween(tmp1, tmp2);

                    if (intervalName == IntervalName.MinorSecond)
                        return false;
                }
            }

            return true;
        }
    }
}
