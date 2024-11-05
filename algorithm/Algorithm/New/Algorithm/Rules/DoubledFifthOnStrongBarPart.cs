using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules
{
    public class DoubledFifthOnStrongBarPart : Rule
    {
        public DoubledFifthOnStrongBarPart() : base(
            name: "Dwojenie kwinty na mocnej części taktu",
            description: "Czy w ramach jednej funkcji, przypadającej na mocnej części taktu, podwojona została kwinta, przy oparciu na kwincie?",
            oneFunction: true) { }

        public override bool IsSatisfied(params Stack[] functions)
        {
            throw new NotImplementedException();
        }
    }
}
