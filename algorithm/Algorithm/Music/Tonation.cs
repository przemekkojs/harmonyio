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

        public static Tonation GetTonation(string name, string mode)
        {
            Mode realMode = mode.ToLower() switch
            {
                "minor" => Mode.MINOR,
                "major" => Mode.MAJOR,
                _ => throw new ArgumentException("Invalid mode")
            };

            List<Tonation> tonations = new List<Tonation>
            {
                CFlatMajor, CMajor, CMinor, CSharpMajor, CSharpMinor,
                DFlatMajor, DMajor, DMinor, DSharpMinor, EFlatMajor,
                EFlatMinor, EMajor, EMinor, FMajor, FMinor, FSharpMajor,
                FSHarpMinor, GFlatMajor, GMajor, GMinor, GSharpMinor,
                AFlatMajor, AFlatMinor, AMajor, AMinor, ASharpMinor,
                BFlatMajor, BFlatMinor, BMajor, BMinor
            };

            return tonations.FirstOrDefault(t => t.Name == name && t.Mode == realMode) ?? throw new ArgumentException("Tonation not found");
        }

        public static readonly Tonation CFlatMajor = new("Cb", Mode.MAJOR, 7, 0);
        public static readonly Tonation CMajor = new("C", Mode.MAJOR, 0, 0);
        public static readonly Tonation CMinor = new("C", Mode.MINOR, 3, 0);
        public static readonly Tonation CSharpMajor = new("C#", Mode.MAJOR, 0, 7);
        public static readonly Tonation CSharpMinor = new("C#", Mode.MINOR, 0, 4);
        public static readonly Tonation DFlatMajor = new("Db", Mode.MAJOR, 5, 0);
        public static readonly Tonation DMajor = new("D", Mode.MAJOR, 0, 2);
        public static readonly Tonation DMinor = new("D", Mode.MINOR, 1, 0);
        public static readonly Tonation DSharpMinor = new("D#", Mode.MINOR, 0, 6);
        public static readonly Tonation EFlatMajor = new("Eb", Mode.MAJOR, 3, 0);
        public static readonly Tonation EFlatMinor = new("Eb", Mode.MINOR, 6, 0);
        public static readonly Tonation EMajor = new("E", Mode.MAJOR, 0, 4);
        public static readonly Tonation EMinor = new("E", Mode.MINOR, 0, 1);
        public static readonly Tonation FMajor = new("F", Mode.MAJOR, 1, 0);
        public static readonly Tonation FMinor = new("F", Mode.MINOR, 4, 0);
        public static readonly Tonation FSharpMajor = new("F#", Mode.MAJOR, 0, 6);
        public static readonly Tonation FSHarpMinor = new("F#", Mode.MINOR, 0, 3);
        public static readonly Tonation GFlatMajor = new("Gb", Mode.MAJOR, 6, 0);
        public static readonly Tonation GMajor = new("G", Mode.MAJOR, 0, 1);
        public static readonly Tonation GMinor = new("G", Mode.MINOR, 2, 0);
        public static readonly Tonation GSharpMinor = new("G#", Mode.MINOR, 0, 5);
        public static readonly Tonation AFlatMajor = new("Ab", Mode.MAJOR, 4, 0);
        public static readonly Tonation AFlatMinor = new("Ab", Mode.MINOR, 7, 0);
        public static readonly Tonation AMajor = new("A", Mode.MAJOR, 0, 3);
        public static readonly Tonation AMinor = new("A", Mode.MINOR, 0, 0);
        public static readonly Tonation ASharpMinor = new("A#", Mode.MINOR, 0, 7);
        public static readonly Tonation BFlatMajor = new("Bb", Mode.MAJOR, 2, 0);
        public static readonly Tonation BFlatMinor = new("Bb", Mode.MINOR, 5, 0);
        public static readonly Tonation BMajor = new("B", Mode.MAJOR, 0, 5);
        public static readonly Tonation BMinor = new("B", Mode.MINOR, 0, 2);
    }
}
