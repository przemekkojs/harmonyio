using Algorithm.New.Algorithm.Rules;
using Algorithm.New.Music;
using System.Text.Json.Serialization;

namespace Algorithm.New.Algorithm.Mistake
{
    public class StackMistake : Mistake
    {
        public List<int> BarIndexes { get; private set; }
        public List<int> VerticalIndexes { get; private set; }
        public string RuleName { get; private set; }

        [JsonConstructor]
        public StackMistake(List<int> barIndexes, List<int> verticalIndexes, string ruleName)
        {
            BarIndexes = barIndexes;
            VerticalIndexes = verticalIndexes;
            RuleName = ruleName;
            GenerateDescription();
        }

        public StackMistake(List<Stack> stacks, Rule? rule)
        {
            RuleName = rule != null ? rule.Name : "Nieokreślona zasada";

            BarIndexes = [];
            VerticalIndexes = [];

            foreach (var stack in stacks)
            {
                var barIndex = stack.Index.Bar;
                var verticalIndex = stack.Index.Position;

                BarIndexes.Add(barIndex + 1);
                VerticalIndexes.Add(verticalIndex + 1);
            }

            BarIndexes = BarIndexes
                .Distinct()
                .ToList();

            VerticalIndexes = VerticalIndexes
                .Distinct()
                .ToList();

            GenerateDescription();
        }

        public override int Quantity => VerticalIndexes.Count;

        public override void GenerateDescription() =>
            Description = Mistake.GenerateStackMistakeDescription(BarIndexes, VerticalIndexes, RuleName);
    }
}