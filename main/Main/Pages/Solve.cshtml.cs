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

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var appUser = (await _userManager.GetUserAsync(User))!;
            var quiz = (await _repository.GetAsync<Quiz>(
                q => q.Code == id,
                query => query
                    .Include(q => q.Participants)
                    .Include(q => q.QuizResults)
                    .Include(q => q.Excersises)
                    .ThenInclude(q => q.ExcersiseSolutions)
            ))!;

            // if (quiz == null || appUser == null ||
            //     quiz.State != Enumerations.QuizState.Open ||
            //     !quiz.Participants.Any(u => u.Id == appUser.Id) ||
            //     quiz.QuizResults.Any(qr => qr.UserId == appUser.Id))
            // {
            //     return Forbid();
            // }

            Quiz = quiz;
            Answers = Quiz.Excersises
                .Select(e => e.ExcersiseSolutions.FirstOrDefault(es => es.UserId == appUser.Id))
                .Select(es => es?.Answer ?? "").ToList();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var quiz = await _repository.GetAsync<Quiz>(
                q => q.Id == QuizId,
                query => query.Include(q => q.Excersises)
            );

            if (currentUser == null || quiz == null)
                return RedirectToPage("Error");

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
