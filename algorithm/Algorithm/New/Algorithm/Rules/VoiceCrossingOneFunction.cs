using Algorithm.New.Utils;

namespace Algorithm.New.Algorithm.Rules
{
    public class VoiceCrossingOneFunction : Rule
    {
        public VoiceCrossingOneFunction() : base(
            name: "Krzyżowanie głosów w ramach jednej funkcji",
            description: "Czy nuty w ramach jednej funkcji nie krzyżują się wysokościami w ramach sąsiednich głosów?",
            oneFunction: true) { }

        public override bool IsSatisfied(params Music.Stack[] stacks)
        {
            if (!ValidateParametersCount(stacks))
                return false;

            var stack = stacks[0];

            bool sopranoAlto = Interval.IsLower(stack.Soprano, stack.Alto);
            bool altoTenore = Interval.IsLower(stack.Alto, stack.Tenore);
            bool tenoreBass = Interval.IsLower(stack.Tenore, stack.Bass);

            return sopranoAlto && altoTenore && tenoreBass;
        }
    }
}
