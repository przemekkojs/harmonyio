using Algorithm.New.Algorithm.Rules.Solution;
using Algorithm.New.Music;
using System.Text.Json.Serialization;

namespace Algorithm.New.Algorithm.Mistake.Solution
{
    public class StackMistake : Mistake
    {
        public List<int> BarIndexes { get; private set; }
        public List<int> VerticalIndexes { get; private set; }

        public StackMistake(List<int> barIndexes, List<int> verticalIndexes, Rule rule)
        {
            BarIndexes = barIndexes;
            VerticalIndexes = verticalIndexes;
            MistakeCode = rule.Id;

            GenerateDescription();
        }

        public StackMistake(List<Stack> stacks, Rule? rule)
        {
            MistakeCode = rule != null ? rule.Id : 0;

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

            GenerateDescription();
        }

        public override int Quantity => VerticalIndexes.Count;

        public override void GenerateDescription() => Description = (BarIndexes, VerticalIndexes, MistakeCode);
    }
}