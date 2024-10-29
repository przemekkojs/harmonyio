using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Main.Pages
{
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
