using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Main.Enumerations;
using Microsoft.AspNetCore.Authorization;
using NuGet.Packaging;
using Algorithm.New.Algorithm.Mistake.Solution;

namespace Main.Pages
{
    [Authorize]
    public class GradeModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        public Dictionary<ApplicationUser, List<ExerciseSolution>> UsersToSolutions { get; set; } = [];
        public Dictionary<ExerciseSolution, (int, int, string)> SolutionsToGradings { get; set; } = [];
        public Dictionary<ApplicationUser, List<(int, int, string)>> UsersToGradings { get; set; } = [];

        public string QuizName { get; set; } = "";
        public List<ApplicationUser> Users { get; set; } = [];
        public List<List<string>> Solutions { get; set; } = [];
        public List<Exercise> Exercises { get; set; } = [];
        public List<List<int>> PointSuggestions { get; set; } = [];
        public List<List<string>> Opinions { get; set; } = [];

        [BindProperty]
        public int QuizId { get; set; }
        [BindProperty]
        public List<string> UserIds { get; set; } = [];
        [BindProperty]
        public List<Grade?> Grades { get; set; } = []; // grade of each user
        [BindProperty]
        public List<List<int>> Points { get; set; } = []; //user, then points for exercise solution
        [BindProperty]
        public List<List<string>> Comments { get; set; } = []; //user, then comment for exercise solution
        [BindProperty]
        public bool ShareAlgorithmOpinion { get; set; }

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
                return Forbid();

            var quiz = await _repository.GetAsync<Quiz>(
                q => q.Id == id,
                query => query
                    .Include(q => q.Participants)
                    .Include(q => q.QuizResults)
                    .Include(q => q.Exercises)
                        .ThenInclude(e => e.ExerciseSolutions)
                            .ThenInclude(es => es.ExerciseResult!)
                                .ThenInclude(er => er.MistakeResults)
                    .Include(q => q.PublishedToGroup)
                        .ThenInclude(q => q.Teachers.Where(u => u.Id == appUser.Id))
            );

            if (quiz == null)
                return RedirectToPage("Error");

            // check if user can grade quiz
            // he cant if he is not creator of quiz or he isnt teacher or master in any of the groups the quiz is published to
            var userIsNotCreator = quiz.CreatorId != appUser.Id;
            var isMasterOfGroup = quiz.PublishedToGroup.Any(g => g.MasterId == appUser.Id || g.Teachers.Any(t => t.Id == appUser.Id));

            if (userIsNotCreator && !isMasterOfGroup)
                return Forbid();

            QuizId = quiz.Id;
            ShareAlgorithmOpinion = quiz.ShowAlgorithmOpinion;
            QuizName = quiz.Name;
            Exercises = [.. quiz.Exercises];

            var allSolutions = quiz.Exercises.SelectMany(e => e.ExerciseSolutions).ToList();
            var participantsAnsweredIds = allSolutions.Select(es => es.UserId).ToHashSet();

            // fill missing exercises only if quiz is closed
            if (quiz.State == QuizState.Closed && participantsAnsweredIds.Count != quiz.Participants.Count)
            {
                var participantsNotAnsweredIds = quiz.Participants.Where(p => !participantsAnsweredIds.Contains(p.Id)).Select(p => p.Id);
                var newSolutions = await FillMissingExerciseSolutionsAndResults(participantsNotAnsweredIds, quiz.Exercises);
                allSolutions.AddRange(newSolutions);
                participantsAnsweredIds.AddRange(newSolutions.Select(es => es.UserId));
            }

            // this is where nobody solved the quiz, maybe should be handled differently
            var noSolutions = allSolutions.Count == 0;
            if (noSolutions)
                return RedirectToPage("Error");

            // just to make sure that soutions are in good order
            allSolutions = [.. allSolutions.OrderBy(es => es.ExerciseId)];

            var userIdToSolutions = allSolutions
                .GroupBy(es => es.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var userIdToSolutionResults = userIdToSolutions
                .ToDictionary(
                    userToSolutions => userToSolutions.Key,
                    userToSolutions => userToSolutions.Value
                        .Select(es => es.ExerciseResult)
                        .ToList()
                );

            var userIdToQuizResult = userIdToSolutions.Keys
                .ToDictionary(
                    userId => userId,
                    userId => quiz.QuizResults.FirstOrDefault(qr => qr.UserId == userId)
                );

            Users = quiz.Participants
                .Where(p => participantsAnsweredIds.Contains(p.Id))
                .ToList();

            Users = [.. Users
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)];

