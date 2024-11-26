using System.ComponentModel.DataAnnotations.Schema;
using Main.Enumerations;

namespace Main.Models
{
	public class Quiz
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;
		public DateTime OpenDate { get; set; } = default;
		public DateTime CloseDate { get; set; } = default;
		public bool IsCreated { get; set; } = false;
		public bool IsValid { get; set; } = true;
		public bool ShowAlgorithmOpinion { get; set; } = false;

		public string? Code { get; set; }

		//FOREIGN KEYS
		public string CreatorId { get; set; } = null!;
		public ApplicationUser Creator { get; set; } = null!;

		public ICollection<Exercise> Exercises = new List<Exercise>();
		public ICollection<ApplicationUser> Participants = new List<ApplicationUser>();
		public ICollection<QuizResult> QuizResults = new List<QuizResult>();
		public ICollection<UsersGroup> PublishedToGroup = new List<UsersGroup>();
		public ICollection<QuizRequest> Requests { get; set; } = new List<QuizRequest>();

		//NOT MAPPED 
		[NotMapped]
		public QuizState State => !IsCreated || OpenDate > DateTime.Now ? QuizState.NotStarted :
			CloseDate < DateTime.Now ? QuizState.Closed : QuizState.Open;
	}
}

