using System;
using Main.Enumerations;

namespace Main.Models
{
	public class GroupRequest
	{
		public int Id { get; set; }
		public bool ForAdmin { get; set; } = false;

		// FOREIGN KEYS
		public string UserId { get; set; } = null!;
		public ApplicationUser User { get; set; } = null!;

		public int GroupId { get; set; }
		public UsersGroup Group { get; set; } = null!;
	}
}

