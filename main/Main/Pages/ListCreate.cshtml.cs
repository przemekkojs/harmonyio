using System.Collections;
using System.Security.Claims;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
    
    public async Task OnGetAsync()
    {
        // var AppUser = await _userManager.GetUserAsync(User);
        // if (AppUser == null)
        // {
        //     Forbid();
        // }
        //
        // Created = (await _repository.GetAsync<ApplicationUser>(q => q.Id == AppUser!.Id,
        //     q => q.Include(u => u.CreatedQuizes)))!.CreatedQuizes;
        Created = await _repository.GetAllAsync<Quiz>();
    }
}