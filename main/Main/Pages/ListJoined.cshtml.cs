using System.Security.Claims;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

    public async Task OnGetAsync()
    {
        AppUser = await _userManager.GetUserAsync(User);
        Joined = AppUser.CreatedQuizes;
    }
}