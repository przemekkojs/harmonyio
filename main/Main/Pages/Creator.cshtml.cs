using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Algorithm.Parsers.ProblemParser;
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
        public List<int> Maxes { get; set; } = null!;

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
            {
                return Page();
            }

            var appUser = await _userManager.GetUserAsync(User);

            var quiz = await _repository.GetAsync<Quiz>(
                filter: q => q.Id == id,
                modifier: q => q.Include(r => r.Excersises)
            );

            if (quiz == null || appUser == null || appUser.Id != quiz.CreatorId)
            {
                return Forbid();
            }

            BrowseOnly = quiz.IsCreated;

            EditedQuizId = quiz.Id;
            QuizName = quiz.Name;
            Code = quiz.Code;
            Questions = quiz.Excersises.Select(e => e.Question).ToList();

            if (triggerSubmit ?? false)
            {
                return await OnPostSubmit();
            }

            return Page();
        }

        // To tak funkcja, któr¹ trzeba lepiej okodowaæ.
        private bool CheckProblem(string question)
        {
            // Tutaj bierzemy rozwi¹zanie z kreatora i parsujemy
            try
            {
                var parsed = Parser.ParseJsonToProblem(question);

                // Mo¿e wyjœæ NULL, wiêc trzeba sprawdziæ
                if (parsed != null)
                {
                    if (parsed.Functions.Count == 0)
                    {
                        return false;
                    }

                    // A to jest lista b³êdów. Na sam pocz¹tek wystarczy informowanie, czy b³êdy sa, potem siê
                    // doda jakieœ wyœwietlanie, jakie to b³êdy
                    var mistakes = ProblemChecker.CheckProblem(parsed);

                    // Co jak s¹ b³êdy
                    if (mistakes.Count != 0)
                    {
                        // Tu siê przyda popup, ¿e b³êdy w zadaniu
                        // TODO: ¯eby nie prze³adowaæ strony - trzeba siê z AJAXem pobawiæ
                        return false;
                    }
                    else
                    {
                        // Jak nie ma b³êdów, to wsm nie ma co nic robiæ.
                        return true;
                    }
                }
                else
                {
                    // TODO: Tutaj trzeba ogarn¹æ czemu wsm wyszed³ null
                    return true;
                }
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is InvalidOperationException)
            {
                return false;
            }
        }

        private bool ValidateAndAddExercises()
        {
            foreach (string question in Questions)
            {
                var questionEmpty = string.IsNullOrWhiteSpace(question);
                var questionInvalid = CheckProblem(question);

                if (questionEmpty || !questionInvalid)
                    return false;                
            }

            return true;
        }

        // TODO: Ogarn¹æ, ¿eby sprawdzarka dzia³a³a.
        // Je¿eli ktoœ siê tym zajmuje, to w komentarzach jest tutorial
        public async Task<IActionResult> OnPostSave()
        {
            Questions = JsonConvert.DeserializeObject<List<string>>(Questions[0]);

            var currentUser = await _userManager.GetUserAsync(User);

            var successResult = new { success = true, redirect = true, redirectUrl = Url.Page("Created") };
            var loginResult = new { success = false, redirect = true, redirectUrl = Url.Page("Login") };
            var errorResult = new { success = false, redirect = true, redirectUrl = Url.Page("Error") };
            var invalidQuestionsResult = new { success = false, redirect = false, errorMessage = "Quiz zawiera b³êdne zadania." };

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
                    modifier: q => q.Include(r => r.Excersises)
                );

                if (quiz == null)
                    return new JsonResult(errorResult);

                quiz.Name = QuizName;
                _repository.Update(quiz);

                foreach (var exercise in quiz.Excersises)
                {
                    _repository.Delete(exercise);
                }
            }

            if (!ValidateAndAddExercises())
                return new JsonResult(invalidQuestionsResult);

            if (EditedQuizId == null)
                await _repository.SaveChangesAsync();

            var quizId = quiz.Id;

            foreach (var question in Questions)
            {
                _repository.Add(new Excersise
                {
                    Question = question,
                    QuizId = quizId,
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
                ModelState.AddModelError(nameof(Questions), "Nie mo¿na dodaæ pustych zadañ.");
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
                    _repository.Add(new Excersise()
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
                    modifier: q => q.Include(r => r.Excersises)
                );

                if (editedQuiz == null)
                    return RedirectToPage("Error");

                editedQuiz.Name = QuizName;
                editedQuiz.IsCreated = true;

                _repository.Update(editedQuiz);

                await _repository.SaveChangesAsync();

                foreach (var question in editedQuiz.Excersises)
                {
                    _repository.Delete(question);
                }

                foreach (var question in Questions)
                {
                    _repository.Add(new Excersise()
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
