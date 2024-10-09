using Algorithm.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm.Algorithm
{
    public class Stack
    {
        public Note Soprano {  get; private set; }
        public Note Alto { get; private set; }
        public Note Tenore { get; private set; }
        public Note Bass { get; private set; }

        public int Duration { get; private set; }
        public int Beat { get; private set; }

        public Stack(Note soprano, Note alto, Note tenore, Note bass, int duration, int beat)
        {
            Soprano = soprano;
            Alto = alto;
            Tenore = tenore;
            Bass = bass;

            Duration = duration;
            Beat = beat;
        }

        public Stack(List<Note> notes, int duration, int beat)
        {
            Soprano = notes[0];
            Alto = notes[1];
            Tenore = notes[2];
            Bass = notes[3];

            Duration = duration;
            Beat = beat;
        }
    }
}
