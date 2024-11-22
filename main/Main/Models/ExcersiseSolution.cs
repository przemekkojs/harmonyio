using System;
using System.Text.Json.Serialization;

namespace Main.Models
{
	public class ExerciseSolution
	{
		public int Id { get; set; }

		//Tymczasowo odpowiedź to po prostu jakis testowy string
		public string Answer { get; set; } = "";

		//FOREIGN KEYS

		public int ExerciseId { get; set; }
		[JsonIgnore]
		public Exercise Exercise { get; set; } = null!;

		public string UserId { get; set; } = null!;
		[JsonIgnore]
		public ApplicationUser User { get; set; } = null!;

		[JsonIgnore]    //looping dependency, json converter goes crazy without it
		public ExerciseResult? ExerciseResult { get; set; }
	}
}

