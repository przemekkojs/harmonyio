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

        public async void OnGet(int id)
        {
            //TODO: POPUALTE WITH REAL USER
            // var currentUser = await GetTestUser();

            //TODO: REMOVE THIS, THE ID SHOULD BE A PARAMETER OF ONGET
            // var allQuizResults = await _repository.GetAllAsync<QuizResult>();
            // var allUsersResults = allQuizResults.Where(qr => qr.UserId == currentUser.Id).ToList();
            // if (allUsersResults.Count == 0)
            //     throw new InvalidOperationException("No quiz results found in the repository. Go to /grade page and grade a quiz for testing purposes");
            // int id = allUsersResults[0].Id;

            QuizResult = (await _repository.GetAsync<QuizResult>(
                qr => qr.Id == id,
                query => query
                    .Include(qr => qr.ExcersiseResults)
                    .ThenInclude(er => er.ExcersiseSolution)
                    .Include(qr => qr.Quiz)
                    .ThenInclude(q => q.Excersises)
            ))!;
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
