using Algorithm.New.Music;

namespace Algorithm.New.Algorithm.Rules
{
    public class VoiceCrossing : Rule
    {
        public VoiceCrossing() : base("Voice Crossing", "") { }

        public override bool IsSatisfied(params Stack[] functions)
        {
            if (functions.Length != 2)
                return false;

            var stack1 = functions[0];
            var stack2 = functions[1];

            // TODO: Implement

            return true;
        }
    }
}
