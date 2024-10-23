using System.Security.Claims;
using Main.Data;
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
    
    public AssignedModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _repository = repository;
    }

    public ApplicationUser AppUser { get; set; } = null!;
    public ICollection<Quiz> Joined { get; set; } = new List<Quiz>();

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Forbid();
        }

        AppUser = user;
        Joined = (await _repository.GetAsync<ApplicationUser>(
            q => q.Id == AppUser!.Id,
            q => q
                .Include(u => u.ParticipatedQuizes)
                    .ThenInclude(q => q.QuizResults)
                .Include(u => u.ParticipatedQuizes)
                    .ThenInclude(q => q.Excersises)
                        .ThenInclude(e => e.ExcersiseSolutions)
        ))!.ParticipatedQuizes;

        return Page();
    }
}