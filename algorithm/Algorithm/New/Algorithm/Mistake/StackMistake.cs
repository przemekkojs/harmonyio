using Algorithm.New.Algorithm.Rules;
using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Mistake
{
    public class StackMistake : Mistake
    {
        public List<Stack> Stacks { get; set; } = [];
        public Rule? Rule { get; set; } = null;

        // FOR JSON MISTAKE LOGIC
        public List<int> BarIndexes { get; private set; }
        public List<int> VerticalIndexes { get; private set; }
        public string RuleName { get; private set; }

        public StackMistake(List<Stack> stacks, Rule? rule)
        {
            Rule = rule;
            Stacks = stacks;
            RuleName = rule != null ? rule.Name : "Nieokreślona zasada";

            BarIndexes = [];
            VerticalIndexes = [];

            foreach (var stack in Stacks)
            {
                var barIndex = stack.Index.Bar;
                var verticalIndex = stack.Index.Position;

                BarIndexes.Add(barIndex);
                VerticalIndexes.Add(verticalIndex);
            }

            BarIndexes = BarIndexes
                .Distinct()
                .ToList();

            VerticalIndexes = VerticalIndexes
                .Distinct()
                .ToList();
        }

        public override int Quantity => Stacks.Count;

        public override void GenerateDescription() =>
            Description = Mistake.GenerateStackMistakeDescription(BarIndexes, VerticalIndexes, RuleName);
    }
}