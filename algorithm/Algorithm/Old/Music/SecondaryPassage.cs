namespace Algorithm.Old.Music
{
    public class SecondaryPassage
    {
        public Function Resolution { get => resolution; }
        public List<Function> Passage { get => passage; }
        public int Length { get => passage.Count; }

        private readonly Function resolution;
        private readonly List<Function> passage;

        public SecondaryPassage(List<Function> passage, Function resolution)
        {
            this.resolution = resolution;
            this.passage = passage;

            ValidatePassage();
        }

        public void ValidatePassage()
        {

        }

        public void AddFunctionToPassage(int index, Function function)
        {
            ValidatePassage();
        }

        public void RemoveFunctionFromPassage(int index)
        {
            ValidatePassage();
        }

        public void RemoveFunctionFromPassage(Function function)
        {
            ValidatePassage();
        }
    }
}
