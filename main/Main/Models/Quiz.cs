using System;
namespace Main.Models
{
	public class Quiz
	{
		public int Id { get; set; }

		public string Name { get; set; } = "";
		public DateTime OpenDate { get; set; } = default;
		public DateTime CloseDate { get; set; } = default;

		//FOREIGN KEYS

		public string CreatorId { get; set; } = null!;
        public ApplicationUser Creator { get; set; } = null!;

        public ICollection<Excersise> Excersises = new List<Excersise>();
        public ICollection<ApplicationUser> Participants = new List<ApplicationUser>();
		public ICollection<QuizResult> QuizResults = new List<QuizResult>();
    }
}