            UserIds = Users
                .Select(u => u.Id)
                .ToList();

            foreach (var userId in UserIds)
            {
                Grades.Add(userIdToQuizResult[userId]?.Grade ?? null);

                Solutions.Add(userIdToSolutions[userId]
                    .Select(es => es.Answer)
                    .ToList());

                var userSolutionResults = userIdToSolutionResults[userId];

                Points.Add(userSolutionResults
                    .Select(er => er?.Points ?? 0)
                    .ToList());

                Comments.Add(userSolutionResults
                    .Select(er => er?.Comment ?? "")
                    .ToList());

                PointSuggestions.Add(userSolutionResults
                    .Select(er => er?.AlgorithmPoints ?? 0)
                    .ToList());

                // foreach (var result in userSolutionResults)
                // {
                //     if (result == null)
                //         continue;

                //     _repository.Context.Entry(result)
                //         .Collection(er => er.MistakeResults)
                //         .Load();
                // }

                Opinions.Add(userSolutionResults
                    .Select(er => Utils.Utils.MistakesToHTML(er?.MistakeResults ?? []) ?? "Brak błędów.")
                    .ToList());
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
                    .Include(q => q.Exercises)
                        .ThenInclude(e => e.ExerciseSolutions)
                            .ThenInclude(es => es.ExerciseResult)
                    .Include(q => q.PublishedToGroup)
                        .ThenInclude(q => q.Teachers.Where(u => u.Id == appUser.Id))
            );

            if (quiz == null)
                return RedirectToPage("Error");

            // check if user can grade quiz
            // he cant if he is not creator of quiz or he isnt teacher or master in any of the groups the quiz is published to
            var userIsNotCreator = quiz.CreatorId != appUser.Id;
            var userIsMaster = quiz.PublishedToGroup.Any(g => g.MasterId == appUser.Id || g.Teachers.Any(t => t.Id == appUser.Id));

            if (userIsNotCreator && !userIsMaster)
                return Forbid();

            var allSolutions = quiz.Exercises
                .SelectMany(e => e.ExerciseSolutions)
                .OrderBy(es => es.ExerciseId)
                .ToList();

            var participantsAnsweredIds = allSolutions
                .Select(es => es.UserId)
                .ToHashSet();

            var userIdToSolutions = allSolutions
                .GroupBy(es => es.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var userIdToSolutionResults = userIdToSolutions
                .ToDictionary(
                    userToSolutions => userToSolutions.Key,
                    userToSolutions => userToSolutions.Value
                        .Select(es => es.ExerciseResult)
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
                var hasParticipantAnswered = participantsAnsweredIds.Contains(curUserId);
                if (!hasParticipantAnswered)
                    continue; // user hasnt answered so continue without grading him

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

            quiz.ShowAlgorithmOpinion = ShareAlgorithmOpinion;
            _repository.Update(quiz);

            await _repository.SaveChangesAsync();
            return RedirectToPage("Created");
        }

        private async Task<List<ExerciseSolution>> FillMissingExerciseSolutionsAndResults(IEnumerable<string> participantsNotAnsweredIds, IEnumerable<Exercise> exercises)
        {
            List<ExerciseSolution> newSolutions = [];
            foreach (var exercise in exercises)
            {
                foreach (var participantId in participantsNotAnsweredIds)
                {
                    var solution = new ExerciseSolution()
                    {
                        ExerciseId = exercise.Id,
                        Answer = "",
                        UserId = participantId,
                    };

                    _repository.Add(solution);

                    var exerciseResult = new ExerciseResult
                    {
                        Points = 0,
                        MaxPoints = exercise.MaxPoints,
                        Comment = string.Empty,
                        AlgorithmPoints = 0,
                        ExerciseSolution = solution,
                        // TODO add mistake results that say exercise wasnt solved
                        MistakeResults = []
                    };

                    _repository.Add(exerciseResult);
                    newSolutions.Add(solution);
                }
            }

            await _repository.SaveChangesAsync();
            return newSolutions;
        }
    }
}