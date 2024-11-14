using Main.GradingAlgorithm;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Main.Enumerations;
using Humanizer;
using Microsoft.AspNetCore.Authorization;

namespace Main.Pages
{
    [Authorize]
    public class GradeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;
        private readonly IGradingAlgorithm _algorithm;

        public Quiz Quiz { get; set; } = null!;
        public ApplicationUser AppUser { get; set; } = null!;

        public List<ApplicationUser> Users { get; set; } = null!;
        public List<Excersise> Excersises { get; set; } = null!;
        public Dictionary<ApplicationUser, List<ExcersiseSolution>> UsersToSolutions { get; set; } = null!;
        public Dictionary<ExcersiseSolution, (int, int, string)> SolutionsToGradings { get; set; } = null!;
        public Dictionary<ApplicationUser, List<(int, int, string)>> UsersToGradings { get; set; } = null!;

        [BindProperty]
        public int QuizId { get; set; }
        [BindProperty]
        public List<Grade?> Grades { get; set; } = null!;
        [BindProperty]
        public List<List<int>> Points { get; set; } = null!; //user, then excersise
        [BindProperty]
        public List<int> Maxes { get; set; } = null!;  //user, then excersise
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
                        .ThenInclude(qr => qr.ExcersiseResults)
                    .Include(q => q.Excersises)
                        .ThenInclude(e => e.ExcersiseSolutions)
                            .ThenInclude(es => es.User)
            );

            if (quiz == null || currentUser == null ||
                quiz.CreatorId != currentUser.Id 
                    // ||
                    // (quiz.State == QuizState.Open 
                    // && quiz.Excersises.First().ExcersiseSolutions.Count < quiz.Participants.Count
                    // )
                )
            {
                return false;
            }

            AppUser = currentUser;
            Quiz = quiz;

            await FillMissingExcersiseSolutions();

            UsersToSolutions = Quiz.Excersises
                .SelectMany(e => e.ExcersiseSolutions)
                .GroupBy(es => es.User)
                .ToDictionary(g => g.Key, g => g.ToList());

            // To siê wysypuje - ExcersiseResult jest zawsze nullem
            SolutionsToGradings = Quiz.Excersises
                .SelectMany(e => e.ExcersiseSolutions, (e, es) => (e.Question, es))
                .ToDictionary(
                    tuple => tuple.es,
                    tuple => (tuple.es.ExcersiseResult.Points, tuple.es.Excersise.MaxPoints, tuple.es.ExcersiseResult.AlgorithmOpinion)
                );

            UsersToGradings = UsersToSolutions
                .ToDictionary(
                    userToSolutions => userToSolutions.Key,
                    userToSolutions => userToSolutions.Value
                        .Select(solution => SolutionsToGradings[solution])
                        .ToList()
                );

            Users = [.. UsersToSolutions.Keys];
            Excersises = [.. Quiz.Excersises];
            Maxes = Excersises
                .Select(x => x.MaxPoints)
                .ToList();

            return true;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            bool success = await Init(id);

            if (!success)
            {
                return Forbid();
            }

            if (Quiz.QuizResults.Count > 0)
            {
                Grades = Users.Select(u => Quiz.QuizResults.First(qr => qr.UserId == u.Id).Grade).ToList();

                var pmc = Users.Select(u => Quiz.QuizResults
                        .First(qr => qr.UserId == u.Id).ExcersiseResults
                            .Select(er => (er.Points, er.MaxPoints, er.Comment))
                        .ToList())
                    .ToList();

                Points = pmc
                    .Select(innerList => innerList.Select(t => t.Points).ToList())
                    .ToList();

                Comments = pmc
                    .Select(innerList => innerList.Select(t => t.Comment).ToList())
                    .ToList();
            }
            // TODO: To trzeba jakoœ zrobiæ sprytniej
            //else
            //{
            //    Grades = [];
            //    Points = [];
            //    Comments = [];

            //    foreach (var user in Users)
            //    {
            //        Grades.Add(Grade.One);

            //        var pointsToAdd = new List<int>();
            //        var commentsToAdd = new List<string>();

            //        foreach (var ex in SolutionsToGradings.Keys)
            //        {
            //            var mark = SolutionsToGradings[ex];
            //            pointsToAdd.Add(mark.Item1);
            //            commentsToAdd.Add("");
            //        }

            //        Points.Add(pointsToAdd);
            //        Comments.Add(commentsToAdd);
            //    }
            //}

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
            if (Quiz.QuizResults.Count == 0)
            {
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
            }
            else
            {
                for (int i = 0; i < Users.Count; i++)
                {
                    var quizResult = Quiz.QuizResults.First(qr => qr.UserId == Users[i].Id);

                    quizResult.QuizId = QuizId;
                    quizResult.UserId = Users[i].Id;
                    quizResult.Grade = (Grade)Grades[i]!;

                    quizResults.Add(quizResult);
                    _repository.Update(quizResult);

                    foreach (var res in quizResult.ExcersiseResults)
                    {
                        _repository.Delete(res);
                    }
                }
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
                        Comment = Comments[i][j] ?? "",
                        MaxPoints = Excersises[j].MaxPoints
                    };
                    _repository.Add(excersiseResult);
                }
            }
            
            await _repository.SaveChangesAsync();

            return RedirectToPage("Created");
        }

        private async Task FillMissingExcersiseSolutions()
        {
            bool changedAnything = false;
            foreach (var excersise in Quiz.Excersises)
            {
                var participantsAnswered = excersise.ExcersiseSolutions.Select(es => es.UserId).ToHashSet();
                var participantsNotAnswered = Quiz.Participants.Where(p => !participantsAnswered.Contains(p.Id));

                foreach (var participant in participantsNotAnswered)
                {
                    var solution = new ExcersiseSolution()
                    {
                        ExcersiseId = excersise.Id,
                        Answer = "",
                        UserId = participant.Id,
                    };
                    _repository.Add(solution);
                    changedAnything = true;
                }
            }

            if (changedAnything)
                await _repository.SaveChangesAsync();
        }
    }
}
