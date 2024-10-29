using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Main.Pages
{
    public class GroupDetailsModel : PageModel
    {
        public bool IsAdmin { get; set; }

        public void OnGet(int id, bool admin = false)
        {
            IsAdmin = admin;
        }
    }
}
