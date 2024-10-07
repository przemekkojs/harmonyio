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

        private readonly int numberOfFlats;
        private readonly int numberOfSharps;

        private readonly string name;
        private readonly Mode mode;

        private readonly Scale scale;

        private readonly int sharpsCount;
        private readonly int flatsCount;

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
            int startIndex = allNotes.IndexOf(name);
            int accidentalsCount = sharpsCount + flatsCount;
            string accidentalSign = "";
            List<string> accidentalsList = [];            

            if (accidentalsCount != 0)
            {
                accidentalsList = flatsCount != 0 ? flatsQueue : sharpsQueue;
                accidentalSign = flatsCount != 0 ? "b" : "#";
            }            

            for (int index = 0; index < NOTES_IN_TONATION; index++)
            {
                int actualIndex = (index + startIndex) % NOTES_IN_TONATION;
                string currentNote = allNotes[actualIndex];

                for (int accidentalIndex = 0; accidentalIndex < accidentalsCount; accidentalIndex++)
                {
                    if (currentNote == accidentalsList[accidentalIndex])
                    {
                        currentNote += accidentalSign;
                    }
                }

                scale.NoteNames.Add(currentNote);
            }
        }

        private void Validate()
        {
            if (sharpsCount < 0 || sharpsCount > 7)
                throw new ArgumentException("Invalid sharps count.");

            if (flatsCount < 0 || flatsCount > 7)
                throw new ArgumentException("Invalid flats count.");

            if (sharpsCount != 0 && flatsCount != 0)
                throw new ArgumentException("Invalid accidentals count.");
        }
    }
}
