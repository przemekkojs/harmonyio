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

    public List<QuizRequest> QuizRequests = [];
    public List<Quiz> NotSolvedOpen = [];
    public List<Quiz> NotSolvedPlanned = [];
    public List<Quiz> WaitingForGrade = [];
    public List<Quiz> Graded = [];
    public Dictionary<int, QuizResult> GradedQuizes = [];

    [BindProperty]
    public int RequestId { get; set; }

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

        var user = await _repository.GetAsync<ApplicationUser>(
            u => u.Id == appUser.Id,
            u => u
                .Include(u => u.QuizResults)
                .Include(u => u.ParticipatedQuizes)
                    .ThenInclude(q => q.Exercises)
                        .ThenInclude(e => e.ExerciseSolutions.Where(es => es.UserId == appUser.Id))
                .Include(u => u.ParticipatedQuizes)
                    .ThenInclude(q => q.Creator)
                .Include(u => u.QuizRequests)
                    .ThenInclude(qr => qr.Quiz)
                        .ThenInclude(q => q.Creator)
        );

        if (user == null)
        {
            return RedirectToPage("/Error");
        };

        QuizRequests = user.QuizRequests.ToList();

        GradedQuizes = user.QuizResults
            .Where(qr => qr.Grade != null)
            .ToDictionary(qr => qr.QuizId);

        foreach (var quiz in user.ParticipatedQuizes.Reverse())
        {
            bool isSolved = quiz.Exercises.Any(e => e.ExerciseSolutions.Any(es => es.UserId == appUser.Id));

            if (isSolved || quiz.State == QuizState.Closed)
            {
                if (GradedQuizes.ContainsKey(quiz.Id))
                {
                    Graded.Add(quiz);
                }
                else
                {
                    WaitingForGrade.Add(quiz);
                }
            }
            else
            {
                if (quiz.State == QuizState.Open)
                    NotSolvedOpen.Add(quiz);
                else if (quiz.State == QuizState.NotStarted)
                    NotSolvedPlanned.Add(quiz);
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostDeclineRequest()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser == null)
        {
            return Forbid();
        }

        var quizRequest = await _repository.GetAsync<QuizRequest>(qr => qr.Id == RequestId);
        if (quizRequest == null)
        {
            return Forbid();
        }

        _repository.Delete(quizRequest);
        await _repository.SaveChangesAsync();

        return RedirectToPage();
    }
}