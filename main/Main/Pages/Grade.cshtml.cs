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

        public Dictionary<ApplicationUser, List<ExcersiseSolution>> UsersToSolutions { get; set; } = [];
        public Dictionary<ExcersiseSolution, (int, int, string)> SolutionsToGradings { get; set; } = [];
        public Dictionary<ApplicationUser, List<(int, int, string)>> UsersToGradings { get; set; } = [];


        public Quiz Quiz { get; set; } = null!;
        public List<ApplicationUser> Users { get; set; } = [];
        public List<List<string>> Solutions { get; set; } = [];
        public List<Excersise> Excersises { get; set; } = [];

        [BindProperty]
        public int QuizId { get; set; }
        [BindProperty]
        public List<string> UserIds { get; set; } = [];
        [BindProperty]
        public List<Grade?> Grades { get; set; } = []; // grade of each user
        public List<List<int>> PointSuggestions { get; set; } = [];
        public List<List<string>> Opinions { get; set; } = [];
        [BindProperty]
        public List<List<int>> Points { get; set; } = []; //user, then points for excersise soltion
        [BindProperty]
        public List<List<string>> Comments { get; set; } = []; //user, then comment for excersise solution

        public GradeModel(
            ApplicationRepository repository,
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _repository = repository;
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
                    .Include(q => q.Participants)
                    .Include(q => q.QuizResults)
                    .Include(q => q.Excersises)
                        .ThenInclude(e => e.ExcersiseSolutions)
                            .ThenInclude(es => es.ExcersiseResult)
                    .Include(q => q.PublishedToGroup)
                        .ThenInclude(q => q.Teachers.Where(u => u.Id == appUser.Id))
            );

            if (quiz == null)
            {
                return RedirectToPage("Error");
            }

            // check if user can grade quiz
            // he cant if he is not creator of quiz or he isnt teacher or master in any of the groups the quiz is published to
            if (
                quiz.CreatorId != appUser.Id &&
                !quiz.PublishedToGroup.Any(g => g.MasterId == appUser.Id || g.Teachers.Count != 0))
            {
                return Forbid();
            }

            Quiz = quiz;
            Excersises = [.. Quiz.Excersises];

            var allSolutions = quiz.Excersises.SelectMany(e => e.ExcersiseSolutions).ToList();
            var participantsAnsweredIds = allSolutions.Select(es => es.UserId).ToHashSet();

            // fill missing excersises only if quiz is closed
            if (quiz.State == QuizState.Closed && participantsAnsweredIds.Count != quiz.Participants.Count)
            {
                var participantsNotAnsweredIds = quiz.Participants.Select(p => p.Id).Except(participantsAnsweredIds);
                var newSolutions = await FillMissingExcersiseSolutionsAndResults(participantsNotAnsweredIds);
                allSolutions.AddRange(newSolutions);
            }

            // this is where nobody solved the quiz, maybe should be handled differently
            if (allSolutions.Count == 0)
            {
                return RedirectToPage("Error");
            }

            // just to make sure that soutions are in good order
            allSolutions = allSolutions.OrderBy(es => es.ExcersiseId).ToList();

            var usersToSolutions = allSolutions
                .GroupBy(es => es.User)
                .ToDictionary(g => g.Key, g => g.ToList());

            var usersToSolutionResults = usersToSolutions
                .ToDictionary(
                    userToSolutions => userToSolutions.Key,
                    userToSolutions => userToSolutions.Value
                        .Select(es => es.ExcersiseResult)
                        .ToList()
                );

            var usersToQuizResult = usersToSolutions.Keys
                .ToDictionary(
                    user => user,
                    user => quiz.QuizResults.FirstOrDefault(qr => qr.UserId == user.Id)
                );

            Users = [.. usersToSolutions.Keys];
            Users = Users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToList();

            UserIds = Users.Select(u => u.Id).ToList();

            foreach (var user in Users)
            {
                Grades.Add(usersToQuizResult[user]?.Grade ?? null);
                Solutions.Add(usersToSolutions[user].Select(es => es.Answer).ToList());

                var userSolutionResults = usersToSolutionResults[user];
                Points.Add(userSolutionResults.Select(er => er?.Points ?? 0).ToList());
                Comments.Add(userSolutionResults.Select(er => er?.Comment ?? "").ToList());
                PointSuggestions.Add(userSolutionResults.Select(er => er?.AlgorithmPoints ?? 0).ToList());
                // przemo tu wlasnie bedziesz tworzyl te elementy html i wkladal zamiast tego co ponizej
                Opinions.Add(userSolutionResults.Select(er => er?.AlgorithmOpinion ?? "").ToList());
            }

            return Page();
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

            Quiz = quiz;

            await FillMissingExcersiseSolutions();

            UsersToSolutions = Quiz.Excersises
                .SelectMany(e => e.ExcersiseSolutions)
                .GroupBy(es => es.User)
                .ToDictionary(g => g.Key, g => g.ToList());

            // To si� wysypuje - ExcersiseResult jest zawsze nullem
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
            return true;
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

        private async Task<List<ExcersiseSolution>> FillMissingExcersiseSolutionsAndResults(IEnumerable<string> participantsNotAnsweredIds)
        {
            List<ExcersiseSolution> newSolutions = [];
            foreach (var excersise in Quiz.Excersises)
            {
                foreach (var participantId in participantsNotAnsweredIds)
                {
                    var solution = new ExcersiseSolution()
                    {
                        ExcersiseId = excersise.Id,
                        Answer = "",
                        UserId = participantId,
                    };
                    _repository.Add(solution);

                    var excersiseResult = new ExcersiseResult
                    {
                        Comment = "",
                        Points = 0,
                        AlgorithmPoints = 0,
                        MaxPoints = excersise.MaxPoints,
                        AlgorithmOpinion = "",
                        ExcersiseSolution = solution,
                    };
                    _repository.Add(excersiseResult);
                    newSolutions.Add(solution);
                }
            }

            await _repository.SaveChangesAsync();
            return newSolutions;
        }
    }
}
