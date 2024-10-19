using System.ComponentModel.DataAnnotations;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages
{
    public class CreateModel : PageModel
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

        public CreateModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //create new quiz
            if (id == null)
            {
                return Page();
            }

            var quiz = await _repository.GetAsync<Quiz>(q => q.Id == id,
                q => q.Include(r => r.Excersises)
            );

            if (quiz == null)
            {
                return Forbid();
            }

            var appUser = (await _userManager.GetUserAsync(User))!;
            if (appUser.Id != quiz.CreatorId)
            {
                return Forbid();
            }

            EditedQuizId = quiz.Id;
            QuizName = quiz.Name;
            CloseDate = quiz.CloseDate;
            OpenDate = quiz.OpenDate;
            Questions = quiz.Excersises.Select(e => e.Question).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (CloseDate <= OpenDate)
            {
                ModelState.AddModelError(nameof(CloseDate), "Close date can't be older than open date.");;
            }
            if (CloseDate <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(CloseDate), "Close date can't be older than current date."); ;
            }

            if (Questions.Count == 0)
            {
                ModelState.AddModelError(nameof(Questions), "At least one excersise is required.");
            }
            else if (Questions.Any(q => q == ""))
            {
                ModelState.AddModelError(nameof(Questions), "No excersise can be empty.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var currentUser = (await _userManager.GetUserAsync(User))!;

            if (EditedQuizId == null)
            {
                var quiz = new Quiz()
                {
                    Name = QuizName,
                    OpenDate = (DateTime)OpenDate!,
                    CloseDate = (DateTime)CloseDate!,
                    CreatorId = currentUser.Id
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
                var editedQuiz = (await _repository.GetAsync<Quiz>(
                    filter: q => q.Id == EditedQuizId,
                    modifier: q => q.Include(r => r.Excersises)
                ))!;

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

            return RedirectToPage("Index");
        }


        private async Task<ApplicationUser> GetTestUser()
        {
            var userId = "testUser";
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                return user;
            }
            else
            {
                user = new ApplicationUser
                {
                    Id = userId,
                    UserName = userId,
                    FirstName = "Test",
                    LastName = "User"
                };

                var result = await _userManager.CreateAsync(user, "Test123!");
                return (await _userManager.FindByIdAsync(userId))!;
            }
        }
    }
}
