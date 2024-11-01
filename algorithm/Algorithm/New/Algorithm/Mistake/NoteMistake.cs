using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Mistake
{
    public class NoteMistake : Mistake
    {
        public List<Note> Notes { get; set; } = [];
        public Stack? Stack { get; set; } = null;

        public override void GenerateDescription()
        {
            if (Notes.Count == 0 || Stack == null)
            {
                Description = "";
                return;
            }

            string postfix = Notes.Count == 1 ? "u" : "ów";
            Description = $"Błędy dźwięk{ postfix } [{ string.Join(", ", Notes) }] w funkcji: Takt: {Stack.Index.Bar}, Miara: {Stack.Index.Beat}.";
        }
    }
}
