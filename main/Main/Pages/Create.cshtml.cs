using System.ComponentModel.DataAnnotations;
using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public CreateModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public void OnGet()
        {
              
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

            if (Questions == null || Questions.Count == 0)
            {
                ModelState.AddModelError(nameof(Questions), "At least one excersise is required.");
            }
            else if (Questions.Any(q => q == null || q == ""))
            {
                ModelState.AddModelError(nameof(Questions), "No excersise can be empty.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            //TODO: POPUALTE WITH REAL USER
            var currentUser = await GetTestUser();

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
            await _repository.SaveChangesAsync();

            return RedirectToPage("Index");
        }

        public async Task<ApplicationUser> GetTestUser()
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
