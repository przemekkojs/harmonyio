using System.Collections;
using System.Security.Claims;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages;

[Authorize] 
public class ListCreate : PageModel
{   
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationRepository _repository;

    [BindProperty]
    public int PublishedQuizId { get; set; }

    public ListCreate(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _repository = repository;
    }

    public ICollection<Quiz> Created { get; set; } = new List<Quiz>();

    public async Task Init()
    {
        var appUser = await _userManager.GetUserAsync(User);
        Created = (await _repository.GetAsync<ApplicationUser>(q => q.Id == appUser!.Id,
            q => q.Include(u => u.CreatedQuizes)))!.CreatedQuizes;
    }
    
    public async Task OnGetAsync()
    {
        await Init();
    }

    public async Task<IActionResult> OnPost()
    {
        var publishedQuiz = await _repository.GetAsync<Quiz>(q => q.Id == PublishedQuizId);
        publishedQuiz!.IsCreated = true;

        _repository.Update(publishedQuiz);
        await _repository.SaveChangesAsync();

        await Init();
        return Page();
    }
}