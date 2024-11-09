using Algorithm.New.Music;
using System.Text.Json.Serialization;

namespace Algorithm.New.Algorithm.Mistake
{
    public class NoteMistake : Mistake
    {
        public int BarIndex { get; set; }
        public int VerticalIndex { get; set; }
        public string Voice { get; set; }

        public override int Quantity => 1;

        public static NoteMistake CreateEmptyNoteMistake(int barIndex, int verticalIndex, string voice)
        {
            return new NoteMistake(barIndex, verticalIndex, voice)
            {
                Description = (
                    [barIndex],
                    [verticalIndex],
                    $"Brakuje nuty w głosie {voice}."
                )
            };
        }

        [JsonConstructor]
        public NoteMistake(int barIndex, int verticalIndex, string voice)
        {
            BarIndex = barIndex;
            VerticalIndex = verticalIndex;
            Voice = voice;

            GenerateDescription();
        }

        public NoteMistake(Note? note, Stack? stack)
        {            
            if (note == null)
                Voice = string.Empty;
            else
            {
                var voice = GetVoiceFromStack(note, stack);

                if (voice != null)
                    Voice = voice;
                else
                    Voice = string.Empty;
            }            

            BarIndex = stack != null ? stack.Index.Bar : 0;
            VerticalIndex = stack != null ? stack.Index.Position : 0;
            GenerateDescription();
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

                index++;
            }

            return null;
        }

        public override void GenerateDescription() => 
            Description = Mistake.GenerateNoteMistakeDescription(Voice, BarIndex, VerticalIndex);
    }
}
