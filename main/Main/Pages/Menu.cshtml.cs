using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Main.Pages;

[Authorize]
public class MenuModel : PageModel
{
    public void OnGet()
    {
        
    }
}