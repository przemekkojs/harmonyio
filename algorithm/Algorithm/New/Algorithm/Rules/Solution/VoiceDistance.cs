using Algorithm.New.Music;
using Algorithm.New.Utils;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public class VoiceDistance : Rule
    {
        public VoiceDistance() : base(
            id: 113,
            name: "Odległość między głosami",
            description: "Odległość między głosami nie powinna przekraczać oktawy, z wyjątkiem tenoru i basu - 2 oktawy.",
            oneFunction: true)
        { }

        public override bool IsSatisfied(List<Function> functions, List<Stack> stacks)
        {
            if (!ValidateEmptyStacks(stacks))
                return false;

            if (!ValidateParametersCount(stacks))
                return false;

            var stack = stacks[0];

            var soprano = stack.Soprano;
            var alto = stack.Alto;
            var tenore = stack.Tenore;
            var bass = stack.Bass;

            var sopranoAlto = CheckNotePairs(soprano, alto);
            var altoTenore = CheckNotePairs(alto, tenore);
            var tenoreBass = CheckNotePairs(tenore, bass);

            return sopranoAlto && altoTenore && tenoreBass;
        }

        private static bool CheckNotePairs(Note? note1, Note? note2)
        {
            if (note1 != null && note2 != null)
            {
                var distance = Interval.SemitonesBetween(note1, note2);

                if (distance > 12)
                    return false;
            }

            return true;
        }
    }
}
