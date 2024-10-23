using System.Collections;
using System.Security.Claims;
using Main.Data;
using Main.Enumerations;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;

namespace Main.Pages;

[Authorize] 
public class CreatedModel : PageModel
{   
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationRepository _repository;

    [BindProperty]
    public string PostAction { get; set; } = null!;
    [BindProperty]
    public int QuizId { get; set; }

    public CreatedModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _repository = repository;
    }

    public ICollection<Quiz> UsersQuizes { get; set; } = null!;
    public Dictionary<Quiz, (int, int)> QuizesToUsersCompleted { get; set; } = null!;

    public async Task<bool> Init()
    {
        var appUser = await _userManager.GetUserAsync(User);
        if (appUser == null) return false;

        var user = await _repository.GetAsync<ApplicationUser>(
            filter: u => u.Id == appUser.Id,
            modifier: u => u
                .Include(u => u.CreatedQuizes)
                .   ThenInclude(q => q.QuizResults)
                .Include(u => u.CreatedQuizes)
                .   ThenInclude(q => q.Participants)
                .Include(u => u.CreatedQuizes)
                    .ThenInclude(q => q.Excersises)
                        .ThenInclude(e => e.ExcersiseSolutions));

        if (user == null) return false;

        UsersQuizes = user.CreatedQuizes;
        QuizesToUsersCompleted = UsersQuizes.ToDictionary(
            q => q,
            q => (q.Excersises.First().ExcersiseSolutions.Count, q.Participants.Count)
        );

        return true;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        var success = await Init();
        if (!success)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (PostAction == "publish")
        {
            var publishedQuiz = await _repository.GetAsync<Quiz>(q => q.Id == QuizId);
            publishedQuiz!.IsCreated = true;

            _repository.Update(publishedQuiz);
            await _repository.SaveChangesAsync();

            var success = await Init();
            if (!success)
            {
                return Forbid();
            }
        }
        else if (PostAction == "delete")
        {
            var deletedQuiz = await _repository.GetAsync<Quiz>(q => q.Id == QuizId);
            if (deletedQuiz == null)
            {
                return RedirectToPage("Error");
            }

            _repository.Delete(deletedQuiz);
            await _repository.SaveChangesAsync();

            var success = await Init();
            if (!success)
            {
                return Forbid();
            }
        }

        return Page();
    }

    public bool IsReadyToGrade(Quiz quiz) =>
        quiz.QuizResults.Count == 0 && (quiz.State == QuizState.Closed ||
            (QuizesToUsersCompleted[quiz].Item2 > 0 &&
                QuizesToUsersCompleted[quiz].Item1 == QuizesToUsersCompleted[quiz].Item2));
}