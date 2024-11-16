namespace Main.Models
{
    public class MistakeResult
    {
        public int Id { get; set; }

        public List<int> Bars { get; set; } = [];
        public List<int> Functions { get; set; } = [];
        public List<int> MistakeCodes { get; set; } = [];

        // FOREIGN KEYS
        public int? ExcersiseResultId { get; set; }
        public ExcersiseResult? ExcersiseResult { get; set; }
    }
}
