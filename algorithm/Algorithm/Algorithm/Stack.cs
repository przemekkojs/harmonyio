using Algorithm.Music;
using System.Linq;

namespace Algorithm.Algorithm
{
    public abstract class Stack
    {
        public Note? Soprano { get => soprano; }
        public Note? Alto { get => alto; }
        public Note? Tenore { get => tenore; }
        public Note? Bass { get => bass; }

        public int Duration { get; set; }
        public int Beat { get; private set; }
        public Function BaseFunction { get => baseFunction; }
        public Tonation Tonation { get => tonation; }

        public List<Note?> Notes { get => [soprano, alto, tenore, bass]; }

        protected readonly Function baseFunction;
        protected readonly Tonation tonation;

        protected Note? soprano;
        protected Note? alto;
        protected Note? tenore;
        protected Note? bass;

        public abstract bool SetSoprano(Note? value, RhytmicValue rhytmicValue);
        public abstract bool SetAlto(Note? value, RhytmicValue rhytmicValue);
        public abstract bool SetTenore(Note? value, RhytmicValue rhytmicValue);
        public abstract bool SetBass(Note? value, RhytmicValue rhytmicValue);

        public Stack(Function baseFunction, Tonation tonation, int startBeat)
        {
            soprano = null;
            alto = null;
            tenore = null;
            bass = null;

            this.Beat = startBeat;
            this.baseFunction = baseFunction;
            this.tonation = tonation;

            ValidateBeat();
        }

        public bool AllNotesValid()
        {
            Chord checker = new(baseFunction, tonation);
            List<Note> validNotes = checker.Notes
                .SelectMany(x => x)
                .Distinct()
                .ToList();

            List<Note?> notesToCheck = [soprano, alto, tenore, bass];

            foreach (var note in notesToCheck)
            {
                if (note == null)
                    return false;

                if (!validNotes.Contains(note))
                    return false;
            }

            return true;
        }

        public bool AllNotesPartiallyValid()
        {
            // TODO
            return true;
        }

        public List<Note> ValidNotes()
        {
            // TODO
            throw new NotImplementedException();
        }

        public List<Note> PartiallyValidNotes()
        {
            // TODO
            throw new NotImplementedException();
        }

        public List<Note> InvalidNotes()
        {
            // TODO
            throw new NotImplementedException();
        }

        public void ValidateDuration()
        {
            if (!(new List<int>() { 1, 2, 3, 4, 6, 8, 12, 16 }.Contains(Duration)))
                throw new ArgumentException("Invalid function duration.");
        }

        public void ValidateBeat()
        {
            if (Beat < 0)
                throw new ArgumentException("Invalid beat.");
        }

        // TODO: Poprawić!!!!!!!!!!!!!!!!!!!!!!!
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is not Stack parsed)
                return false;

            if (parsed == null)
                return false;

            return parsed.Soprano.Equals(Soprano) &&
                parsed.Alto.Equals(Alto) &&
                parsed.Tenore.Equals(Tenore) &&
                parsed.Bass.Equals(Bass);
        }
    }
}