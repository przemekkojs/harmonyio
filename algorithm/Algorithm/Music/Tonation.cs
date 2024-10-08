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

        private static readonly List<string> sharpsQueue = ["F", "C", "G", "D", "A", "E", "B"];
        private static readonly List<string> flatsQueue = ["B", "E", "A", "D", "G", "C", "F"];
        private static readonly List<string> allNotes = ["C", "D", "E", "F", "G", "A", "B"];        

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
            int startIndex = allNotes.IndexOf(name[0].ToString());
            int accidentalsCount = numberOfFlats + numberOfSharps;
            string accidentalSign = "";
            List<string> accidentalsList = [];            

            if (accidentalsCount != 0)
            {
                accidentalsList = numberOfFlats != 0 ? flatsQueue : sharpsQueue;
                accidentalSign = numberOfFlats != 0 ? "b" : "#";
            }

            for (int index = 0; index < allNotes.Count; index++)
            {
                int actualIndex = (index + startIndex) % NOTES_IN_TONATION;
                string currentNote = allNotes[actualIndex];

                for (int accidentalIndex = 0; accidentalIndex < accidentalsCount; accidentalIndex++)
                {
                    if (currentNote.Equals(accidentalsList[accidentalIndex]))
                    {
                        currentNote += accidentalSign;
                    }
                }

                scale.NoteNames.Add(currentNote);
            }
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
