using Algorithm.Music;

namespace Algorithm.Communication
{
    public class FunctionToParse
    {
        public Note Soprano { get; set; }
        public Note Alto { get; set; }
        public Note Tenore { get; set; }
        public Note Bass { get; set; }

        public int Duration { get; set; }

        public FunctionToParse(List<Note> notes, int duration)
        {
            Soprano = notes[0];
            Alto = notes[1];
            Tenore = notes[2];
            Bass = notes[3];
            Duration = duration;
        }
    }

    public static class FunctionParser
    {
        public static void ParseFunction(Function function)
        {
            
        }
    }
}
