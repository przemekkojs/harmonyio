using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages
{
    public class SolveModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        public Quiz Quiz { get; set; } = null!;

        public SolveModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async void OnGet() //int id
        {
            //TODO: REMOVE THIS, THE ID SHOULD BE A PARAMETER OF ONGET
            var allQuizes = await _repository.GetAllAsync<Quiz>();
            if (allQuizes.Count == 0)
                throw new InvalidOperationException("No quizes found in the repository. Go to /create page and create a quiz for testing purposes.");
            int id = allQuizes[2].Id;

            Quiz = (await _repository.GetAsync<Quiz>(
                q => q.Id == id,
                query => query.Include(q => q.Excersises)
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
