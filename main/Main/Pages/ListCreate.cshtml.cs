using System.Collections;
using System.Security.Claims;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages;

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
    
    public async void OnGetAsync()
    {
        Created = (await _userManager.GetUserAsync(User)).CreatedQuizes;
    }
}