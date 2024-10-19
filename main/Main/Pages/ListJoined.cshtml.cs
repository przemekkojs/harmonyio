using System.Security.Claims;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages;

public class ListJoined : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationRepository _repository;
    
    public ListJoined(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _repository = repository;
        
    }
    
    public ICollection<Quiz> Joined { get; set; } = new List<Quiz>();
    
    public ApplicationUser AppUser { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        AppUser = await _userManager.GetUserAsync(User);
        
        Joined = (await _repository.GetAsync<ApplicationUser>(q => q.Id == AppUser!.Id,
            q => q.Include(u => u.ParticipatedQuizes)))!.ParticipatedQuizes;

        return Page();
    }
}