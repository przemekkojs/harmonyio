using Algorithm.Algorithm;
using Algorithm.Music;

namespace Algorithm.Communication
{
    public sealed class TaskSolver
    {        
        private readonly Music.Tonation tonation;
        private readonly List<UserBar> bars;

        public TaskSolver(Tonation tonation)
        {
            this.tonation = tonation;
            bars = [];
        }

        public void CreateUserStack(int bar, int beat)
        {
            if (GetUserStack(bar, beat) == null)
            {
                var currentBar = bars[bar - 1];
                var barFunctionIndex = currentBar.LastEmptyStack;
                var function = currentBar.BaseBar.Functions[barFunctionIndex];

                currentBar.AddStack(new UserStack(function, tonation, beat));
            }
            else
                throw new ArgumentException("Cannot create new stack here.");
        }

        public UserStack? GetUserStack(int bar, int beat)
        {
            if (bar > bars.Count)
                throw new ArgumentException("Invalid bar.");

            var chosenBar = bars[bar - 1];

            if (beat >= chosenBar.BaseBar.Length)
                throw new ArgumentException("Invalid beat.");

            var matching = chosenBar.UserStacks
                .Where(x => x.Beat == beat)
                .ToList();

            if (matching.Count == 0)
                return null;
            else
                return matching[0];
        }

        public static void AddNote(UserStack userStack, Note note, RhytmicValue value)
        {
            switch (note.Voice)
            {
                case Voice.SOPRANO:
                    userStack.SetSoprano(note, value);
                    break;
                case Voice.ALTO:
                    userStack.SetAlto(note, value);
                    break;
                case Voice.TENORE:
                    userStack.SetTenore(note, value);
                    break;
                case Voice.BASS:
                    userStack.SetBass(note, value);
                    break;
                default:
                    throw new ArgumentException("Something went wrong...");
            }
        }

        public static void EditNote(UserStack userStack, Note newNote, RhytmicValue newRhytmicValue)
        {
            AddNote(userStack, newNote, newRhytmicValue);
        }

        public static void DeleteNote(UserStack userStack, Voice voice)
        {
            switch (voice)
            {
                case Voice.SOPRANO:
                    userStack.SetSoprano(null, null);
                    break;
                case Voice.ALTO:
                    userStack.SetAlto(null, null);
                    break;
                case Voice.TENORE:
                    userStack.SetTenore(null, null);
                    break;
                case Voice.BASS:
                    userStack.SetBass(null, null);
                    break;
                default:
                    throw new ArgumentException("Something went wrong...");
            }
        }
    }
}