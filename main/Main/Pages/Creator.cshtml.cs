using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Algorithm.New.Algorithm.Checkers;
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

namespace Main.Pages
{
    public class QuizData
    {
        public int? EditedQuizId { get; set; }
        public List<string> Questions { get; set; }
        public string QuizName { get; set; }
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

        private static bool CheckProblem(string question)
        {
            try
            {
                var parsed = Parser.ParseJsonToProblem(question);

                if (parsed != null)
                {
                    if (parsed.Functions.Count == 0)
                        return false;

                    var mistakes = ProblemChecker.CheckProblem(parsed);
                    return mistakes.Count == 0;
                }
                else
                    return false;
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

        public JsonResult OnPostGenerateTask(int bars, int metreValue, int metreCount, int sharpsCount, int flatsCount, int minor)
        {
            var generatedFunctions = Algorithm.New.Algorithm.Generators.ProblemGenerator
                .Generate(bars, metreValue, metreCount, sharpsCount, flatsCount, minor);

            return new JsonResult(generatedFunctions);
        }

        public async Task<IActionResult> OnPostSave()
        {
            // Generalnie, z Creator.cshtml przychodzi lista jako JSON i si� �aduje jako zerowy element listy,
            // wi�c te linijki jakby rozpakowuj� t� lit�.
            // Je�eli na froncie nie ma b��d�w, to to zawsze si� wykona poprawnie.
            Questions = JsonConvert.DeserializeObject<List<string>>(Questions[0])!;
            Maxes = JsonConvert.DeserializeObject<List<string>>(Maxes[0])!;

            var currentUser = await _userManager.GetUserAsync(User);

            var successResult = new { success = true, redirect = true, redirectUrl = Url.Page("Created") };
            var loginResult = new { success = false, redirect = true, redirectUrl = Url.Page("Login") };
            var errorResult = new { success = false, redirect = true, redirectUrl = Url.Page("Error") };
            var invalidQuestionsResult = new { success = false, redirect = false, errorMessage = "Quiz zawiera b��dne zadania." };

            if (currentUser == null)
                return new JsonResult(loginResult);

            Quiz? quiz;

            if (EditedQuizId == null)
            {
                quiz = new Quiz
                {
                    Name = QuizName,
                    CreatorId = currentUser.Id
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

            quiz.IsValid = true;

            if (!ValidateExcersises())
                quiz.IsValid = false;

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

            return new JsonResult(successResult);
        }

        public async Task<IActionResult> OnPostSubmit()
        {
            if (Questions!.Count == 0)
            {
                ModelState.AddModelError(nameof(Questions), "Wymagane jest przynajmniej jedno zadanie.");
            }
            else if (Questions.Any(q => q == "" || q == null))
            {
                ModelState.AddModelError(nameof(Questions), "Nie mo�na doda� pustych zada�.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

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

                    //TODO: TESTING PURPOSES ONLY, REMOVE THIS
                    Participants = new List<ApplicationUser>() { currentUser },
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
