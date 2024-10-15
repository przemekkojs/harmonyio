using Algorithm.Music;
using System;
namespace Algorithm.Algorithm.Rules
{
    public sealed class VoiceDistance : Rule
    {
        private const int SA_EXPECTED = 12;
        private const int AT_EXPECTED = 12;
        private const int TB_EXPECTED = 24;

        public VoiceDistance() : base("Voice distance", expectedParametersCount: 1) { }

        public override bool IsSatisfied(params Stack[] stacks)
        {
            if (stacks.Length != expectedParametersCount)
                throw new ArgumentException("Invalid parameters length");

            Stack param = stacks[0];

            int SA_real = Interval.SemitonesBetween(param.Soprano, param.Alto);
            int AT_real = Interval.SemitonesBetween(param.Alto, param.Tenore);
            int TB_real = Interval.SemitonesBetween(param.Tenore, param.Bass);

            return (
                SA_EXPECTED <= SA_real &&
                AT_EXPECTED <= AT_real &&
                TB_EXPECTED <= TB_real
            );
        }
    }
}
