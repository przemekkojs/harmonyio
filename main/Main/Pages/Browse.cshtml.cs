using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages
{
    public class BrowseModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        public QuizResult QuizResult { get; set; } = null!;

        public BrowseModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var quizResult = await _repository.GetAsync<QuizResult>(
                qr => qr.Id == id,
                query => query
                    .Include(qr => qr.ExcersiseResults)
                    .ThenInclude(er => er.ExcersiseSolution)
                    .Include(qr => qr.Quiz)
                    .ThenInclude(q => q.Excersises)
            );

            if (currentUser == null || quizResult == null || quizResult.UserId != currentUser.Id)
            {
                return Forbid();
            }

            QuizResult = quizResult;

            return Page();
        }


        private async Task<ApplicationUser> GetTestUser()
        {
            var userId = "testUser";
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return user;
            }
            else
            {
                user = new ApplicationUser
                {
                    Id = userId,
                    UserName = userId,
                    FirstName = "Test",
                    LastName = "User"
                };

                var result = await _userManager.CreateAsync(user, "Test123!");
                return (await _userManager.FindByIdAsync(userId))!;
            }
        }
    }
}
