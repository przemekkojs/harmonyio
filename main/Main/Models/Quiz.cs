using System;
using System.ComponentModel.DataAnnotations.Schema;
using Main.Enumerations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Main.Models
{
	public class Quiz
	{
		public int Id { get; set; }

		public string Name { get; set; } = "";
		public DateTime OpenDate { get; set; } = default;
		public DateTime CloseDate { get; set; } = default;

		public bool IsCreated { get; set; } = false;
		
		public string? Code {get; set;} = CodeGeneration();

		//FOREIGN KEYS

		public string CreatorId { get; set; } = null!;
        public ApplicationUser Creator { get; set; } = null!;

        public ICollection<Excersise> Excersises = new List<Excersise>();
        public ICollection<ApplicationUser> Participants = new List<ApplicationUser>();
		public ICollection<QuizResult> QuizResults = new List<QuizResult>();

		//NOT MAPPED 

		[NotMapped]
        public QuizState State => !IsCreated || OpenDate > DateTime.Now ? QuizState.NotStarted :
			CloseDate < DateTime.Now ? QuizState.Closed : QuizState.Open;

		// Todo: If needed make it secure / filter bad words
		private static string CodeGeneration()
		{
			string path = Path.GetRandomFileName();
			path = path.Replace(".", "");
			return path[..6];
		}
    }
}

