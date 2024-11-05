using Algorithm.New.Algorithm.Rules;
using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Mistake
{
    public class StackMistake : Mistake
    {
        // FOR JSON MISTAKE LOGIC
        public List<int> BarIndexes { get; private set; }
        public List<int> VerticalIndexes { get; private set; }
        public string RuleName { get; private set; }

        public StackMistake(List<Stack> stacks, Rule? rule)
        {
            RuleName = rule != null ? rule.Name : "Nieokreślona zasada";

            BarIndexes = [];
            VerticalIndexes = [];

            foreach (var stack in stacks)
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

        public override int Quantity => VerticalIndexes.Count;

        public override void GenerateDescription() =>
            Description = Mistake.GenerateStackMistakeDescription(BarIndexes, VerticalIndexes, RuleName);
    }
}