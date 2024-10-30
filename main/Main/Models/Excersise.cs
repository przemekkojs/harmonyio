using System;
using System.Text.Json.Serialization;

namespace Main.Models
{
	public class Excersise
	{
		public int Id { get; set; }

		//Tymczasowo pytanie to po prostu jakiś testowy string
		public string Question { get; set; } = "";

		//FOREIGN KEYS

		public int QuizId { get; set; }
		[JsonIgnore]
		public Quiz Quiz { get; set; } = null!;

		public ICollection<ExcersiseSolution> ExcersiseSolutions = new List<ExcersiseSolution>();
	}
}

