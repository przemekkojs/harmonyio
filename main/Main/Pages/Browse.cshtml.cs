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

        public string Opinion { get; private set; } = "Brak opinii";
        public string GradeString { get; set; } = "";
        public Quiz Quiz { get; set; } = null!;
        public List<string> Questions = [];
        public List<string> Answers = [];
        public List<ExerciseResultData> ExerciseResults = [];

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
                    .Include(q => q.Exercises)
                        .ThenInclude(e => e.ExerciseSolutions.Where(es => es.UserId == appUser.Id))
                            .ThenInclude(es => es.ExerciseResult)
                    .Include(q => q.QuizResults.Where(qr => qr.UserId == appUser.Id))
                    .Include(q => q.Participants.Where(p => p.Id == appUser.Id))
            );

            if (quiz == null)
                return RedirectToPage("Error");

            var quizNotStarted = quiz.State == QuizState.NotStarted;
            var userIsParticipant = quiz.Participants.Any(p => p.Id == appUser.Id);

            // quiz isnt started or user isnt participant so forbid browsing it
            if (quizNotStarted || !userIsParticipant)
                return Forbid();


            var quizResult = quiz.QuizResults.FirstOrDefault(qr => qr.UserId == appUser.Id);
            if (quizResult == null || quizResult.Grade == null)
            {
                // quiz is open and quiz result isnt set
                if (quiz.State == QuizState.Open)
                    return RedirectToPage("Solve", new { code = quiz.Code });

                // here quiz must be closed so set grade to -
                GradeString = "-";
            }
            else
                GradeString = ((Grade)quizResult.Grade).AsString();

            Quiz = quiz;
            Questions = quiz.Exercises.Select(e => e.Question).ToList();
            Answers = quiz.Exercises
                .Select(e => e.ExerciseSolutions.FirstOrDefault(es => es.UserId == appUser.Id)?.Answer ?? string.Empty)
                .ToList();

            var showOpinion = quiz.ShowAlgorithmOpinion;

            // Tutaj robimy b��dy
            var excersiseResult = quizResult?.ExerciseResults
                .FirstOrDefault();

            if (excersiseResult != null)
            {
                _repository.Context.Entry(excersiseResult)
                    .Collection(er => er.MistakeResults)
                    .Load();

                var mistakeResults = excersiseResult?.MistakeResults ?? [];

                if (showOpinion)
                    Opinion = Utils.Utils.MistakesToHTML(mistakeResults);
                else
                    Opinion = string.Empty;
            }
            else
                Opinion = string.Empty;

            // TODO when added max points to excersise, assign this max points
            ExerciseResults = quiz.Exercises
            .Select(e => new
            {
                e.ExerciseSolutions.FirstOrDefault(es => es.UserId == appUser.Id)?.ExerciseResult,
                e.MaxPoints
            })
            .Select(x => x.ExerciseResult == null
                ? new ExerciseResultData
                {
                    Points = 0,
                    MaxPoints = x.MaxPoints
                }
                : new ExerciseResultData
                {
                    Points = x.ExerciseResult.Points,
                    MaxPoints = x.MaxPoints,
                    Comment = x.ExerciseResult.Comment
                })
            .ToList();
            return Page();
        }
    }
}
