using Algorithm.New.Algorithm.Rules;
using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Mistake
{
    public class FunctionMistake : Mistake
    {
        public List<Function> Functions { get; set; } = [];
        public Rule? Rule { get; set; } = null;

        public override void GenerateDescription()
        {
            if (Functions.Count == 0 || Rule == null)
            {
                Description = "";
                return;
            }

            string postfix = Functions.Count == 1 ? "i" : "ach";
            Description = $"Błąd w funkcj{postfix}: ";

            if (Functions.Count == 1)
            {
                var function = Functions[0];
                Description += $"Takt {function.Bar}, Miara {function.Beat}. Niespełniona zasada {Rule.Name}.";
            }
            else
            {
                var function1 = Functions[0];
                var function2 = Functions[1];

                Description += $"(Takt {function1.Bar}, Miara {function1.Beat}), (Takt {function2.Bar}, Miara {function2.Beat}). Niespełniona zasada {Rule.Name}.";
            }
        }
    }
}