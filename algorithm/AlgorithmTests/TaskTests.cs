using Algorithm.Algorithm;
using Algorithm.Algorithm.Mistakes;
using Algorithm.Algorithm.Rules;
using Algorithm.Constants;
using Algorithm.Music;

namespace AlgorithmTests
{
    public class TaskTests
    {
        [Fact]
        public void TSDTCMajorGood()
        {
            Meter taskMeter = Meter.Meter3_4;
            Tonation tonation = new("C", Mode.MAJOR, 0, 0);

            Symbol TSymbol = new(false, FunctionSymbol.T);
            Symbol SSymbol = new(false, FunctionSymbol.S);
            Symbol DSymbol = new(false, FunctionSymbol.D);

            Function TFunction = new(TSymbol, true);
            Function SFunction = new(SSymbol, true);
            Function DFunction = new(DSymbol, true);
            Function TFunction2 = new(TSymbol, true);

            UserStack userT = new(TFunction, tonation, 0);
            UserStack userS = new(SFunction, tonation, 4);
            UserStack userD = new(DFunction, tonation, 8);
            UserStack userT2 = new(TFunction2, tonation, 0);

            userT.SetSoprano(new Note("C", 5, FunctionComponent.Root, Voice.SOPRANO), RhytmicValue.QUARTER_NOTE);
            userT.SetAlto(new Note("E", 4, FunctionComponent.Third, Voice.ALTO), RhytmicValue.QUARTER_NOTE);
            userT.SetTenore(new Note("G", 3, FunctionComponent.Fifth, Voice.TENORE), RhytmicValue.QUARTER_NOTE);
            userT.SetBass(new Note("C", 3, FunctionComponent.Root, Voice.BASS), RhytmicValue.QUARTER_NOTE);

            userS.SetSoprano(new Note("C", 5, FunctionComponent.Fifth, Voice.SOPRANO), RhytmicValue.QUARTER_NOTE);
            userS.SetAlto(new Note("F", 4, FunctionComponent.Root, Voice.ALTO), RhytmicValue.QUARTER_NOTE);
            userS.SetTenore(new Note("A", 3, FunctionComponent.Third, Voice.TENORE), RhytmicValue.QUARTER_NOTE);
            userS.SetBass(new Note("C", 3, FunctionComponent.Fifth, Voice.BASS), RhytmicValue.QUARTER_NOTE);

            userD.SetSoprano(new Note("D", 5, FunctionComponent.Fifth, Voice.SOPRANO), RhytmicValue.QUARTER_NOTE);
            userD.SetAlto(new Note("G", 4, FunctionComponent.Root, Voice.ALTO), RhytmicValue.QUARTER_NOTE);
            userD.SetTenore(new Note("G", 3, FunctionComponent.Root, Voice.TENORE), RhytmicValue.QUARTER_NOTE);
            userD.SetBass(new Note("B", 2, FunctionComponent.Root, Voice.BASS), RhytmicValue.QUARTER_NOTE);

            userT2.SetSoprano(new Note("C", 5, FunctionComponent.Root, Voice.SOPRANO), RhytmicValue.HALF_NOTE_DOTTED);
            userT2.SetAlto(new Note("E", 4, FunctionComponent.Third, Voice.ALTO), RhytmicValue.HALF_NOTE_DOTTED);
            userT2.SetTenore(new Note("G", 3, FunctionComponent.Fifth, Voice.TENORE), RhytmicValue.HALF_NOTE_DOTTED);
            userT2.SetBass(new Note("C", 3, FunctionComponent.Root, Voice.BASS), RhytmicValue.HALF_NOTE_DOTTED);

            List<Stack> userSolution = [userT, userS, userD, userT2];

            Bar bar1 = new(tonation, taskMeter);
            Bar bar2 = new(tonation, taskMeter);

            bar1.AddFunctionsRange([TFunction, SFunction, DFunction]);
            bar2.AddFunctionsRange([TFunction2]);

            Algorithm.Music.Task task = new (2);
            task.AddBarsRange([bar1, bar2]);

            List<Rule> activeRules = [Constants.VoiceDistance];

            TaskCheck check = new(task, userSolution, activeRules);
            List<Mistake> mistakes = check.Check();

            Assert.True(mistakes.Count == 0, "This should be true");
        }
    }
}
