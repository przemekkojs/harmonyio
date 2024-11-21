using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages;

[Authorize]
public class JoinModel : PageModel
{ 
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationRepository _repository;

    public Quiz Quiz {get; set;} = null!;

    public JoinModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync(string code)
    {
        var appUser = await _userManager.GetUserAsync(User);
        if (appUser == null)
        {
            return Forbid();
        }

        var quiz = await _repository.GetAsync<Quiz>(
            q => q.Code == code,
            query => query
                .Include(q => q.Creator)
                .Include(q => q.Exercises)
                .Include(q => q.Participants)
        );

        if (quiz == null || quiz.State == Enumerations.QuizState.Closed)
        {
            return RedirectToPage("Error");
        }

        Quiz = quiz;

        if (!quiz.Participants.Any(p => p.Id == appUser.Id))
        {
            quiz.Participants.Add(appUser);
            _repository.Update(Quiz);
            await _repository.SaveChangesAsync();
        }

        return Page();
    }
}