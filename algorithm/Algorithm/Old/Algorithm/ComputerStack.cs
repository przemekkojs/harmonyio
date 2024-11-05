using Algorithm.Old.Music;

namespace Algorithm.Old.Algorithm
{
    public sealed class ComputerStack : Stack
    {
        private Function? nextFunction;
        private readonly List<ComputerStack> possibleNexts;

        public ComputerStack(Function baseFunction, Tonation tonation, int startBeat) : base(baseFunction, tonation, startBeat)
        {
            nextFunction = null;
            possibleNexts = [];
        }

        // Dla komputera wartość rytmiczna się wsm nie liczy, front dba żeby wszystko było wpisane git
        // Zatem funkcja może mieć dowolną wartość rytmiczna - na ten moment ćwierćnuta
        public void PossibleNexts(Bar bar, Function? nextFunction)
        {
            var rhytmicValue = RhytmicValue.QUARTER_NOTE; // To jest stałe
            this.nextFunction = nextFunction;

            if (nextFunction == null)
                return;

            if (Notes.Count != 4)
                return;

            List<List<Note>> permutations = [];

            var tonation = bar.Tonation;
            var chord = new Chord(nextFunction, tonation);
            var notesCount = Notes.Count;

            foreach (var noteSet in chord.Notes)
            {
                var tmp = Old.Utils.Permutations.CreatePermutations(noteSet, notesCount);
                permutations.AddRange(tmp);
            }

            foreach (var perm in permutations)
            {
                var toAdd = new ComputerStack(nextFunction, tonation, Beat + rhytmicValue.RealDuration);

                toAdd.SetSoprano(perm[0], rhytmicValue);
                toAdd.SetAlto(perm[1], rhytmicValue);
                toAdd.SetTenore(perm[2], rhytmicValue);
                toAdd.SetBass(perm[3], rhytmicValue);

                Interval.SetClosestTo(toAdd.Soprano, Soprano);
                Interval.SetClosestTo(toAdd.Alto, Alto);
                Interval.SetClosestTo(toAdd.Tenore, Tenore);
                Interval.SetClosestTo(toAdd.Bass, Bass);
            }
        }

        public override bool SetSoprano(Note? value, RhytmicValue? rhytmicValue = null) => SetNote(ref soprano, value, rhytmicValue);
        public override bool SetAlto(Note? value, RhytmicValue? rhytmicValue = null) => SetNote(ref alto, value, rhytmicValue);
        public override bool SetTenore(Note? value, RhytmicValue? rhytmicValue = null) => SetNote(ref tenore, value, rhytmicValue);
        public override bool SetBass(Note? value, RhytmicValue? rhytmicValue = null) => SetNote(ref bass, value, rhytmicValue);

        private bool SetNote(ref Note? note, Note? newValue, RhytmicValue? rhytmicValue)
        {
            if (newValue == null && rhytmicValue != null)
                throw new ArgumentException("Cannot assigne rhytmic value to an empty note.");
            else if (newValue == null && rhytmicValue == null)
                return true;
            else if (newValue != null && rhytmicValue == null)
                throw new ArgumentException("Cannot assigne no rhytm to a note.");

            if (Duration != 0 && rhytmicValue.RealDuration != Duration)
                return false;

            note = newValue;
            SetDuration(rhytmicValue);
            ValidateDuration();

            return true;
        }

        private void SetDuration(RhytmicValue rhytmicValue)
        {
            foreach (var note in Notes)
            {
                if (note != null)
                {
                    Duration = rhytmicValue.RealDuration;
                    return;
                }
            }

            Duration = 0;
        }

        public override string ToString() => $"{Soprano?.Name}{Alto?.Name}{Tenore?.Name}{Bass?.Name}";
    }
}
