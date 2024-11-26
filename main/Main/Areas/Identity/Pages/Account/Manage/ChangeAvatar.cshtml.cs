using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Main.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ChangeAvatarModel : PageModel {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        public ApplicationUser CurrentUser {get; set;} = null!;

        public ChangeAvatarModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return Forbid();
            }

            CurrentUser = appUser;

            return Page();
        }

        public async Task<IActionResult> OnPost(int colorIndex)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return Forbid();
            }

            appUser.AvatarColorId = colorIndex;
            _repository.Update(appUser);
            await _repository.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}