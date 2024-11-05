using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Parsers.NoteParser
{
    public class NoteParseResult
    {
        public int RhytmicValue { get; private set; }
        public Note Note { get; private set; }
        public int Bar { get; private set; }
        public int Stack { get; private set; }

        public NoteParseResult(Note note, int value, int bar, int stack)
        {
            RhytmicValue = value;
            Note = note;
            Bar = bar;
            Stack = stack;
        }
    }
}
