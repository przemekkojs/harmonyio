using Algorithm.New.Music;
using System.Text.Json.Serialization;

namespace Algorithm.New.Algorithm.Mistake.Solution
{
    public class NoteMistake : Mistake
    {
        public int BarIndex { get; set; }
        public int VerticalIndex { get; set; }

        public override int Quantity => 1;

        [JsonConstructor]
        public NoteMistake(int barIndex, int verticalIndex, int code)
        {
            if (barIndex < 0)
                barIndex = 0;

            if (verticalIndex < 0)
                verticalIndex = 0;

            BarIndex = barIndex;
            VerticalIndex = verticalIndex;
            MistakeCode = code;

            GenerateDescription();
        }

        public NoteMistake(Note? note, Stack? stack)
        {
            if (note == null)
                MistakeCode = -1;
            else
                MistakeCode = GetVoiceIndexFromStack(note, stack);

            BarIndex = stack != null ? stack.Index.Bar : 0;
            VerticalIndex = stack != null ? stack.Index.Position : 0;

            GenerateDescription();
        }

        public static int GetVoiceIndexFromStack(Note note, Stack? stack)
        {
            int index = 0;

            while (index < stack?.Notes.Count)
            {
                var toCheck = stack.Notes[index];

                if (note.Equals(toCheck))
                    return index;

                index++;
            }

            return -1;
        }

        public override void GenerateDescription() => Description = ([BarIndex], [VerticalIndex], MistakeCode);
    }
}
