using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;

namespace Main.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(32), Display(Name = "First Name")]
    public required string FirstName { get; set; }
    
    [MaxLength(32), Display(Name = "Last Name")]
    public required string LastName { get; set; }

    //FOREIGN KEYS

    public ICollection<Quiz> CreatedQuizes = new List<Quiz>();
    public ICollection<Quiz> ParticipatedQuizes = new List<Quiz>();

    public ICollection<ExcersiseSolution> ExcersiseSolutions = new List<ExcersiseSolution>();
    public ICollection<QuizResult> QuizResults = new List<QuizResult>();


    public ICollection<UsersGroup> StudentInGroups { get; set; } = new List<UsersGroup>();

    public ICollection<UsersGroup> TeacherInGroups { get; set; } = new List<UsersGroup>();

    public ICollection<UsersGroup> MasterInGroups { get; set; } = new List<UsersGroup>();

    public ICollection<GroupRequest> Requests { get; set; } = new List<GroupRequest>();

    //NOT MAPPED

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";
}