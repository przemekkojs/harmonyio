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
    
    public int AvatarColorId { get; set; } = 0;

    //FOREIGN KEYS

    public ICollection<Quiz> CreatedQuizes = new List<Quiz>();
    public ICollection<Quiz> ParticipatedQuizes = new List<Quiz>();

    public ICollection<ExerciseSolution> ExerciseSolutions = new List<ExerciseSolution>();
    public ICollection<QuizResult> QuizResults = new List<QuizResult>();


    public ICollection<UsersGroup> StudentInGroups { get; set; } = new List<UsersGroup>();

    public ICollection<UsersGroup> TeacherInGroups { get; set; } = new List<UsersGroup>();

    public ICollection<UsersGroup> MasterInGroups { get; set; } = new List<UsersGroup>();

    public ICollection<GroupRequest> Requests { get; set; } = new List<GroupRequest>();

    //NOT MAPPED

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [NotMapped]
    public string AvatarColor => PossibleAvatarColors[AvatarColorId];

    //STATICS

    [NotMapped]
    public static readonly string[] PossibleAvatarColors = [ 
        "#517dd5", 
        "#235696", 
        "#4396a5",
        "#3c877b",
        "#1e4c41",
        "#74a048",
        "#42692a",
        "#59423a",
        "#7759bb",
        "#4e2fa1",
        "#df742c",
        "#e25e34",
        "#b0431f",
        "#9e4db7",
        "#71269c",
        "#7c8f9d",
        "#495962",
        "#d84f7a",
        "#b12c5e",
    ];
}