using Main.Data;
using Main.Enumerations;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages
{
    // easy access to result data
    public class ExerciseResultData
    {
        public int Points { get; set; }
        public int MaxPoints { get; set; } = 10; // Default to 10 if no data
        public string Comment { get; set; } = string.Empty;
    }

    [Authorize]
    public class BrowseModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        public string GradeString { get; set; } = "";
        public Quiz Quiz { get; set; } = null!;
        public List<string> Questions = [];
        public List<string> Answers = [];
        public List<ExerciseResultData> ExerciseResults = new List<ExerciseResultData>();

        public class ExcersiseResultData
        {
            public int Points { get; set; }
            public int MaxPoints { get; set; } = 10; // Default to 10 if no data
            public string Comment { get; set; } = string.Empty;
        }

        public BrowseModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return Forbid();
            }
            var quiz = await _repository.GetAsync<Quiz>(
                q => q.Id == id,
                query => query
                    .Include(q => q.Excersises)
                        .ThenInclude(e => e.ExcersiseSolutions.Where(es => es.UserId == appUser.Id))
                            .ThenInclude(es => es.ExcersiseResult)
                    .Include(q => q.QuizResults.Where(qr => qr.UserId == appUser.Id))
                    .Include(q => q.Participants.Where(p => p.Id == appUser.Id))
            );

            if (quiz == null)
            {
                return RedirectToPage("Error");
            }

            if (!quiz.Participants.Any())
            {
                return Forbid();
            }

            Quiz = quiz;

            var quizResult = quiz.QuizResults.FirstOrDefault();
            if (quizResult == null || quizResult.Grade == null)
            {
                GradeString = "-";
            }
            else
            {
                GradeString = ((Grade)quizResult.Grade).AsString();
            }

            Questions = quiz.Excersises.Select(e => e.Question).ToList();
            Answers = quiz.Excersises
                .Select(e => e.ExcersiseSolutions.FirstOrDefault()?.Answer ?? "")
                .ToList();

            // TODO when added max points to excersise, assign this max points
            ExerciseResults = quiz.Excersises
                .Select(e => e.ExcersiseSolutions.FirstOrDefault()?.ExcersiseResult)
                .Select(result => result == null
                    ? new ExerciseResultData { Points = 0 }
                    : new ExerciseResultData
                    {
                        Points = result.Points,
                        MaxPoints = result.MaxPoints,
                        Comment = result.Comment
                    })
                .ToList();
            return Page();
        }
    }


}
