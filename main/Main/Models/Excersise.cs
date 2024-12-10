using System.Text.Json.Serialization;

namespace Main.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public int MaxPoints { get; set; }

        //FOREIGN KEYS
        public int QuizId { get; set; }
        [JsonIgnore]
        public Quiz Quiz { get; set; } = null!;

        public ICollection<ExerciseSolution> ExerciseSolutions = [];
    }
}

