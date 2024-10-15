using Algorithm.Algorithm;
using Algorithm.Music;

namespace Algorithm.Communication
{
    public class TaskBuilder
    {
        public List<UserBar> Task { get => task; }
        public Tonation? Tonation { get; private set; }
        public Meter? Meter { get; private set; }
        public int MaxLength { get; set; }
        public int Length { get => Task.Count; }

        private readonly List<UserBar> task;

        public TaskBuilder(int length)
        {
            MaxLength = length;
            task = [];
        }

        public bool AddBar(Bar baseBar)
        {
            if (task.Count < MaxLength)
            {
                Task.Add(new UserBar(baseBar));
                return true;
            }

            return false;                
        }

        public bool AddFunction(string jsonFunction)
        {
            return true;
        }
    }
}