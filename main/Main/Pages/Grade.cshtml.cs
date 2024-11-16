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
using NuGet.Packaging;

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

        public string QuizName { get; set; } = "";
        public List<ApplicationUser> Users { get; set; } = [];
        public List<List<string>> Solutions { get; set; } = [];
        public List<Excersise> Excersises { get; set; } = [];
        public List<List<int>> PointSuggestions { get; set; } = [];
        public List<List<string>> Opinions { get; set; } = [];

        [BindProperty]
        public int QuizId { get; set; }
        [BindProperty]
        public List<string> UserIds { get; set; } = [];
        [BindProperty]
        public List<Grade?> Grades { get; set; } = []; // grade of each user
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

            QuizId = quiz.Id;
            QuizName = quiz.Name;
            Excersises = [.. quiz.Excersises];

            var allSolutions = quiz.Excersises.SelectMany(e => e.ExcersiseSolutions).ToList();
            var participantsAnsweredIds = allSolutions.Select(es => es.UserId).ToHashSet();

            // fill missing excersises only if quiz is closed
            if (quiz.State == QuizState.Closed && participantsAnsweredIds.Count != quiz.Participants.Count)
            {
                var participantsNotAnsweredIds = quiz.Participants.Select(p => p.Id).Except(participantsAnsweredIds);
                var newSolutions = await FillMissingExcersiseSolutionsAndResults(participantsNotAnsweredIds, quiz.Excersises);
                allSolutions.AddRange(newSolutions);
                participantsAnsweredIds.AddRange(newSolutions.Select(es => es.UserId));
            }

            // this is where nobody solved the quiz, maybe should be handled differently
            if (allSolutions.Count == 0)
            {
                return RedirectToPage("Error");
            }

            // just to make sure that soutions are in good order
            allSolutions = allSolutions.OrderBy(es => es.ExcersiseId).ToList();

            var userIdToSolutions = allSolutions
                .GroupBy(es => es.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var userIdToSolutionResults = userIdToSolutions
                .ToDictionary(
                    userToSolutions => userToSolutions.Key,
                    userToSolutions => userToSolutions.Value
                        .Select(es => es.ExcersiseResult)
                        .ToList()
                );

            var userIdToQuizResult = userIdToSolutions.Keys
                .ToDictionary(
                    userId => userId,
                    userId => quiz.QuizResults.FirstOrDefault(qr => qr.UserId == userId)
                );

            Users = quiz.Participants.Where(p => participantsAnsweredIds.Contains(p.Id)).ToList();
            Users = Users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToList();
            UserIds = Users.Select(u => u.Id).ToList();

            foreach (var userId in UserIds)
            {
                Grades.Add(userIdToQuizResult[userId]?.Grade ?? null);
                Solutions.Add(userIdToSolutions[userId].Select(es => es.Answer).ToList());

                var userSolutionResults = userIdToSolutionResults[userId];
                Points.Add(userSolutionResults.Select(er => er?.Points ?? 0).ToList());
                Comments.Add(userSolutionResults.Select(er => er?.Comment ?? "").ToList());
                PointSuggestions.Add(userSolutionResults.Select(er => er?.AlgorithmPoints ?? 0).ToList());
                // przemo tu wlasnie bedziesz tworzyl te elementy html i wkladal zamiast tego co ponizej
                Opinions.Add(userSolutionResults.Select(er => er?.AlgorithmOpinion ?? "").ToList());
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return Forbid();
            }

            var quiz = await _repository.GetAsync<Quiz>(
                q => q.Id == QuizId,
                query => query
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

            var allSolutions = quiz.Excersises.SelectMany(e => e.ExcersiseSolutions).ToList().OrderBy(es => es.ExcersiseId).ToList();
            var participantsAnsweredIds = allSolutions.Select(es => es.UserId).ToHashSet();

            var userIdToSolutions = allSolutions
                .GroupBy(es => es.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var userIdToSolutionResults = userIdToSolutions
                .ToDictionary(
                    userToSolutions => userToSolutions.Key,
                    userToSolutions => userToSolutions.Value
                        .Select(es => es.ExcersiseResult)
                        .ToList()
                );

            var userIdToQuizResult = userIdToSolutions.Keys
                .ToDictionary(
                    userId => userId,
                    userId => quiz.QuizResults.FirstOrDefault(qr => qr.UserId == userId)
                );

            var gradingTime = DateTime.Now;

            for (int i = 0; i < UserIds.Count; i++)
            {
                var curUserId = UserIds[i];
                // trying to grade user who didnt submit solution, if quiz is closed then the solutions are automatically filled in onGet
                if (!participantsAnsweredIds.Contains(curUserId))
                {
                    continue;
                }

                var curGrade = Grades[i];
                var curQuizResult = userIdToQuizResult[curUserId];
                if (curQuizResult == null)
                {
                    curQuizResult = new QuizResult
                    {
                        Grade = curGrade,
                        QuizId = QuizId,
                        UserId = curUserId,
                        GradeDate = gradingTime
                    };
                    _repository.Add(curQuizResult);
                }
                else
                {
                    curQuizResult.Grade = curGrade;
                    curQuizResult.GradeDate = gradingTime;
                    _repository.Update(curQuizResult);
                }

                var curSolutionResults = userIdToSolutionResults[curUserId];
                for (int j = 0; j < curSolutionResults.Count; j++)
                {
                    var curSolutionResult = curSolutionResults[j]!;

                    curSolutionResult.Points = Points[i][j];
                    curSolutionResult.Comment = Comments[i][j] ?? "";
                    curSolutionResult.QuizResult = curQuizResult;
                }
            }

            await _repository.SaveChangesAsync();
            return RedirectToPage("Created");
        }

        private async Task<List<ExcersiseSolution>> FillMissingExcersiseSolutionsAndResults(IEnumerable<string> participantsNotAnsweredIds, IEnumerable<Excersise> excersises)
        {
            List<ExcersiseSolution> newSolutions = [];
            foreach (var excersise in excersises)
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
