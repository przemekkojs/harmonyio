using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules.Solution
{
    // Tutaj pamiętać, że dla dominanty jest osobna zasada
    public sealed class SixthUp : Rule
    {
        public SixthUp(int id, string name, string description, bool oneFunction = false) : base(
            id,
            name,
            description,
            oneFunction) { }

        public override bool IsSatisfied(string additionalParamsJson = "", params Stack[] stacks)
        {
            throw new NotImplementedException();
        }
    }
}
