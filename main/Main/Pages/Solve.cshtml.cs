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
            // quiz result exists or quiz is closed and user is participant
            if (
                (quizResult != null && quizResult.Grade != null) ||
                (
                    quiz.State == Enumerations.QuizState.Closed &&
                    Quiz.Participants.Any())
                )
            {
                return RedirectToPage("Browse", new { id = quiz.Id });
            }

            // quiz is closed and user isnt participant
            if (quiz.State == Enumerations.QuizState.Closed && !Quiz.Participants.Any())
            {
                return Forbid();
            }

            // TODO redirect to some quiz is not started page, adding this to solve page wil add one big if so i think new page is better
            if (quiz.State != Enumerations.QuizState.Open)
            {
                // also here can check if quiz.State == notStarted (published already) and add user as participant
                // this way the quiz will be in Assigned planned section
                return RedirectToPage("Error");
            }

            Quiz = quiz;

            if (!Quiz.Participants.Any())
            {
                Quiz.Participants.Add(appUser);
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
            {
                return Forbid();
            }

            var quiz = await _repository.GetAsync<Quiz>(
                q => q.Id == QuizId,
                query => query
                    .Include(q => q.QuizResults
                        .Where(qr => qr.UserId == appUser.Id))
                    .Include(q => q.Excersises)
                    .ThenInclude(e => e.ExcersiseSolutions
                        .Where(es => es.UserId == appUser.Id))
                    .Include(q => q.Participants.Where(p => p.Id == appUser.Id))
            );

            if (quiz == null)
            {
                return RedirectToPage("Error");
            }

            var quizResult = quiz.QuizResults.FirstOrDefault();

            // quiz isnt open or user isnt participant
            if (quiz.State != Enumerations.QuizState.Open || !quiz.Participants.Any())
            {
                return Forbid();
            }

            // quiz is graded so show the result
            if (quizResult != null && quizResult.Grade != null)
            {
                RedirectToPage("Browse", new { id = quiz.Id });
            }

            if (Answers.Count != quiz.Excersises.Count)
            {
                return RedirectToPage("Error");
            }

            bool hasChangedAnswer = false;

            var excersises = quiz.Excersises.ToList();
            var userSolutionsMap = quiz.Excersises
                .SelectMany(e => e.ExcersiseSolutions)
                .ToDictionary(es => es.ExcersiseId);

            for (int i = 0; i < quiz.Excersises.Count; i++)
            {
                var exercise = excersises[i];
                var newAnswer = Answers[i] ?? "";

                // case when for example there is two excersises, user just does first and saves
                // then answer returned from second excersise is empty, as if there was no solution to it
                // if there was previous solution, it will stay the same
                if (newAnswer == "")
                {
                    continue;
                }

                // check if there is solution to excersise
                if (userSolutionsMap.TryGetValue(exercise.Id, out var existingSolution))
                {
                    // if solution answer is the same, dont do anything
                    if (existingSolution.Answer == newAnswer)
                    {
                        continue;
                    }
                    hasChangedAnswer = true;
                    // if solution answer is different, delete current solution (this also deletes result)
                    _repository.Delete(existingSolution);
                }
                else
                {
                    hasChangedAnswer = true;
                }

                // add new solution if there was no solution or current answer is different
                var newSolution = new ExcersiseSolution
                {
                    ExcersiseId = exercise.Id,
                    Answer = newAnswer,
                    UserId = appUser.Id
                };
                _repository.Add(newSolution);

                var algorithmGrade = _algorithm.Grade(exercise.Question, newAnswer);
                var excersiseResult = new ExcersiseResult
                {
                    Comment = "",
                    Points = algorithmGrade.Item1, // Set initial points based on the algorithm's grade
                    AlgorithmPoints = algorithmGrade.Item1,
                    MaxPoints = exercise.MaxPoints,
                    AlgorithmOpinion = algorithmGrade.Item3,
                    ExcersiseSolution = newSolution, // Associate with the solution
                };

                _repository.Add(excersiseResult);
            }

            // answers was changed so set quiz result grade to null, it needs new grade
            // setting to null instead of deleting will keep exising excersise results
            if (hasChangedAnswer)
            {
                foreach (var result in quiz.QuizResults)
                {
                    result.Grade = null;
                    _repository.Update(result);
                }
                await _repository.SaveChangesAsync();
            }

            return RedirectToPage("Assigned");
        }
    }
}
