using System;
using Main.Enumerations;

namespace Main.Models
{
	public class QuizResult
	{
		public int Id { get; set; }

		public Grade? Grade { get; set; }
        public DateTime GradeDate { get; set; }
        public bool ShowAlgorithmOpinion { get; set; } = false;

        //FOREIGN KEYS
        public int QuizId { get; set; }
		public Quiz Quiz { get; set; } = null!;

		public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

        public ICollection<ExcersiseResult> ExcersiseResults = [];
	}
}

