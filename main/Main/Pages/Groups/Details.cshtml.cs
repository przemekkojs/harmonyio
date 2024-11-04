using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Main.Pages
{
    [Authorize]
    public class GroupDetailsModel : PageModel
    {
        public bool IsAdmin { get; set; }

        public void OnGet(int id)
        {
            IsAdmin = (bool)(TempData["IsAdmin"] ?? false);
        }

        public IActionResult OnPostRedirectToIndexOwned()
        {
            TempData["OwnedShown"] = true;
            return RedirectToPage("Index");
        }

        public IActionResult OnPostRedirectToIndexJoined()
        {
            TempData["OwnedShown"] = false;
            return RedirectToPage("Index");
        }
    }
}
