﻿namespace Algorithm.New.Music
{
    public enum Mode { Minor, Major }
    public class Tonation
    {
        public int SharpsCount { get; private set; }
        public int FlatsCount { get; private set; }
        public string Name { get; private set; }
        public Mode Mode { get; private set; }

        public static readonly List<string> SharpsNotes = ["F", "C", "G", "D", "A", "E", "B"];
        public static readonly List<string> FlatsNotes = ["B", "E", "A", "D", "G", "C", "F"];

        // TODO: Json Constructor

        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is Tonation casted)
                return casted.SharpsCount == SharpsCount && casted.FlatsCount == FlatsCount;
            else
                return false;
        }

        public Tonation(string name, Mode mode, int flatsCount, int sharpsCount)
        {
            SharpsCount = sharpsCount;
            FlatsCount = flatsCount;
            Name = name;
            Mode = mode;

            Validate();
        }

        private void Validate()
        {
            if (SharpsCount < 0 || SharpsCount > 10)
                throw new ArgumentException("Invalid sharps count.");

            if (FlatsCount < 0 || FlatsCount > 10)
                throw new ArgumentException("Invalid flats count.");

            if (SharpsCount != 0 && FlatsCount != 0)
                throw new ArgumentException("Invalid accidentals count.");
        }

        public static Tonation GetMirroredVersion(Tonation tonation)
        {
            var flatsCount = tonation.FlatsCount != 0 ? tonation.FlatsCount : -tonation.SharpsCount;
            var newAccidentalsCount = tonation.Mode == Mode.Major ?
                flatsCount + 3 :
                flatsCount - 3;

            if (newAccidentalsCount > 7 || newAccidentalsCount < -7)
            {
                // TODO:
                // A co jak mamy np. Ces dur i chcemy zrobić ces moll?
                var x = 0;
            }

            var newFlatsCount = newAccidentalsCount >= 0 ? newAccidentalsCount : 0;
            var newSharpsCount = newAccidentalsCount <= 0 ? -newAccidentalsCount : 0;

            var newTonation = tonation.Mode == Mode.Minor ?
                GetTonation(newSharpsCount, newFlatsCount).First(x => x.Mode == Mode.Major) :
                GetTonation(newSharpsCount, newFlatsCount).First(x => x.Mode == Mode.Minor);

            return newTonation;
        }

        public static IEnumerable<Tonation> GetTonation(int sharpsCount, int flatsCount)
        {
            List<Tonation> tonations = [
                CFlatMajor, CMajor, CMinor, CSharpMajor, CSharpMinor,
                DFlatMajor, DMajor, DMinor, DSharpMinor, EFlatMajor,
                EFlatMinor, EMajor, EMinor, FMajor, FMinor, FSharpMajor,
                FSHarpMinor, GFlatMajor, GMajor, GMinor, GSharpMinor,
                AFlatMajor, AFlatMinor, AMajor, AMinor, ASharpMinor,
                BFlatMajor, BFlatMinor, BMajor, BMinor,

                DFlatMinor, GFlatMinor, CFlatMinor, FFlatMajor, BDoubleFlatMajor,
                EDoubleFlatMajor, GSharpMajor, DSharpMajor, ASharpMajor,
                ESharpMinor, BSharpMinor, FDoubleSharpMinor
            ];

            return tonations
                .Where(t => t.FlatsCount == flatsCount && t.SharpsCount == sharpsCount) ??
                throw new ArgumentException("Invalid number of accidentals.");
        }

        public static readonly Tonation CFlatMajor = new("Cb", Mode.Major, 7, 0);
        public static readonly Tonation CMajor = new("C", Mode.Major, 0, 0);
        public static readonly Tonation CMinor = new("C", Mode.Minor, 3, 0);
        public static readonly Tonation CSharpMajor = new("C#", Mode.Major, 0, 7);
        public static readonly Tonation CSharpMinor = new("C#", Mode.Minor, 0, 4);
        public static readonly Tonation DFlatMajor = new("Db", Mode.Major, 5, 0);
        public static readonly Tonation DMajor = new("D", Mode.Major, 0, 2);
        public static readonly Tonation DMinor = new("D", Mode.Minor, 1, 0);
        public static readonly Tonation DSharpMinor = new("D#", Mode.Minor, 0, 6);
        public static readonly Tonation EFlatMajor = new("Eb", Mode.Major, 3, 0);
        public static readonly Tonation EFlatMinor = new("Eb", Mode.Minor, 6, 0);
        public static readonly Tonation EMajor = new("E", Mode.Major, 0, 4);
        public static readonly Tonation EMinor = new("E", Mode.Minor, 0, 1);
        public static readonly Tonation FMajor = new("F", Mode.Major, 1, 0);
        public static readonly Tonation FMinor = new("F", Mode.Minor, 4, 0);
        public static readonly Tonation FSharpMajor = new("F#", Mode.Major, 0, 6);
        public static readonly Tonation FSHarpMinor = new("F#", Mode.Minor, 0, 3);
        public static readonly Tonation GFlatMajor = new("Gb", Mode.Major, 6, 0);
        public static readonly Tonation GMajor = new("G", Mode.Major, 0, 1);
        public static readonly Tonation GMinor = new("G", Mode.Minor, 2, 0);
        public static readonly Tonation GSharpMinor = new("G#", Mode.Minor, 0, 5);
        public static readonly Tonation AFlatMajor = new("Ab", Mode.Major, 4, 0);
        public static readonly Tonation AFlatMinor = new("Ab", Mode.Minor, 7, 0);
        public static readonly Tonation AMajor = new("A", Mode.Major, 0, 3);
        public static readonly Tonation AMinor = new("A", Mode.Minor, 0, 0);
        public static readonly Tonation ASharpMinor = new("A#", Mode.Minor, 0, 7);
        public static readonly Tonation BFlatMajor = new("Bb", Mode.Major, 2, 0);
        public static readonly Tonation BFlatMinor = new("Bb", Mode.Minor, 5, 0);
        public static readonly Tonation BMajor = new("B", Mode.Major, 0, 5);
        public static readonly Tonation BMinor = new("B", Mode.Minor, 0, 2);

        // Tonacje teoretyczne:
        public static readonly Tonation DFlatMinor = new("Db", Mode.Minor, 8, 0);
        public static readonly Tonation GFlatMinor = new("Gb", Mode.Minor, 9, 0);
        public static readonly Tonation CFlatMinor = new("Cb", Mode.Minor, 10, 0);

        public static readonly Tonation FFlatMajor = new("Fb", Mode.Major, 8, 0);
        public static readonly Tonation BDoubleFlatMajor = new("Bbb", Mode.Major, 9, 0);
        public static readonly Tonation EDoubleFlatMajor = new("Ebb", Mode.Major, 10, 0);

        public static readonly Tonation GSharpMajor = new("G#", Mode.Major, 0, 8);
        public static readonly Tonation DSharpMajor = new("D#", Mode.Major, 0, 9);
        public static readonly Tonation ASharpMajor = new("A#", Mode.Major, 0, 10);

        public static readonly Tonation ESharpMinor = new("E#", Mode.Minor, 0, 8);
        public static readonly Tonation BSharpMinor = new("B#", Mode.Minor, 0, 9);
        public static readonly Tonation FDoubleSharpMinor = new("Fx", Mode.Minor, 0, 10);
    }
}
