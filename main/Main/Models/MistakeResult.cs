using Microsoft.EntityFrameworkCore;

namespace Main.Models
{
    public class MistakeResult
    {
        public int Id { get; set; }
        public int ExcersiseResultId { get; set; }

        public List<int> Bars { get; set; } = [];
        public List<int> Functions { get; set; } = [];
        public List<int> MistakeCodes { get; set; } = [];
    }
}
