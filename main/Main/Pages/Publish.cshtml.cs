using System.ComponentModel.DataAnnotations;
using Main.Data;
using Main.Enumerations;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages
{
    [Authorize]
    public class PublishModel : PageModel
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
        public DateTime? OpenDate { get; set; } = DateTime.Now;
        [BindProperty]
        [Display(Name = "Close Date")]
        [Required(ErrorMessage = "Close date is required.")]
        public DateTime? CloseDate { get; set; } = DateTime.Now;

        [BindProperty]
        public int QuizId { get; set; }

        public PublishModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            
            var appUser = await _userManager.GetUserAsync(User);
            var quiz = await _repository.GetAsync<Quiz>(
                filter: q => q.Id == id,
                modifier: q => q.Include(r => r.Excersises)
            );

            if (quiz == null || appUser == null || appUser.Id != quiz.CreatorId)
            {
                return Forbid();
            }

            QuizName = quiz.Name;
            QuizId = id;

            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            if (CloseDate <= OpenDate)
            {
                ModelState.AddModelError(nameof(CloseDate), "Close date can't be older than open date.");;
            }
            if (CloseDate <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(CloseDate), "Close date can't be older than current date."); ;
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToPage("Error");
            }
        
            var quiz = await _repository.GetAsync<Quiz>(
                filter: q => q.Id == QuizId,
                modifier: q => q.Include(r => r.Excersises)
            );

            if (quiz == null)
            {
                return RedirectToPage("Error");
            }

            quiz.Name = QuizName;
            quiz.OpenDate = (DateTime)OpenDate!;
            quiz.CloseDate = (DateTime)CloseDate!;
            quiz.IsCreated = true;
            
            _repository.Update(quiz);

            //creating new copy in sketches

            var newSketch = new Quiz(){
                Creator = quiz.Creator,
                Name = quiz.Name,
            };

            _repository.Add(newSketch);

            foreach (var e in quiz.Excersises)
            {
                _repository.Add(new Excersise(){
                    Quiz = newSketch,
                    Question = e.Question
                });
            }
            
            await _repository.SaveChangesAsync();

            return RedirectToPage("Created");
        }

        public async Task<IActionResult> OnPostSubmit()
        {           
    
            if (CloseDate <= OpenDate)
            {
                ModelState.AddModelError(nameof(CloseDate), "Close date can't be older than open date.");;
            }
            if (CloseDate <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(CloseDate), "Close date can't be older than current date."); ;
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return RedirectToPage("Error");

        
            var quiz = await _repository.GetAsync<Quiz>(
                filter: q => q.Id == QuizId,
                modifier: q => q.Include(r => r.Excersises)
            );

            if (quiz == null)
            {
                return RedirectToPage("Error");
            }
            
            quiz.Name = QuizName;
            quiz.OpenDate = (DateTime)OpenDate!;
            quiz.CloseDate = (DateTime)CloseDate!;
            quiz.IsCreated = true;

            
            _repository.Update(quiz);
            
            await _repository.SaveChangesAsync();

            return RedirectToPage("Created");
		}
    }
}
