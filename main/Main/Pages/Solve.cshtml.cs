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

        [BindProperty]
        public int QuizId { get; set; }
        [BindProperty]
        public List<string> Answers { get; set; } = null!;

        public SolveModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async void OnGet(int id)
        {
            Quiz = (await _repository.GetAsync<Quiz>(
                q => q.Id == id,
                query => query
                    .Include(q => q.Excersises)
                    .ThenInclude(q => q.ExcersiseSolutions)
            ))!;
            
            var appUser = (await _userManager.GetUserAsync(User))!;

            Answers = Quiz.Excersises
                .Select(e => e.ExcersiseSolutions).First(e => e.First().UserId == appUser.Id)
                .Select(e => e.Answer).ToList();
        }

        public async Task<IActionResult> OnPost()
        {
            //TODO: POPUALTE WITH REAL USER
            var currentUser = await GetTestUser();

            var quiz = (await _repository.GetAsync<Quiz>(
                q => q.Id == QuizId,
                query => query.Include(q => q.Excersises)
            ))!;

            var excersises = (List<Excersise>)quiz.Excersises;
            for (int i = 0; i < excersises.Count; i++)
            {
                var solution = new ExcersiseSolution()
                {
                    ExcersiseId = excersises[i].Id,
                    Answer = Answers[i] ?? "",
                    UserId = currentUser.Id,
                };
                _repository.Add(solution);
            }
            await _repository.SaveChangesAsync();

            return RedirectToPage("Index");
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
