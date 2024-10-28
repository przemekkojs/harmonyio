using System.ComponentModel.DataAnnotations;
using Humanizer;
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
    public class GroupDetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        [BindProperty]
        [Display(Name = "Group Name")]
        [Required(ErrorMessage = "Group name is required.")]
        public string GroupName { get; set; } = null!;
        
        
        [BindProperty]
        public List<string> Emails { get; set; } = [];
        [BindProperty]
        public int? GroupId { get; set; } = null;
        [BindProperty]
        public string? UserId { get; set; } = null;
        [BindProperty]
        public bool ForTeachers { get; set; }

        public List<ApplicationUser> Students { get; set; } = [];

        public List<ApplicationUser> Teachers { get; set; } = [];

        public List<GroupRequest> Requests { get; set; } = [];

        public int Id { get; set; } = 0;

        public GroupDetailsModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {

            var appUser = await _userManager.GetUserAsync(User);
            var group = await _repository.GetAsync<UsersGroup>(
                filter: q => q.Id == id,
                modifier: q => q
                    .Include(g => g.Requests)
                        .ThenInclude(r => r.User)
                    .Include(g => g.Students)
                    .Include(g => g.Teachers)
            );

            if (group == null || appUser == null || !group.Teachers.Any(t => t.Id == appUser.Id))
            {
                return Forbid();
            }

            Teachers = group.Teachers.ToList();

            Students = group.Students.ToList();

            Requests = group.Requests.ToList();

            GroupId = id;

            return Page();
        }

        public async Task<IActionResult> OnPostSendRequests()
        {            
            var currentUser = (await _userManager.GetUserAsync(User))!;
            if (currentUser == null)
            {
                return RedirectToPage("Error");
            }

            var requestUsers = new List<ApplicationUser>();

            var nonExistingMails = new List<string>();

            foreach(var email in Emails)
            {
                var user = await _repository.GetAsync<ApplicationUser>(u => u.Email == email);

                if (user == null)
                {
                    nonExistingMails.Add(email);
                }
                else
                {
                    requestUsers.Add(user);
                }
            }

            // if (nonExistingMails.Any())
            // {
            //     if (nonExistingMails.Count == 1)
            //     {
            //         ModelState.AddModelError(nameof(Emails), $"Nie istnieje użytkownik z mail-em: {nonExistingMails.First()}");
            //     }
            //     else
            //     {
            //         ModelState.AddModelError(nameof(Emails), $"Nie istnieją użytkownicy z mail-ami: {nonExistingMails.Humanize()}");
            //     }
            // }
            
            var group = await _repository.GetAsync<UsersGroup>(
                filter: q => q.Id == GroupId,
                modifier: q => q
                    .Include(g => g.Requests)
                    .Include(g => g.Teachers)
            );

            if (group == null)
            {
                return RedirectToPage("Error");
            }

            foreach (var user in requestUsers)
            {
                _repository.Add(new GroupRequest()
                {
                    Group = group,
                    User = user,
                    ForTeacher = ForTeachers,
                    ExpirationDate = DateTime.Now.AddDays(7),
                });
            }

            await _repository.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveFromGroup()
        {
            var currentUser = (await _userManager.GetUserAsync(User))!;
            if (currentUser == null)
            {
                return RedirectToPage("Error");
            }

            var userToRemove = await _repository.GetAsync<ApplicationUser>(
                u => u.Id == UserId,
                q => q
                    .Include(u => u.TeacherInGroups)
                    .Include(u => u.StudentInGroups)
            );

            if (userToRemove == null)
            {
                return RedirectToPage("Error");
            }

            var group = await _repository.GetAsync<UsersGroup>(
                g => g.Id == GroupId
            );

            if (group == null)
            {
                return RedirectToPage("Error");
            }

            if (ForTeachers)
            {
                group.Teachers.Remove(userToRemove);
            }
            else
            {
                group.Students.Remove(userToRemove);
            }

            _repository.Update(group);

            await _repository.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
