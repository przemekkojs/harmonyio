using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Main.Enumerations;
using Microsoft.AspNetCore.Authorization;
using NuGet.Packaging;

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

            if (!CanUserGradeQuiz(quiz, appUser.Id))
                return Forbid();

            QuizId = quiz.Id;
            ShareAlgorithmOpinion = quiz.ShowAlgorithmOpinion;
            QuizName = quiz.Name;
            Exercises = [.. quiz.Exercises.OrderBy(e => e.Id)];

            var allSolutions = GetAllExerciseSolutions(Exercises);
            var participantsAnsweredIds = GetParticipantsWhoAnsweredIds(allSolutions);

            // fill missing exercises only if quiz is closed
            if (quiz.State == QuizState.Closed && participantsAnsweredIds.Count != quiz.Participants.Count)
            {
                var participantsNotAnsweredIds = quiz.Participants.Where(p => !participantsAnsweredIds.Contains(p.Id)).Select(p => p.Id);
                var newSolutions = await FillMissingExerciseSolutionsAndResults(participantsNotAnsweredIds, quiz.Exercises);
                allSolutions.AddRange(newSolutions);
                participantsAnsweredIds.AddRange(newSolutions.Select(es => es.UserId));
            }

            // this is where nobody solved the quiz,
            // TODO: maybe should be handled differently
            var noSolutions = allSolutions.Count == 0;
            if (noSolutions)
                return RedirectToPage("Error");

            // just to make sure that soutions are in good order
            allSolutions = [.. allSolutions.OrderBy(es => es.ExerciseId)];

            var userIdToSolutions = GetUserIdToSolutions(allSolutions);
            var userIdToSolutionResults = GetUserIdToExerciseResults(userIdToSolutions);
            var userIdToQuizResult = GetUserIdToQuizResult(userIdToSolutions, quiz);

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
                    .Select(er => er?.Comment ?? string.Empty)
                    .ToList());

                PointSuggestions.Add(userSolutionResults
                    .Select(er => er?.AlgorithmPoints ?? 0)
                    .ToList());

                Opinions.Add(userSolutionResults
                    .Select(er => Utils.Utils.MistakesToHTML(er?.MistakeResults ?? []) ?? "Puste rozwiązanie.")
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

            if (
                UserIds.Count != Grades.Count ||
                UserIds.Count != Points.Count ||
                UserIds.Count != Comments.Count
            )
            {
                return RedirectToPage("Error");
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

            if (!CanUserGradeQuiz(quiz, appUser.Id))
                return Forbid();

            var allSolutions = GetAllExerciseSolutions(quiz.Exercises);
            var participantsAnsweredIds = GetParticipantsWhoAnsweredIds(allSolutions);
            var userIdToSolutions = GetUserIdToSolutions(allSolutions);
            var userIdToSolutionResults = GetUserIdToExerciseResults(userIdToSolutions);
            var userIdToQuizResult = GetUserIdToQuizResult(userIdToSolutions, quiz);

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

                    curSolutionResult.Points = Points[i][j] <= curSolutionResult.MaxPoints ? Points[i][j] : curSolutionResult.MaxPoints;
                    curSolutionResult.Comment = Comments[i][j] ?? string.Empty;
                    curSolutionResult.QuizResult = curQuizResult;
                }
            }

            quiz.ShowAlgorithmOpinion = ShareAlgorithmOpinion;
            _repository.Update(quiz);

            await _repository.SaveChangesAsync();
            return RedirectToPage("Created");
        }

        private static List<ExerciseSolution> GetAllExerciseSolutions(ICollection<Exercise> exercises)
        {
            return [.. exercises
                .SelectMany(e => e.ExerciseSolutions)
                .OrderBy(es => es.ExerciseId)];
        }

        private static HashSet<string> GetParticipantsWhoAnsweredIds(ICollection<ExerciseSolution> solutions)
        {
            return solutions.Select(es => es.UserId).ToHashSet();
        }

        private static Dictionary<string, List<ExerciseSolution>> GetUserIdToSolutions(ICollection<ExerciseSolution> solutions)
        {
            return solutions
                .GroupBy(es => es.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        private static Dictionary<string, List<ExerciseResult?>> GetUserIdToExerciseResults(Dictionary<string, List<ExerciseSolution>> userIdToSolutions)
        {
            return userIdToSolutions
                .ToDictionary(
                    userToSolutions => userToSolutions.Key,
                    userToSolutions => userToSolutions.Value
                        .Select(es => es.ExerciseResult)
                        .ToList()
                );
        }

        private static Dictionary<string, QuizResult?> GetUserIdToQuizResult(Dictionary<string, List<ExerciseSolution>> userIdToSolutions, Quiz quiz)
        {
            return userIdToSolutions.Keys
                .ToDictionary(
                    userId => userId,
                    userId => quiz.QuizResults.FirstOrDefault(qr => qr.UserId == userId)
                );
        }

        private static bool CanUserGradeQuiz(Quiz quiz, string userId)
        {
            var userIsCreator = quiz.CreatorId == userId;
            var userIsMaster = quiz.PublishedToGroup.Any(g => g.MasterId == userId || g.Teachers.Any(t => t.Id == userId));

            return userIsCreator || userIsMaster;
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

                    var emptySolutionResult = new MistakeResult()
                    {
                        Bars = [],
                        Functions = [],
                        MistakeCodes = [999]
                    };

                    var exerciseResult = new ExerciseResult
                    {
                        Points = 0,
                        MaxPoints = exercise.MaxPoints,
                        Comment = string.Empty,
                        AlgorithmPoints = 0,
                        ExerciseSolution = solution,
                        MistakeResults = [emptySolutionResult]
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