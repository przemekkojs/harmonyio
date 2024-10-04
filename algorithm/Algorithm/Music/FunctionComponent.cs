namespace Algorithm.Music
{
    public class FunctionComponent
    {
        public string Symbol { get => symbol; }
        public bool Required {  get => required; }

        private readonly string symbol;
        private readonly bool required;

        public FunctionComponent(string symbol, bool required=false)
        {
            this.symbol = symbol;
            this.required = required;
        }
    }
}
