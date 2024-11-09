using System;
using Main.Enumerations;

namespace Main.Models
{
	public class GroupRequest
	{
		public int Id { get; set; }

		public string UserId { get; set; } = null!;

		public ApplicationUser User { get; set; } = null!;

		public int GroupId { get; set; }

		public UsersGroup Group { get; set; } = null!;

		public bool ForTeacher { get; set; } = false;
	}
}

