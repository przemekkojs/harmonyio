using System.Collections;
using System.Security.Claims;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages;

[Authorize]
public class ListCreate : PageModel
{
    
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationRepository _repository;
    public ListCreate(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _repository = repository;
    }

    public ICollection<Quiz> Created { get; set; } = new List<Quiz>();
    
    public async Task OnGetAsync()
    {
        var appUser = await _userManager.GetUserAsync(User);
        
        Created = (await _repository.GetAsync<ApplicationUser>(q => q.Id == appUser!.Id,
            q => q.Include(u => u.ParticipatedQuizes)))!.ParticipatedQuizes;
    }
}