using System.ComponentModel.DataAnnotations;
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
}