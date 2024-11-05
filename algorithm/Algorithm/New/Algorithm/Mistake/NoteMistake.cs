using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Mistake
{
    public class NoteMistake : Mistake
    {
        public int BarIndex { get; set; }
        public int VerticalIndex { get; set; }
        public List<string> Voices { get; set; }

        public override int Quantity => Voices.Count;

        public NoteMistake(List<Note?> notes, Stack? stack)
        {
            Voices = [];

            foreach (var note in notes)
            {
                if (note == null)
                    continue;

                var noteName = note.Name;
                var voice = GetVoiceFromStack(note, stack);

                if (voice != null)
                    Voices.Add(voice);
            }

            BarIndex = stack != null ? stack.Index.Bar : 0;
            VerticalIndex = stack != null ? stack.Index.Position : 0;
        }

        public static string? GetVoiceFromStack(Note note, Stack? stack)
        {
            int index = 0;
            List<string> voices = ["S", "A", "T", "B"];

            while (index < stack?.Notes.Count)
            {
                var toCheck = stack.Notes[index];

                if (toCheck == null)
                {
                    index++;
                    continue;
                }

                if (toCheck.Equals(note))
                    return voices[index];
            }

            return null;
        }

        public override void GenerateDescription() => 
            Description = Mistake.GenerateNoteMistakeDescription(Voices, BarIndex, VerticalIndex);
    }
}
