using Algorithm.Music;
using System.Linq;

namespace Algorithm.Algorithm
{
    public class Stack
    {        
        public int Duration { get; set; }
        public int Beat { get; private set; }
        public Function BaseChord { get => baseFunction; }
        public Tonation Tonation { get => tonation; }

        protected readonly Function baseFunction;
        protected readonly Tonation tonation;
        protected int startBeat;
        protected int duration;

        protected Note? soprano;
        protected Note? alto;
        protected Note? tenore;
        protected Note? bass;

        public Stack(Function baseFunction, Tonation tonation, int startBeat)
        {
            soprano = null;
            alto = null;
            tenore = null;
            bass = null;

            this.startBeat = startBeat;
            this.baseFunction = baseFunction;
            this.tonation = tonation;
        }

        public bool AllNotesValid()
        {
            Chord checker = new(baseFunction, tonation);
            List<Note> validNotes = checker.Notes
                .SelectMany(x => x)
                .Distinct()
                .ToList();

            List<Note?> notesToCheck = [soprano, alto, tenore, bass];
            
            foreach(var note in notesToCheck)
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
    }
}
