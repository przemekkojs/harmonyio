using Algorithm.New.Algorithm.Rules.Problem;

namespace Algorithm.New.Algorithm.Mistake.Problem
{
    public class ProblemMistake
    {
        public string Description { get; set; }
        public int BarIndex { get; set; }
        public int FunctionIndex { get; set; }
        public Rule? Rule { get; private set; } = null;

        public ProblemMistake(int barIndex, int functionIndex, string description = "")
        {
            BarIndex = barIndex;
            FunctionIndex = functionIndex;
            Description = description;
        }

        public void GenerateDescription(Rule rule)
        {
            Rule = rule;
            var ruleName = rule.Name;
            Description = $"Niespełniona zasada {ruleName} w takcie {BarIndex + 1}, funkcji {FunctionIndex + 1}.";
        }
    }
}
