using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Main.Pages;

[Authorize]
public class IndexModel : PageModel
{ 
    public void OnGet()
    {
        
    }
}