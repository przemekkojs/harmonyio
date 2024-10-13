using System;
namespace Main.Models
{
	public class ExcersiseSolution
	{
		public int Id { get; set; }

		//Tymczasowo odpowiedź to po prostu jakis testowy string
		public string Answer { get; set; } = "";

		//FOREIGN KEYS

		public int ExcersiseId { get; set; }
		public Excersise Excersise { get; set; } = null!;

		public string UserId { get; set; } = null!;
		public ApplicationUser User { get; set; } = null!;

		public int ExcersiseResultId { get; set; }
		public ExcersiseResult? ExcersiseResult { get; set; }
    }
}

