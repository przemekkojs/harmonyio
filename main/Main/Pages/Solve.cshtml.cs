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
    public class SolveModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        public Quiz Quiz { get; set; } = null!;

        [BindProperty]
        public int QuizId { get; set; }
        [BindProperty]
        public List<string> Answers { get; set; } = new();

        public SolveModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string code)
        {
            var appUser = await _userManager.GetUserAsync(User);
            var quiz = await _repository.GetAsync<Quiz>(
                q => q.Code == code,
                query => query
                    .Include(q => q.Participants)
                    .Include(q => q.QuizResults)
                    .Include(q => q.Excersises)
                    .ThenInclude(q => q.ExcersiseSolutions)
            );

            if (quiz == null || appUser == null ||
                quiz.State != Enumerations.QuizState.Open)
            {
                return Forbid();
            }

            Quiz = quiz;

            if (!Quiz.Participants.Any(u => u.Id == appUser.Id))
            {
                Quiz.Participants.Add(appUser);
                _repository.Update(Quiz);
                await _repository.SaveChangesAsync();
            }

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
                query => query
                    .Include(q => q.Excersises)
                    .ThenInclude(e => e.ExcersiseSolutions)
            );

            if (currentUser == null || quiz == null)
                return RedirectToPage("Error");

            var excersises = (List<Excersise>)quiz.Excersises;
            var oldAnswers = quiz.Excersises.SelectMany(e => e.ExcersiseSolutions.Where(es => es.UserId == currentUser.Id));
            foreach (var solution in oldAnswers)
            {
                if (solution != null)
                {
                    _repository.Delete(solution);
                }
            }
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
    }
}
