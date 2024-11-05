using Algorithm.Old.Music;
using System.Collections.Generic;

namespace Algorithm.Old.Algorithm
{
    public class Algorithm
    {
        private readonly Music.Task task;
        private const int NOTES_COUNT = 4;

        public Algorithm(Music.Task task)
        {
            this.task = task;
        }

        public List<List<ComputerStack>> GenerateAllResults()
        {
            List<List<ComputerStack>> results = [];
            List<Function> functions = [];

            ComputerStack current;

            foreach (var bar in task.Bars)
                functions.AddRange(bar.Functions);

            for (int index = 0; index < functions.Count; index++)
            {
                current = new ComputerStack(functions[index], task.Tonation, index * RhytmicValue.QUARTER_NOTE.RealDuration);
            }

            return results;
        }
    }
}
