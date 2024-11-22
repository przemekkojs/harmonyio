using System;
using System.Text.Json.Serialization;

namespace Main.Models
{
	public class ExerciseResult
	{
		public int Id { get; set; }
		public int Points { get; set; }
		public int MaxPoints { get; set; }
		public string Comment { get; set; } = "";
		public int AlgorithmPoints { get; set; }

		//FOREIGN KEYS

		public int ExerciseSolutionId { get; set; }
		[JsonIgnore]    //looping dependency, json converter goes crazy without it
		public ExerciseSolution ExerciseSolution { get; set; } = null!;

		public int? QuizResultId { get; set; }
		// TODO quiz result should be nullable, when grading with algorithm, create new exercise result
		// this result may not be linked to quiz result
		[JsonIgnore]
		public QuizResult? QuizResult { get; set; } = null;

		public ICollection<MistakeResult> MistakeResults { get; set; } = new List<MistakeResult>();
	}
}