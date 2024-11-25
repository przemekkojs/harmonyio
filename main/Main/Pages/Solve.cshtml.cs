using Main.Data;
using Main.GradingAlgorithm;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Main.Pages
{
    [Authorize]
    public class SolveModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;
        private readonly IGradingAlgorithm _algorithm;

        public Quiz Quiz { get; set; } = null!;

        [BindProperty]
        public int QuizId { get; set; }
        [BindProperty]
        public List<string> Answers { get; set; } = new();

        public SolveModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager, IGradingAlgorithm algorithm)
        {
            _repository = repository;
            _userManager = userManager;
            _algorithm = algorithm;
        }

        public async Task<IActionResult> OnGetAsync(string code)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return Forbid();
            }

            var quiz = await _repository.GetAsync<Quiz>(
                q => q.Code == code,
                query => query
                    .Include(q => q.Participants.Where(p => p.Id == appUser.Id))
                    .Include(q => q.Exercises)
                        .ThenInclude(q => q.ExerciseSolutions.Where(es => es.UserId == appUser.Id))
                    .Include(q => q.QuizResults.Where(qr => qr.UserId == appUser.Id))
            );

            if (quiz == null)
            {
                return RedirectToPage("Error");
            }

            var quizResult = quiz.QuizResults.FirstOrDefault(qr => qr.UserId == appUser.Id);
            var quizResultExists = quizResult != null && quizResult.Grade != null;
            var quizClosed = quiz.State == Enumerations.QuizState.Closed;
            var userIsParticipant = quiz.Participants.Any(p => p.Id == appUser.Id);
            if (quizResultExists || (quizClosed && userIsParticipant))
                return RedirectToPage("Browse", new { id = quiz.Id });

            if (quizClosed && !userIsParticipant)
                return Forbid();

            // TODO redirect to some quiz is not started page, adding this to solve page wil add one big if so i think new page is better
            var quizNotOpen = quiz.State != Enumerations.QuizState.Open;
            if (quizNotOpen)
            {
                // also here can check if quiz.State == notStarted (published already) and add user as participant
                // this way the quiz will be in Assigned planned section
                return RedirectToPage("Error");
            }

            Quiz = quiz;

            if (!userIsParticipant)
            {
                quiz.Participants.Add(appUser);
                _repository.Update(Quiz);
                await _repository.SaveChangesAsync();
            }

            Answers = Quiz.Exercises
                .Select(e => e.ExerciseSolutions.FirstOrDefault(es => es.UserId == appUser.Id)?.Answer ?? "")
                .ToList();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var appUser = await _userManager.GetUserAsync(User);

            if (appUser == null)
                return Forbid();

            var quiz = await _repository.GetAsync<Quiz>(
                q => q.Id == QuizId,
                query => query
                    .Include(q => q.QuizResults.Where(qr => qr.UserId == appUser.Id))
                    .Include(q => q.Exercises)
                        .ThenInclude(e => e.ExerciseSolutions.Where(es => es.UserId == appUser.Id))
                    .Include(q => q.Participants.Where(p => p.Id == appUser.Id)));

            if (quiz == null)
                return RedirectToPage("Error");

            var quizNotStarted = quiz.State == Enumerations.QuizState.NotStarted;
            var userIsParticipant = quiz.Participants.Any(p => p.Id == appUser.Id);
            if (quizNotStarted || !userIsParticipant)
                return Forbid();

            var quizResult = quiz.QuizResults.FirstOrDefault(qr => qr.UserId == appUser.Id);
            var quizIsGraded = quizResult != null && quizResult.Grade != null;
            var quizIsClosed = quiz.State == Enumerations.QuizState.Closed;
            if (quizIsGraded || quizIsClosed)
                RedirectToPage("Browse", new { id = quiz.Id });

            if (Answers.Count != quiz.Exercises.Count)
                return RedirectToPage("Error");

            var userSolutionsMap = quiz.Exercises
                .Where(e => e.ExerciseSolutions.Any(es => es.UserId == appUser.Id))
                .ToDictionary(
                    e => e.Id,
                    e => e.ExerciseSolutions.First(es => es.UserId == appUser.Id)
                );

            var exercises = quiz.Exercises.ToList();
            for (int i = 0; i < quiz.Exercises.Count; i++)
            {
                var exercise = exercises[i];
                var newAnswer = Answers[i] ?? "";

                // check if there is solution to exercise
                if (userSolutionsMap.TryGetValue(exercise.Id, out var existingSolution))
                {
                    // if solution answer is the same, dont do anything
                    if (existingSolution.Answer == newAnswer)
                        continue;

                    // if solution answer is different, delete current solution (this also deletes result)
                    _repository.Delete(existingSolution);
                }

                var newSolution = new ExerciseSolution
                {
                    ExerciseId = exercise.Id,
                    Answer = newAnswer,
                    UserId = appUser.Id,
                };

                _repository.Add(newSolution);

                var maxPointsForExercise = exercise.MaxPoints;
                var algorithmGrade = _algorithm.Grade(exercise.Question, newAnswer, maxPointsForExercise);
                var mistakes = algorithmGrade.Item3;
                var mistakeResults = GenerateMistakeResults(mistakes);

                var exerciseResult = new ExerciseResult
                {
                    Points = 0,
                    MaxPoints = maxPointsForExercise,
                    Comment = string.Empty,
                    AlgorithmPoints = algorithmGrade.Item1,

                    ExerciseSolution = newSolution,
                    MistakeResults = mistakeResults
                };

                _repository.Add(exerciseResult);
            }

            await _repository.SaveChangesAsync();
            return RedirectToPage("Assigned");
        }

        private static List<MistakeResult> GenerateMistakeResults(Dictionary<(int, (int, int, int)), List<int>> tmp)
        {
            var sortedKeys = tmp.Keys
                .OrderBy(key => key.Item1)
                    .ThenBy(key => key.Item2.Item1)
                        .ThenBy(key => key.Item2.Item2)
                            .ThenBy(key => key.Item2.Item3)
                .ToList();

            int lastBar = 0;
            List<MistakeResult> result = [];

            foreach (var key in sortedKeys)
            {
                MistakeResult mistakeResult = new();

                var bar = key.Item1;
                var function1 = key.Item2.Item1;
                var function2 = key.Item2.Item2;
                var bar2 = key.Item2.Item3;

                if (bar != lastBar)
                {
                    lastBar = bar;
                    mistakeResult.Bars = [bar];
                }

                if (function1 == function2)
                    mistakeResult.Functions = [function1];
                else
                {
                    if (bar2 != bar)
                    {
                        mistakeResult.Bars = [bar2];
                        mistakeResult.Functions = [function1, function2];
                    }
                    else
                        mistakeResult.Functions = [function1, function2];
                }

                foreach (var o in tmp[key])
                {
                    mistakeResult.MistakeCodes.Add(o);
                }

                result.Add(mistakeResult);
            }

            return result;
        }
    }
}
