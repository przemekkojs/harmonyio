using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Main.Pages
{
    [Authorize]
    public class GroupsIndexModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPostRedirectToDetailsAdmin()
        {
            TempData["IsAdmin"] = true;
            return RedirectToPage("Details");
        }

        public IActionResult OnPostRedirectToDetailsUser()
        {
            TempData["IsAdmin"] = false;
            return RedirectToPage("Details");
        }
    }
}
