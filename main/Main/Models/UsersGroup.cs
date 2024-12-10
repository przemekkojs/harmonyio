namespace Main.Models
{
    public class UsersGroup
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        // FOREIGN KEYS
        public string? MasterId { get; set; }
        public ApplicationUser MasterUser { get; set; } = null!;

        public ICollection<ApplicationUser> Members { get; set; } = new List<ApplicationUser>();
        public ICollection<ApplicationUser> Admins { get; set; } = new List<ApplicationUser>();
        public ICollection<GroupRequest> Requests { get; set; } = new List<GroupRequest>();
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }
}

