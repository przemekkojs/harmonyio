using Main.Data;
using Main.GradingAlgorithm;
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
                    .Include(q => q.Excersises)
                        .ThenInclude(q => q.ExcersiseSolutions
                            .Where(es => es.UserId == appUser.Id))
                    .Include(q => q.QuizResults.Where(qr => qr.UserId == appUser.Id))
            );

            if (quiz == null)
            {
                return RedirectToPage("Error");
            }

            var quizResult = quiz.QuizResults.FirstOrDefault();
            var quizResultExists = quizResult != null && quizResult.Grade != null;
            var quizClosed = quiz.State == Enumerations.QuizState.Closed;
            var quizNotOpen = quiz.State != Enumerations.QuizState.Open;
            var quizHasUsers = quiz.Participants != null && quiz.Participants.Count != 0;

            // quiz result exists or quiz is closed and user is participant
            if ((quizResultExists) || (quizClosed && quizHasUsers))
                return RedirectToPage("Browse", new { id = quiz.Id });

            // quiz is closed and user isnt participant
            if (quizClosed && !quizHasUsers)
                return Forbid();

            // TODO redirect to some quiz is not started page, adding this to solve page wil add one big if so i think new page is better
            if (quizNotOpen)
            {
                // also here can check if quiz.State == notStarted (published already) and add user as participant
                // this way the quiz will be in Assigned planned section
                return RedirectToPage("Error");
            }

            Quiz = quiz;

            if (!quizHasUsers)
            {
                quiz.Participants.Add(appUser);
                _repository.Update(Quiz);
                await _repository.SaveChangesAsync();
            }

            Answers = Quiz.Excersises
                .Select(e => e.ExcersiseSolutions.FirstOrDefault()?.Answer ?? "")
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
                    .Include(q => q.QuizResults
                        .Where(qr => qr.UserId == appUser.Id))
                    .Include(q => q.Excersises)
                    .ThenInclude(e => e.ExcersiseSolutions
                        .Where(es => es.UserId == appUser.Id))
                    .Include(q => q.Participants.Where(p => p.Id == appUser.Id)));

            if (quiz == null)
                return RedirectToPage("Error");

            var quizResult = quiz.QuizResults.FirstOrDefault();

            // quiz isnt open or user isnt participant
            var quizNotOpen = quiz.State != Enumerations.QuizState.Open;
            var quizHasParticipants = quiz.Participants.Count != 0;

            if (quizNotOpen || !quizHasParticipants)
                return Forbid();

            // quiz is graded so show the result
            if (quizResult != null && quizResult.Grade != null)
                RedirectToPage("Browse", new { id = quiz.Id });

            if (Answers.Count != quiz.Excersises.Count)
                return RedirectToPage("Error");

            var excersises = quiz.Excersises.ToList();
            var userSolutionsMap = quiz.Excersises
                .SelectMany(e => e.ExcersiseSolutions)
                .ToDictionary(es => es.ExcersiseId);

            for (int i = 0; i < quiz.Excersises.Count; i++)
            {
                var exercise = excersises[i];
                var newAnswer = Answers[i] ?? "";

                // check if there is solution to excersise
                if (userSolutionsMap.TryGetValue(exercise.Id, out var existingSolution))
                {
                    // if solution answer is the same, dont do anything
                    if (existingSolution.Answer == newAnswer)
                        continue;

                    // if solution answer is different, delete current solution (this also deletes result)
                    _repository.Delete(existingSolution);
                }

                var newSolution = new ExcersiseSolution
                {
                    ExcersiseId = exercise.Id,
                    Answer = newAnswer,
                    UserId = appUser.Id
                };

                _repository.Add(newSolution);

                var maxPointsForExcersise = exercise.MaxPoints;                
                var algorithmGrade = _algorithm.Grade(exercise.Question, newAnswer, maxPointsForExcersise);
                var mistakes = algorithmGrade.Item3;
                var mistakeResults = GenerateMistakeResults(mistakes);

                var excersiseResult = new ExcersiseResult
                {
                    Comment = string.Empty,
                    Points = 0,
                    AlgorithmPoints = algorithmGrade.Item1,
                    MaxPoints = maxPointsForExcersise,
                    ExcersiseSolution = newSolution,
                    MistakeResults = mistakeResults
                };

                _repository.Add(excersiseResult);
                await _repository.SaveChangesAsync();
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
