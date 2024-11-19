namespace Algorithm.New.Music
{
    public class Note
    {
        public string Name { get; private set; }
        public int Octave { get; set; }
        public string Accidental { get; private set; }

        private static readonly List<string> PossibleAccidentals = ["#", "b", "x", "bb", "bq", ""];

        public Note (string name, int octave)
        {
            Name = name;
            Octave = octave;

            DeductAccidental();
        }

        public void SetNewName(string newName)
        {
            Name = newName;
            DeductAccidental();
        }

        public Note (string name, int octave, string accidental)
        {
            if (!PossibleAccidentals.Contains(accidental))
                throw new ArgumentException("Invalid accidental");

            if (!accidental.Equals("bq"))
                Name = name + accidental;
            else
                Name = name;

            Octave = octave;
            Accidental = accidental;
        }

        private void DeductAccidental()
        {
            var accidentalString = Name[1..];

            if (!PossibleAccidentals.Contains(accidentalString))
                throw new ArgumentException("Invalid note parameters");

            Accidental = accidentalString;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is Note casted)
            {
                var checkName = Name == casted.Name;
                var checkOctave = Octave == casted.Octave;

                return checkName && checkOctave;
            }

            return false;
        }

        public override string ToString() => $"{Name}{Octave}";
    }
}
