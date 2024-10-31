using System.ComponentModel.DataAnnotations;
using Main.Data;
using Main.Enumerations;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Main.Pages
{
    [Authorize]
    public class CreatorModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        [BindProperty]
        [Display(Name = "Quiz Name")]
        [Required(ErrorMessage = "Quiz name is required.")]
        public string QuizName { get; set; } = null!;
        [BindProperty]
        [Display(Name = "Open Date")]
        [Required(ErrorMessage = "Open date is required.")]
        public DateTime? OpenDate { get; set; } = null;
        [BindProperty]
        [Display(Name = "Close Date")]
        [Required(ErrorMessage = "Close date is required.")]
        public DateTime? CloseDate { get; set; } = null;
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
            //create new quiz
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
            CloseDate = quiz.CloseDate;
            OpenDate = quiz.OpenDate;
            Code = quiz.Code;
            Questions = quiz.Excersises.Select(e => e.Question).ToList();

            if (triggerSubmit ?? false)
            {
                return await OnPostSubmit();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            var currentUser = (await _userManager.GetUserAsync(User))!;
            if (currentUser == null)
                return RedirectToPage("Error");

            if (EditedQuizId == null)
            {
                var quiz = new Quiz()
                {
                    Name = QuizName,
                    OpenDate = (DateTime)OpenDate!,
                    CloseDate = (DateTime)CloseDate!,
                    CreatorId = currentUser.Id,
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
                editedQuiz.OpenDate = (DateTime)OpenDate!;
                editedQuiz.CloseDate = (DateTime)CloseDate!;

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

        public async Task<IActionResult> OnPostSubmit()
        {
            if (CloseDate <= OpenDate)
            {
                ModelState.AddModelError(nameof(CloseDate), "Close date can't be older than open date.");
            }
            if (CloseDate <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(CloseDate), "Close date can't be older than current date.");
            }

            if (Questions!.Count == 0)
            {
                ModelState.AddModelError(nameof(Questions), "At least one excersise is required.");
            }
            else if (Questions.Any(q => (q == "" || q == null)))
            {
                ModelState.AddModelError(nameof(Questions), "No excersise can be empty.");
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
                    OpenDate = (DateTime)OpenDate!,
                    CloseDate = (DateTime)CloseDate!,
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
                editedQuiz.OpenDate = (DateTime)OpenDate!;
                editedQuiz.CloseDate = (DateTime)CloseDate!;
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
