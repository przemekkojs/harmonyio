using Algorithm.Music;
using System;
namespace Algorithm.Algorithm.Rules
{
    public sealed class VoiceDistance : Rule
    {
        public VoiceDistance() : base("Voice distance") { }

        public override bool IsSatisfied(params Function[] functions)
        {
            if (functions.Length != 1)
                throw new ArgumentException("Invalid parameters length");

            // TODO: Implement

            return true;
        }
    }
}
