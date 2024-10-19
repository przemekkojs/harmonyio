using Main.Algorithm;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Main.Enumerations;
using Humanizer;

namespace Main.Pages
{
    public class GradeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;
        private readonly IGradingAlgorithm _algorithm;

        public Quiz Quiz { get; set; } = null!;

        public List<ApplicationUser> Users { get; set; } = null!;
        public List<Excersise> Excersises { get; set; } = null!;
        public Dictionary<ApplicationUser, List<ExcersiseSolution>> UsersToSolutions { get; set; } = null!;
        public Dictionary<ExcersiseSolution, (int, int)> SolutionsToGradings { get; set; } = null!;
        public Dictionary<ApplicationUser, List<(int, int)>> UsersToGradings { get; set; } = null!;

        [BindProperty]
        public int QuizId { get; set; }
        [BindProperty]
        public List<Grade?> Grades { get; set; } = null!;
        [BindProperty]
        public List<List<int>> Points { get; set; } = null!; //user, then excersise
        [BindProperty]
        public List<List<int>> Maxes { get; set; } = null!;  //user, then excersise
        [BindProperty]
        public List<List<string>> Comments { get; set; } = null!;

        public GradeModel(
            ApplicationRepository repository,
            UserManager<ApplicationUser> userManager,
            IGradingAlgorithm algorithm)
        {
            _userManager = userManager;
            _repository = repository;
            _algorithm = algorithm;
        }

        private async Task<bool> Init(int quizId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var quiz = await _repository.GetAsync<Quiz>(
                q => q.Id == quizId,
                query => query
                    .Include(q => q.Participants)
                    .Include(q => q.QuizResults)
                    .Include(q => q.Excersises)
                    .ThenInclude(e => e.ExcersiseSolutions)
                    .ThenInclude(es => es.User)
            );

            if (quiz == null || currentUser == null ||
                quiz.CreatorId != currentUser.Id ||
                quiz.QuizResults.Count > 0 ||
                (quiz.State == QuizState.Open && quiz.Excersises.First().ExcersiseSolutions.Count < quiz.Participants.Count))
            {
                return false;
            }

            Quiz = quiz;

            UsersToSolutions = Quiz.Excersises
                .SelectMany(e => e.ExcersiseSolutions)
                .GroupBy(es => es.User)
                .ToDictionary(g => g.Key, g => g.ToList());

            SolutionsToGradings = Quiz.Excersises
                .SelectMany(e => e.ExcersiseSolutions, (e, es) => (e.Question, es))
                .ToDictionary(
                    tuple => tuple.es,
                    tuple => _algorithm.Grade(tuple.Question, tuple.es.Answer)
                );

            UsersToGradings = UsersToSolutions
                .ToDictionary(
                    userToSolutions => userToSolutions.Key,
                    userToSolutions => userToSolutions.Value
                        .Select(solution => SolutionsToGradings[solution])
                        .ToList()
                );

            Users = UsersToSolutions.Keys.ToList();
            Excersises = Quiz.Excersises.ToList();

            return true;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            bool success = await Init(id);
            if (!success)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            bool success = await Init(QuizId);
            if (!success)
            {
                return RedirectToPage("Error");
            }

            if (Grades.Any(g => g == null))
            {
                ModelState.AddModelError(nameof(Grades), "Some students don't have their grades"); ;
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var quizResults = new List<QuizResult>();
            for (int i = 0; i < Users.Count; i++)
            {
                var quizResult = new QuizResult()
                {
                    QuizId = QuizId,
                    UserId = Users[i].Id,
                    Grade = (Grade)Grades[i]!,
                };

                quizResults.Add(quizResult);
                _repository.Add(quizResult);
            }
            await _repository.SaveChangesAsync();

            for (int i = 0; i < Users.Count; i++)
            {
                for (int j = 0; j < Excersises.Count; j++)
                {
                    var excersiseResult = new ExcersiseResult()
                    {
                        QuizResultId = quizResults[i].Id,
                        ExcersiseSolutionId = UsersToSolutions[Users[i]][j].Id,
                        Points = Points[i][j],
                        MaxPoints = Maxes[i][j],
                        Comment = Comments[i][j] ?? "",
                    };
                    _repository.Add(excersiseResult);
                }
            }
            await _repository.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
