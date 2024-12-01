namespace Algorithm.New.Music
{
    public record Index(int Bar = 0, int Position = 0, int Duration = 0)
    {
        public int Bar { get; set; } = Bar;
        public int Position { get; set; } = Position;
        public int Duration { get; set; } = Duration;
    }
}