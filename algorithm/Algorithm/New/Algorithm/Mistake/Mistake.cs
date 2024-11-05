namespace Algorithm.New.Algorithm.Mistake
{
    public abstract class Mistake
    {
        public string Description { get; protected set; }

        public abstract void GenerateDescription();
    }
}
