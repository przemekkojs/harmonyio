using System;

namespace Algorithm.Music
{
    public sealed class Tonation
    {
        public const int NOTES_IN_TONATION = 7;

        public int NumberOfFlats { get => numberOfFlats; }
        public int NumberOfSharps { get => numberOfSharps; }
        public string Name { get => name; }
        public Mode Mode { get => mode; }
        public string this[int index] => scale[index];
        public Scale Scale { get => scale; }

        private readonly int numberOfFlats;
        private readonly int numberOfSharps;

        private readonly string name;
        private readonly Mode mode;

        private readonly Scale scale;

        public Tonation(string name, Mode mode, int numberOfFlats, int numberOfSharps)
        {
            this.name = name;
            this.mode = mode;
            this.numberOfFlats = numberOfFlats;
            this.numberOfSharps = numberOfSharps;
            this.scale = new Scale(name, mode);

            Validate();
            DeductNotes();            
        }

        private void DeductNotes()
        {
            int startIndex = Constants.Constants.AllNotes.IndexOf(name[0].ToString());
            int accidentalsCount = numberOfFlats + numberOfSharps;
            string accidentalSign = "";
            List<string> accidentalsList = [];            

            if (accidentalsCount != 0)
            {
                accidentalsList = numberOfFlats != 0 ? Constants.Constants.FlatsQueue : Constants.Constants.SharpsQueue;
                accidentalSign = numberOfFlats != 0 ? "b" : "#";
            }

            for (int index = 0; index < Constants.Constants.AllNotes.Count; index++)
            {
                int actualIndex = (index + startIndex) % NOTES_IN_TONATION;
                string currentNote = Constants.Constants.AllNotes[actualIndex];

                for (int accidentalIndex = 0; accidentalIndex < accidentalsCount; accidentalIndex++)
                {
                    if (currentNote.Equals(accidentalsList[accidentalIndex]))
                    {
                        currentNote += accidentalSign;
                    }
                }

                scale.NoteNames.Add(currentNote);
            }

            if (scale.NoteNames.Count != 7)
                throw new InvalidOperationException("Something went wrong.");
        }

        private void Validate()
        {
            if (numberOfSharps < 0 || numberOfSharps > 7)
                throw new ArgumentException("Invalid sharps count.");

            if (numberOfFlats < 0 || numberOfFlats > 7)
                throw new ArgumentException("Invalid flats count.");

            if (numberOfSharps != 0 && numberOfFlats != 0)
                throw new ArgumentException("Invalid accidentals count.");
        }
    }
}
