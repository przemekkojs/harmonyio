namespace Algorithm.New.Algorithm.Mistake
{
    public abstract class Mistake
    {
        public string Description { get; protected set; } = "Niesprecyzowany błąd.";
        public abstract int Quantity { get; }

        public abstract void GenerateDescription();
    }
}
