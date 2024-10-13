using System;
namespace Main.Models
{
	public class ExcersiseResult
	{
		public int Id { get; set; }

		public int Points { get; set; }
		public int MaxPoints { get; set; }
		public string Comment { get; set; } = "";

		//FOREIGN KEYS

		public int ExcersiseSolutionId { get; set; }
        public ExcersiseSolution ExcersiseSolution { get; set; } = null!;

        public int QuizResultId { get; set; }
        public QuizResult QuizResult { get; set; } = null!;
    }
}

