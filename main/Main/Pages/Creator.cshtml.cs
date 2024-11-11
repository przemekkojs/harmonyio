using System.ComponentModel.DataAnnotations;
using Algorithm.New.Algorithm.Checkers;
using Algorithm.New.Algorithm.Parsers.ProblemParser;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages
{
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

        // To tak funkcja, kt�r� trzeba lepiej okodowa�.
        private bool CheckProblem(string question)
        {
            // Tutaj bierzemy rozwi�zanie z kreatora i parsujemy
            var parsed = Parser.ParseJsonToProblem(question);

            // Mo�e wyj�� NULL, wi�c trzeba sprawdzi�
            if (parsed != null)
            {
                // A to jest lista b��d�w. Na sam pocz�tek wystarczy informowanie, czy b��dy sa, potem si�
                // doda jakie� wy�wietlanie, jakie to b��dy
                var mistakes = ProblemChecker.CheckProblem(parsed);

                // Co jak s� b��dy
                if (mistakes.Count != 0)
                {
                    // Tu si� przyda popup, �e b��dy w zadaniu
                    // TODO: �eby nie prze�adowa� strony - trzeba si� z AJAXem pobawi�
                    return false;
                }
                else
                {
                    // Jak nie ma b��d�w, to wsm nie ma co nic robi�.
                    return true;
                }
            }
            else
            {
                // TODO: Tutaj trzeba ogarn�� czemu wsm wyszed� null
                return true;
            }
        }

        // TODO: Ogarn��, �eby sprawdzarka dzia�a�a.
        // Je�eli kto� si� tym zajmuje, to w komentarzach jest tutorial
        public async Task<IActionResult> OnPostSave()
        {
            var currentUser = (await _userManager.GetUserAsync(User))!;

            if (currentUser == null)
                return RedirectToPage("Login");

            if (EditedQuizId == null)
            {
                var quiz = new Quiz()
                {
                    Name = QuizName,
                    CreatorId = currentUser.Id,
                };

                _repository.Add(quiz);

                await _repository.SaveChangesAsync();

                foreach (string question in Questions!)
                {
                    // Te 4 linijki trzeba ogarn��, �eby lepiej dzia�a�y.
                    var checkProblemResult = CheckProblem(question);

                    if (!checkProblemResult)
                        return Page();

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

                _repository.Update(editedQuiz);

                await _repository.SaveChangesAsync();

                foreach (var question in editedQuiz.Excersises)
                {
                    _repository.Delete(question);
                }

                if (Questions.Any(q => q == null))
                {
                    ModelState.AddModelError(nameof(Questions), "Nie mo�na doda� pustych zada�.");
                    return Page();
                }

                foreach (var question in Questions)
                {
                    // Te 4 linijki trzeba ogarn��, �eby lepiej dzia�a�y.
                    var checkProblemResult = CheckProblem(question);

                    if (!checkProblemResult)
                        return Page();

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
