using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Mistake
{
    public class NoteMistake : Mistake
    {
        public List<string> Notes { get; set; } = [];
        public Stack? Stack { get; set; } = null;

        // FOR JSON MISTAKE LOGIC
        public int BarIndex { get; set; }
        public int VerticalIndex { get; set; }
        public List<string> Voices { get; set; } = [];

        public override int Quantity => Notes.Count;

        public override void GenerateDescription()
        {
            if (Notes.Count == 0 || Stack == null)
            {
                Description = "";
                return;
            }

            string postfix = Notes.Count == 1 ? "u" : "ów";
            Description = $"Błędy dźwięk{ postfix } [{ string.Join(", ", Notes) }] w funkcji: Takt: {Stack.Index.Bar}, Miara: {Stack.Index.Position}.";
        }

        // FOR JSON MISTAKE LOGIC
        public void SetInfo ()
        {
            if (Stack == null)
                return;

            BarIndex = Stack.Index.Bar;
            VerticalIndex = Stack.Index.Position;
        }
    }
}
