using Algorithm.Music;

namespace Algorithm.Algorithm
{
    public class UserStack : Stack
    {
        public Note? Soprano { get => soprano; set => SetSoprano(value); }
        public Note? Alto { get => alto; set => SetAlto(value); }
        public Note? Tenore { get => tenore; set => SetTenore(value); }
        public Note? Bass { get => bass; set => SetBass(value); }

        public UserStack(Function baseFunction, Tonation tonation, int startBeat) : base(baseFunction, tonation, startBeat) { }
        
        private bool SetSoprano(Note? value) => SetNote(ref soprano, value);
        private bool SetAlto(Note? value) => SetNote(ref alto, value);
        private bool SetTenore(Note? value) => SetNote(ref tenore, value);
        private bool SetBass(Note? value) => SetNote(ref bass, value);

        private bool SetNote(ref Note? note, Note? newValue)
        {
            if (duration != 0 && note != null && note.RhytmicValue.Duration == duration)
            {
                note = newValue;
                SetDuration();
                return true;
            }

            return false;
        }

        private void SetDuration()
        {
            List<Note?> all = [Soprano, Alto, Tenore, Bass];

            foreach (var note in all)
            {
                if (note != null)
                {
                    duration = note.RhytmicValue.Duration;
                    return;
                }
            }

            duration = 0;
        }
    }
}
