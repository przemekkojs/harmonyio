using System;
using System.Text.Json.Serialization;
using Main.Enumerations;

namespace Main.Models
{
	public class QuizRequest
	{
		public int Id { get; set; }
    
		public string UserId { get; set; } = null!;

        [JsonIgnore]
		public ApplicationUser User { get; set; } = null!;

		public int QuizId { get; set; }

        [JsonIgnore]
		public Quiz Quiz { get; set; } = null!;
	}
}

