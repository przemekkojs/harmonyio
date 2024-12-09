using Algorithm.New.Algorithm;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Algorithm.Mistake.Problem;
using Algorithm.New.Algorithm.Parsers.ProblemParser;
using Algorithm.New.Music;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Main.Pages
{
    public class QuizData
    {
        public int? EditedQuizId { get; set; }
        public List<string> Questions { get; set; }
        public string QuizName { get; set; }
    }

    public record GenerateData
    {
        public int Bars { get; set; }
        public string MetreValue { get; set; }
        public string MetreCount { get; set; }
        public int SharpsCount { get; set; }
        public int FlatsCount { get; set; }
        public int Minor { get; set; }
    }

    [Authorize]
    public class CreatorModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        [BindProperty]
        [Display(Name = "Nazwa quizu")]
        [Required(ErrorMessage = "Nazwa quizu jest wymagana")]
        public string QuizName { get; set; } = null!;

        [BindProperty]
        public List<string> Questions { get; set; } = null!;

        [BindProperty]
        public int? EditedQuizId { get; set; } = null;

        [BindProperty]
        public List<string> Maxes { get; set; } = null!;

        [BindProperty]
        public string? Code { get; set; }

        public bool BrowseOnly { get; set; } = false;

        public CreatorModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id, bool? triggerSubmit)
        {
            if (id == null)
                return Page();

            var appUser = await _userManager.GetUserAsync(User);

            var quiz = await _repository.GetAsync<Quiz>(
                filter: q => q.Id == id,
                modifier: q => q.Include(r => r.Exercises)
            );

            if (quiz == null || appUser == null || appUser.Id != quiz.CreatorId)
                return Forbid();

            BrowseOnly = quiz.IsCreated;
            EditedQuizId = quiz.Id;
            QuizName = quiz.Name;
            Code = quiz.Code;
            Questions = quiz.Exercises.Select(e => e.Question).ToList();

            if (triggerSubmit ?? false)
                return await OnPostSubmit();

            return Page();
        }

        private static List<ProblemMistake> GetProblemMistakes(string question)
        {
            var parsed = Parser.ParseJsonToProblem(question);

            if (parsed != null)
            {
                if (parsed.Functions.Count == 0)
                    return [];

                var mistakes = ProblemChecker.CheckProblem(parsed);
                return mistakes;
            }
            else
                throw new ArgumentException("Invalid question");
        }

        private static bool CheckProblem(string question)
        {
            try
            {
                var mistakes = GetProblemMistakes(question);
                var mistakesCount = mistakes.Count;

                return mistakesCount == 0;
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException)
            {
                return false;
            }
        }

        private bool ValidateEmptyExercises()
        {
            foreach (string question in Questions)
            {
                var questionEmpty = string.IsNullOrWhiteSpace(question);

                if (questionEmpty)
                    return false;

                // Tu może polecieć JsonException i ArgumentException, jak jakieś inne to coś nie tak, ale lepiej tak się zabezpieczyć
                // teoretycznie nie powinien, co potwierdzają testy, no ale znając życie, przegapiłem jakiś case...
                try
                {
                    var questionParsed = Parser.ParseJsonToProblem(question);
                    var functions = questionParsed.Functions;

                    if (functions == null)
                        return false;

                    var functionsEmpty = functions.Count == 0;

                    if (functionsEmpty)
                        return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return true;
        }

        private bool ValidateExcersises()
        {
            foreach (string question in Questions)
            {
                var valid = CheckProblem(question);

                if (!valid)
                    return false;
            }

            return true;
        }

        private string MistakesToHTML()
        {
            var result = string.Empty;
            var taskId = 1;

            foreach (string question in Questions)
            {
                var mistakes = GetProblemMistakes(question);
                var mistakeCount = 1;

                result += $"<details open><summary>Zadanie {taskId}</summary><p>";

                foreach (var mistake in mistakes)
                {
                    var mistakeExists = mistake.Rule != null;

                    var desc = mistakeExists ?
                        $"<span title=\"{mistake.Rule?.Description}\" style=\"cursor: pointer;\"><i>{mistake.Rule?.Name}</i></span> w takcie <b>{mistake.BarIndex + 1}</b>, funkcja <b>{mistake.FunctionIndex + 1}</b>" :
                        $"Błąd w takcie <b>{mistake.BarIndex + 1}</b>, funkcja <b>{mistake.FunctionIndex + 1}</b>";

                    result += $"{mistakeCount}. {desc}<br/>";
                    mistakeCount++;
                }

                result += "</p></details>";

                taskId++;
            }

            return result;
        }

        public JsonResult OnPostGenerateTask([FromBody] GenerateData data)
        {
            var mV = Convert.ToInt32(data.MetreValue);
            var mC = Convert.ToInt32(data.MetreCount);

            var generatedFunctions = Algorithm.New.Algorithm.Generators.ProblemGenerator
                .Generate(data.Bars, mV, mC, data.SharpsCount, data.FlatsCount, data.Minor);

            var metre = Metre.GetMetre(mC, mV);
            var tonationList = Tonation.GetTonation(data.SharpsCount, data.FlatsCount);

            var tonation = data.Minor == 1 ?
                tonationList.First(x => x.Mode == Mode.Major) :
                tonationList.First(x => x.Mode == Mode.Minor);

            var problem = new Problem(generatedFunctions, metre, tonation);
            var parsedProblem = Parser.ParseProblemFunctionsToString(problem);

            return new JsonResult(parsedProblem);
        }

        public async Task<IActionResult> OnPostSave()
        {
            // Fixes-2: Kurna, który ma utf-16 włączone? XD
            // Generalnie, z Creator.cshtml przychodzi lista jako JSON i się ładuje jako zerowy element listy,
            // wi�c te linijki jakby rozpakowuj� t� lit�.
            // Je�eli na froncie nie ma b��d�w, to to zawsze si� wykona poprawnie.
            Questions = JsonConvert.DeserializeObject<List<string>>(Questions[0])!;
            Maxes = JsonConvert.DeserializeObject<List<string>>(Maxes[0])!;

            var currentUser = await _userManager.GetUserAsync(User);

            var successResult = new { success = true, redirect = true, redirectUrl = Url.Page("Created") };
            var loginResult = new { success = false, redirect = true, redirectUrl = Url.Page("Login") };
            var errorResult = new { success = false, redirect = true, redirectUrl = Url.Page("Error") };
            var invalidQuestionsResult = new { success = false, redirect = false, errorMessage = "Quiz zawiera puste zadania." };

            if (currentUser == null)
                return new JsonResult(loginResult);

            Quiz? quiz;

            if (EditedQuizId == null)
            {
                quiz = new Quiz
                {
                    Name = QuizName,
                    CreatorId = currentUser.Id,
                    Code = await _repository.GenerateUniqueCodeAsync()
                };

                _repository.Add(quiz);
            }
            else
            {
                quiz = await _repository.GetAsync<Quiz>(
                    filter: q => q.Id == EditedQuizId,
                    modifier: q => q.Include(r => r.Exercises)
                );

                if (quiz == null)
                    return new JsonResult(errorResult);

                quiz.Name = QuizName;
                _repository.Update(quiz);

                foreach (var exercise in quiz.Exercises)
                {
                    _repository.Delete(exercise);
                }
            }

            if (!ValidateEmptyExercises())
                return new JsonResult(invalidQuestionsResult);

            quiz.IsValid = ValidateExcersises();

            if (EditedQuizId == null)
                await _repository.SaveChangesAsync();

            var quizId = quiz.Id;

            for (int index = 0; index < Questions.Count; index++)
            {
                var question = Questions[index];
                var points = Convert.ToInt32(Maxes[index]);

                _repository.Add(new Exercise
                {
                    Question = question,
                    QuizId = quizId,
                    MaxPoints = points
                });
            }

            await _repository.SaveChangesAsync();

            if (quiz.IsValid)
                return new JsonResult(successResult);
            else
            {
                var mistakesHTML = MistakesToHTML();
                var warningResult = new { success = true, display = mistakesHTML };
                return new JsonResult(warningResult);
            }
        }

        public async Task<IActionResult> OnPostSubmit()
        {
            if (Questions!.Count == 0)
                ModelState.AddModelError(nameof(Questions), "Wymagane jest przynajmniej jedno zadanie.");
            else if (Questions.Any(q => q == null || q.Equals(string.Empty)))
                ModelState.AddModelError(nameof(Questions), "Nie można dodaż pustych zadań.");

            if (!ModelState.IsValid)
                return Page();

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
                return RedirectToPage("Error");

            if (EditedQuizId == null)
            {
                var quiz = new Quiz()
                {
                    Name = QuizName,
                    CreatorId = currentUser.Id,
                    IsCreated = true,
                    Code = await _repository.GenerateUniqueCodeAsync(),
                };

                _repository.Add(quiz);

                await _repository.SaveChangesAsync();

                foreach (string question in Questions!)
                {
                    _repository.Add(new Exercise()
                    {
                        Question = question,
                        QuizId = quiz.Id,
                    });
                }
            }
            else
            {
                var editedQuiz = await _repository.GetAsync<Quiz>(
                    filter: q => q.Id == EditedQuizId,
                    modifier: q => q.Include(r => r.Exercises)
                );

                if (editedQuiz == null)
                    return RedirectToPage("Error");

                editedQuiz.Name = QuizName;
                editedQuiz.IsCreated = true;

                _repository.Update(editedQuiz);

                await _repository.SaveChangesAsync();

                foreach (var question in editedQuiz.Exercises)
                {
                    _repository.Delete(question);
                }

                foreach (var question in Questions)
                {
                    _repository.Add(new Exercise()
                    {
                        Question = question,
                        QuizId = editedQuiz.Id,
                    });
                }
            }

            await _repository.SaveChangesAsync();

            return RedirectToPage("Created");
        }
    }
}
