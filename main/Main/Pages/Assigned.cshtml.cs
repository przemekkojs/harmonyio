using System.Security.Claims;
using Main.Data;
using Main.Enumerations;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages;

[Authorize]
public class AssignedModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationRepository _repository;

    public ApplicationUser AppUser { get; set; } = null!;
    public List<Quiz> SolvedOpen = [];
    public List<Quiz> NotSolvedOpen = [];
    public List<Quiz> NotSolvedPlanned = [];
    public List<Quiz> Closed = [];
    public Dictionary<int, QuizResult> GradedQuizes = [];

    public AssignedModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _repository = repository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var appUser = await _userManager.GetUserAsync(User);
        if (appUser == null)
        {
            return Forbid();
        }

        AppUser = appUser;

        var user = await _repository.GetAsync<ApplicationUser>(
            u => u.Id == appUser.Id,
            u => u
                .Include(u => u.QuizResults)
                .Include(u => u.ParticipatedQuizes)
                    .ThenInclude(q => q.Exercises)
                        .ThenInclude(e => e.ExerciseSolutions.Where(es => es.UserId == appUser.Id))
                .Include(u => u.ParticipatedQuizes)
                    .ThenInclude(q => q.Creator)
        );


        if (user == null)
        {
            return RedirectToPage("/Error");
        };

        GradedQuizes = user.QuizResults
            .Where(qr => qr.Grade != null)
            .ToDictionary(qr => qr.QuizId);

        foreach (var quiz in user.ParticipatedQuizes.Reverse())
        {
            bool isSolved = quiz.Exercises.Any(e => e.ExerciseSolutions.Any());

            if (isSolved)
            {
                if (quiz.State == QuizState.Open)
                    SolvedOpen.Add(quiz);
                else if (quiz.State == QuizState.Closed)
                    Closed.Add(quiz);
            }
            else
            {
                if (quiz.State == QuizState.Open)
                    NotSolvedOpen.Add(quiz);
                else if (quiz.State == QuizState.NotStarted)
                    NotSolvedPlanned.Add(quiz);
                else if (quiz.State == QuizState.Closed)
                    Closed.Add(quiz);
            }
        }
        return Page();
    }
}