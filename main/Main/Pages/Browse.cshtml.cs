using Algorithm.New.Algorithm.Generators;
using Main.Data;
using Main.Enumerations;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MidiPlayback;

namespace Main.Pages
{
    // easy access to result data
    public record ExerciseResultData
    {
        public int Points { get; set; }
        public int MaxPoints { get; set; } = 10; // Default to 10 if no data
        public string Comment { get; set; } = string.Empty;
        public string Opinion { get; set; } = string.Empty;
    }

    public record PlayExerciseData
    {
        public int SolutionIndex { get; set; }
    }

    [Authorize]
    public class BrowseModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        public List<string> AlgorithmSolutions { get; private set; } = [];
        public string Opinion { get; private set; } = "Brak opinii";
        public string GradeString { get; set; } = string.Empty;
        public Quiz Quiz { get; set; } = null!;
        public List<string> Questions = [];
        public List<string> Answers = [];
        public List<ExerciseResultData> ExerciseResults = [];

        private const string EMPTY_GRADE = "-";

        public BrowseModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        // TODO: Implementacja
        public JsonResult OnPostPlayFile([FromBody] PlayExerciseData data)
        {
            var solutionIndex = data.SolutionIndex;

            if (AlgorithmSolutions.Count == 0)
                return new JsonResult(new { success = false });

            var solutionString = AlgorithmSolutions[solutionIndex];
            var solutionParseResult = Algorithm.New.Algorithm.Parsers.SolutionParser.Parser
                .ParseJsonToSolutionParseResult(solutionString);

            var stacks = solutionParseResult.Stacks;

            var result = FileCreator.Create();

            return new JsonResult(new { success = true, bytes = result });
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
                            .ThenInclude(es => es.ExerciseResult!)
                                .ThenInclude(er => er.MistakeResults)
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
                GradeString = EMPTY_GRADE;
            }
            else
                GradeString = ((Grade)quizResult.Grade).AsString();

            Quiz = quiz;
            Questions = quiz.Exercises.Select(e => e.Question).ToList();
            Answers = quiz.Exercises
                .Select(e => e.ExerciseSolutions.FirstOrDefault(es => es.UserId == appUser.Id)?.Answer ?? string.Empty)
                .ToList();            

            var showOpinion = quiz.ShowAlgorithmOpinion;

            if (showOpinion && AlgorithmSolutions.Count == 0)
                GenerateExampleSolutions();

            ExerciseResults = quiz.Exercises
                .Select(e => new
                {
                    e.ExerciseSolutions.FirstOrDefault(es => es.UserId == appUser.Id)?.ExerciseResult,
                    e.MaxPoints
                })
                .Select(x => x.ExerciseResult == null ?
                    new ExerciseResultData
                    {
                        Points = 0,
                        MaxPoints = x.MaxPoints,
                        Comment = string.Empty,
                        Opinion = string.Empty
                    } :
                    new ExerciseResultData
                    {
                        Points = x.ExerciseResult.Points,
                        MaxPoints = x.MaxPoints,
                        Comment = x.ExerciseResult.Comment,
                        Opinion = showOpinion ? Utils.Utils.MistakesToHTML(x.ExerciseResult.MistakeResults) : string.Empty
                    })                
                .ToList();

            return Page();
        }

        private void GenerateExampleSolutions()
        {
            foreach (var question in Questions)
            {
                try
                {
                    var problem = Algorithm.New.Algorithm.Parsers.ProblemParser.Parser
                        .ParseJsonToProblem(question);

                    var solution = SolutionGenerator.GenerateLinear(problem);
                    var result = Algorithm.New.Algorithm.Parsers.SolutionParser.Parser
                        .ParseSolutionToJson(solution);

                    AlgorithmSolutions.Add(result);
                }
                catch (Exception)
                {
                    var result = string.Empty;
                    AlgorithmSolutions.Add(result);
                    continue;
                }
            }
        }
    }
}
