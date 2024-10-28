using System;
using Main.Enumerations;

namespace Main.Models
{
	public class UsersGroup
	{
		public int Id { get; set; }

		public string Name {get; set; } = "";

		public ICollection<ApplicationUser> Students { get; set; } = new List<ApplicationUser>();

        public ICollection<ApplicationUser> Teachers { get; set; } = new List<ApplicationUser>();

        public ICollection<GroupRequest> Requests { get; set; } = new List<GroupRequest>();
	}
}
