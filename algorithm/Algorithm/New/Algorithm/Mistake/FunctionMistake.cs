﻿using Algorithm.New.Algorithm.Rules;
using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Mistake
{
    public class StackMistake : Mistake
    {
        public List<Stack> Stacks { get; set; } = [];
        public Rule? Rule { get; set; } = null;

        public override void GenerateDescription()
        {
            if (Stacks.Count == 0 || Rule == null)
            {
                Description = "";
                return;
            }

            string postfix = Stacks.Count == 1 ? "i" : "ach";
            Description = $"Błąd w funkcj{postfix}: ";

            if (Stacks.Count == 1)
            {
                var function = Stacks[0];
                Description += $"Takt {function.Index.Bar}, Miara {function.Index.Beat}. Niespełniona zasada {Rule.Name}.";
            }
            else
            {
                var function1 = Stacks[0];
                var function2 = Stacks[1];

                Description += $"(Takt {function1.Index.Bar}, Miara {function1.Index.Beat}), (Takt {function2.Index.Bar}, Miara {function2.Index.Beat}). Niespełniona zasada {Rule.Name}.";
            }
        }
    }
}