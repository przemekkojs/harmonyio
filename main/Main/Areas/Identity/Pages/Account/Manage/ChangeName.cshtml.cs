using System.ComponentModel.DataAnnotations;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Main.Areas.Identity.Pages.Account.Manage
{
    public class ChangeNameModel : PageModel {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        [Required(ErrorMessage = "Imię nie moze być puste")]
        [BindProperty]
        public string Name {get; set;} = null!;

        [Required(ErrorMessage = "Nazwisko nie może być puste")]
        [BindProperty]
        public string Surname {get; set;} = null!;

        public ApplicationUser CurrentUser {get; set;} = null!;

        public ChangeNameModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
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

        public async Task<IActionResult> OnPostAsync()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return Forbid();
            }

            appUser.FirstName = Name;
            appUser.LastName = Surname;
            _repository.Update(appUser);
            await _repository.SaveChangesAsync();

            return RedirectToPage();
        }

    }
}