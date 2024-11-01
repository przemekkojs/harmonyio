using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules
{
    public class VoiceCrossing : Rule
    {
        public VoiceCrossing() : base("Voice Crossing", "") { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (functions.Length != 2)
                return false;

            var function1 = functions[0];
            var function2 = functions[1];

            // TODO: Implement

            return true;
        }
    }
}
