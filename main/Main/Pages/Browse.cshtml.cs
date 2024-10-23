using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages
{
    [Authorize]
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
                qr => qr.QuizId == id,
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
    }
}
