using Algorithm.Music;

namespace Algorithm.Algorithm
{
    public sealed class UserStack : Stack
    {
        public UserStack(Function baseFunction, Tonation tonation, int startBeat) : base(baseFunction, tonation, startBeat) { }

        public override bool SetSoprano(Note? value, RhytmicValue rhytmicValue) => SetNote(ref soprano, value, rhytmicValue);
        public override bool SetAlto(Note? value, RhytmicValue rhytmicValue) => SetNote(ref alto, value, rhytmicValue);
        public override bool SetTenore(Note? value, RhytmicValue rhytmicValue) => SetNote(ref tenore, value, rhytmicValue);
        public override bool SetBass(Note? value, RhytmicValue rhytmicValue) => SetNote(ref bass, value, rhytmicValue);

        private bool SetNote(ref Note? note, Note? newValue, RhytmicValue rhytmicValue)
        {
            if (Duration == 0)
            {
                Duration = rhytmicValue.Duration;
            }

            if (Duration != 0 && note != null && rhytmicValue.Duration == Duration)
            {
                note = newValue;
                SetDuration(rhytmicValue);
                ValidateDuration();
                return true;
            }

            return false;
        }

        private void SetDuration(RhytmicValue rhytmicValue)
        {
            List<Note?> all = [Soprano, Alto, Tenore, Bass];

            foreach (var note in all)
            {
                if (note != null)
                {
                    Duration = rhytmicValue.RealDuration;
                    return;
                }
            }

            Duration = 0;
        }
    }
}
