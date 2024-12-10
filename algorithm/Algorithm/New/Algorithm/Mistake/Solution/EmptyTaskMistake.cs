namespace Algorithm.New.Algorithm.Mistake.Solution
{
    public class EmptyTaskMistake : Mistake
    {
        public override int Quantity => 1;

        public EmptyTaskMistake(int code = 999)
        {
            MistakeCode = code;
            GenerateDescription();
        }

        public override void GenerateDescription() =>
            Description = ([], [], MistakeCode);
    }
}
