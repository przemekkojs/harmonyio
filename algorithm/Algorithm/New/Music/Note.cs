namespace Algorithm.New.Music
{
    public class Note
    {
        public string Name { get; private set; }
        public int Octave { get; set; }
        public string Accidental { get; private set; }

        private static readonly List<string> PossibleAccidentals = ["#", "b", "x", "bb", "bq"];

        public Note (string name, int octave)
        {
            Name = name;
            Octave = octave;

            DeductAccidental();
        }

        public Note (string name, int octave, string accidental)
        {
            Name = name;
            Octave = octave;
            Accidental = accidental;
        }

        private void DeductAccidental()
        {
            var accidentalString = Name[1..];

            if (PossibleAccidentals.Contains(accidentalString))
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
    }
}
