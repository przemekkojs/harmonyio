using Algorithm.Music;

namespace Algorithm.Algorithm
{
    public sealed class ComputerStack : Stack
    {
        public ComputerStack(Function baseFunction, Tonation tonation, int startBeat) : base(baseFunction, tonation, startBeat) { }

        public override bool SetAlto(Note? value, RhytmicValue rhytmicValue)
        {
            throw new NotImplementedException();
        }

        public override bool SetBass(Note? value, RhytmicValue rhytmicValue)
        {
            throw new NotImplementedException();
        }

        public override bool SetSoprano(Note? value, RhytmicValue rhytmicValue)
        {
            throw new NotImplementedException();
        }

        public override bool SetTenore(Note? value, RhytmicValue rhytmicValue)
        {
            throw new NotImplementedException();
        }
    }
}
