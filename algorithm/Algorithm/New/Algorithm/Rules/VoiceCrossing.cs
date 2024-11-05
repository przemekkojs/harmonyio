﻿using Algorithm.New.Music;
using Algorithm.New.Utils;

namespace Algorithm.New.Algorithm.Rules
{
    public class VoiceCrossing : Rule
    {
        public VoiceCrossing() : base(
            name: "Krzyżowanie głosów",
            description: "Czy nuty w obu funkcjach nie krzyżują się wysokościami w ramach tego samego głosu?") { }

        public override bool IsSatisfied(params Stack[] functions)
        {
            if (functions.Length != 2)
                return false;

            var stack1 = functions[0];
            var stack2 = functions[1];

            bool sopranoSatisfied1 = NoteCheckResult(stack1.Soprano, stack2.Alto);
            bool altoSatisfied1 = NoteCheckResult(stack1.Alto, stack2.Tenore);
            bool tenoreSatisfied1 = NoteCheckResult(stack1.Tenore, stack2.Bass);

            bool sopranoSatisfied2 = NoteCheckResult(stack1.Alto, stack2.Soprano);
            bool altoSatisfied2 = NoteCheckResult(stack1.Tenore, stack2.Alto);
            bool tenoreSatisfied2 = NoteCheckResult(stack1.Bass, stack2.Tenore);

            return sopranoSatisfied1 && sopranoSatisfied2 &&
                altoSatisfied1 && altoSatisfied2 &&
                tenoreSatisfied1 && tenoreSatisfied2;
        }

        private bool NoteCheckResult(Note? note1, Note? note2)
        {
            if (note1 != null && note2 != null)
                return Interval.IsLower(note1, note2);
            else
                return false;
        }
    }
}
