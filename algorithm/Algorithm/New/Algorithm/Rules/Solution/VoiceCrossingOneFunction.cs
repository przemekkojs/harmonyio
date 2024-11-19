using Algorithm.New.Music;
using Algorithm.New.Utils;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    public class VoiceCrossingOneFunction : Rule
    {
        public VoiceCrossingOneFunction() : base(
            id: 105,
            name: "Krzyżowanie głosów w ramach jednej funkcji",
            description: "Czy nuty w ramach jednej funkcji nie krzyżują się wysokościami w ramach sąsiednich głosów?",
            oneFunction: true)
        { }

        public override bool IsSatisfied(string additionalParamsJson = "", params Music.Stack[] stacks)
        {
            if (!ValidateParametersCount(stacks))
                return false;

            if (!ValidateEmptyStacks(stacks))
                return false;

            var stack = stacks[0];

            bool sopranoAlto = NoteCheckResult(stack.Soprano, stack.Alto);
            bool altoTenore = NoteCheckResult(stack.Alto, stack.Tenore);
            bool tenoreBass = NoteCheckResult(stack.Tenore, stack.Bass);            

            return sopranoAlto && altoTenore && tenoreBass;
        }

        private static bool NoteCheckResult(Note? note1, Note? note2)
        {
            if (note1 != null && note2 != null)
                return Interval.IsLower(note2, note1);
            else
                return true;
        }
    }
}
