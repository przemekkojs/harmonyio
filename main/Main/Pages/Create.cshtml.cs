using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Main.Pages
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        [BindProperty]
        public Quiz Quiz { get; set; } = null!;
        [BindProperty]
        public List<string> Questions { get; set; } = null!;

        public CreateModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public void OnGet()
        {
              
        }

        public async Task<IActionResult> OnPost()
        {
            //TODO: REMOVE THIS
            var toRemove = await _repository.GetAllAsync<Quiz>();
            foreach (var quiz in toRemove)
                _repository.Delete(quiz);
            await _repository.SaveChangesAsync();

            //TODO: POPUALTE WITH REAL USER
            var currentUser = await GetTestUser();
            Quiz.CreatorId = currentUser.Id;

            _repository.Add(Quiz);
            await _repository.SaveChangesAsync();

            foreach (string question in Questions)
            {
                _repository.Add(new Excersise()
                {
                    Question = question,
                    QuizId = Quiz.Id,
                });
            }
            await _repository.SaveChangesAsync();

            //

            var allExcersises = _repository.GetAllAsync<Excersise>();
            var allQuizes = _repository.GetAllAsync<Quiz>();

            return RedirectToPage("Index");
        }

        public async Task<ApplicationUser> GetTestUser()
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
