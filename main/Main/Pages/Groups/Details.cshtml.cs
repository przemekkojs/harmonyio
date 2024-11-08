using Main.Data;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace Main.Pages
{
    [Authorize]
    public class GroupDetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        public GroupDetailsModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }
        public bool IsAdmin { get; set; }

        public bool IsMaster { get; set; }

        public UsersGroup Group { get; set; } = null!;

        public ApplicationUser GroupMaster { get; set; } = null!;

        [BindProperty]
        public string UserId { get; set; } = "";

        [BindProperty]
        public bool RemoveFromStudents { get; set; }

        [BindProperty]
        public int GroupId { get; set; }

        [BindProperty]
        public bool AsAdmins { get; set; }

        [BindProperty]
        public string EmailsAsString { get; set; } = "";

        public async Task<IActionResult> OnGet(int id)
        {
            var appUser = await _userManager.GetUserAsync(User);

            if (appUser == null)
            {
                return Forbid();
            }

            var group = await _repository.GetAsync<UsersGroup>(
                q => q.Id == id,
                query => query
                    .Include(g => g.Teachers)
                    .Include(g => g.Students)
                    .Include(g => g.MasterUser)
                    .Include(g => g.Quizzes)
                        .ThenInclude(q => q.Excersises)
                    .Include(g => g.Quizzes)
                        .ThenInclude(q => q.Creator)
                    .Include(g => g.Requests)
                        .ThenInclude(r => r.User)
                );

            if (group == null)
            {
                return RedirectToPage("/Error");
            }

            Group = group;

            GroupMaster = group.MasterUser;

            IsMaster = GroupMaster.Id == appUser.Id;

            IsAdmin = IsMaster || group.Teachers.Contains(appUser);

            if (!IsAdmin && !group.Students.Contains(appUser))
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteUser()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return Forbid();
            }

            var group = await _repository.GetAsync<UsersGroup>(
                q => q.Id == GroupId,
                q => q
                    .Include(g => g.Teachers)
                    .Include(g => g.Students)
            );
            if (group == null)
            {
                return RedirectToPage("/Error");
            }

            var success = false;
            if (RemoveFromStudents)
            {
                success = HandleRemoveFromStudents(appUser, group);
            }
            else
            {
                success = HandleRemoveFromTeachers(appUser, group);
            }

            if (success)
            {
                _repository.Update(group);
                await _repository.SaveChangesAsync();
                return new JsonResult(new { success = true });
            }
            else
            {
                return RedirectToPage("/Error");
            }
        }

        private bool HandleRemoveFromStudents(ApplicationUser appUser, UsersGroup group)
        {
            // Allow only Master or Teacher to remove students
            if (group.MasterId != appUser.Id && !group.Teachers.Any(t => t.Id == appUser.Id))
            {
                return false;
            }

            // Find and remove the student
            var student = group.Students.FirstOrDefault(u => u.Id == UserId);
            if (student == null)
            {
                return false;
            }

            group.Students.Remove(student);
            return true;
        }

        private bool HandleRemoveFromTeachers(ApplicationUser appUser, UsersGroup group)
        {
            // Only Master can remove a teacher
            if (group.MasterId != appUser.Id)
            {
                return false;
            }

            // Find and remove the teacher
            var teacher = group.Teachers.FirstOrDefault(u => u.Id == UserId);
            if (teacher == null)
            {
                return false;
            }

            group.Teachers.Remove(teacher);
            return true;
        }

        public async Task<IActionResult> OnPostAddUsers()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return Forbid();
            }

            var group = await _repository.GetAsync<UsersGroup>(
                q => q.Id == GroupId,
                q => q
                    .Include(g => g.Teachers)
                    .Include(g => g.Students)
            );

            if (group == null ||
                (
                    group.MasterId != appUser.Id &&
                    !group.Teachers.Any(t => t.Id == appUser.Id)
                ))
            {
                return RedirectToPage("/Error");
            }

            if (string.IsNullOrEmpty(EmailsAsString))
            {
                return RedirectToPage("/Error");
            }

            var emails = EmailsAsString
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .ToHashSet();

            var foundUsers = await _repository.GetAllAsync<ApplicationUser>(
                q => q.Where(u => u.Email != null && emails.Contains(u.Email))
            );

            var usersByEmail = foundUsers.ToDictionary(u => u.Email!, u => u);
            var notFoundMails = emails
                    .Where(email => !usersByEmail.ContainsKey(email))
                    .ToList();

            if (notFoundMails.Count() != 0)
            {
                return new JsonResult(new { notFoundEmails = notFoundMails });
            }

            foreach (var user in foundUsers)
            {
                _repository.Add(new GroupRequest()
                {
                    Group = group,
                    ForTeacher = AsAdmins,
                    User = user,
                    ExpirationDate = DateTime.Now.AddDays(7)
                });
            }

            await _repository.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public IActionResult OnPostRedirectToIndexOwned()
        {
            TempData["showJoined"] = false;
            return RedirectToPage("Index");
        }

        public IActionResult OnPostRedirectToIndexJoined()
        {
            TempData["showJoined"] = true;
            return RedirectToPage("Index");
        }
    }
}
