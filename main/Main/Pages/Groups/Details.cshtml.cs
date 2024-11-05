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
                return Forbid();
            }

            // if (group.MasterId == null)
            // {
            //     var teacher = group.Teachers.FirstOrDefault() ?? appUser;

            //     group.MasterUser = teacher;

            //     group.Teachers.Remove(teacher);

            //     _repository.Update(group);

            //     await _repository.SaveChangesAsync();
            // }

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
                return RedirectToPage("Error");
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
                return RedirectToPage("Error");
            }

            var teacher = group.Teachers.FirstOrDefault(u => u.Id == UserId);

            if (teacher == null)
            {
                var student = group.Students.FirstOrDefault(u => u.Id == UserId);

                if (student == null)
                {
                    return RedirectToPage("Error");
                }
                group.Students.Remove(student);
            }
            else
            {
                group.Teachers.Remove(teacher);
            }

            _repository.Update(group);

            await _repository.SaveChangesAsync();

            return RedirectToRoute(new { id = GroupId});
        }

        public async Task<IActionResult> OnPostAddUsers()
        {
            var appUser = await _userManager.GetUserAsync(User);

            if (appUser == null)
            {
                return RedirectToPage("Error");
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
                return RedirectToPage("Error");
            }

            if (EmailsAsString == "" || EmailsAsString == null)
            {
                return RedirectToRoute(new { id = GroupId});
            }

            var emails = new HashSet<string>();

            if (EmailsAsString.Contains(','))
            {
                foreach (var email in EmailsAsString.Split(','))
                {
                    emails.Add(email);
                }
            }
            else
            {
                emails.Add(EmailsAsString);
            }

            var users = new List<ApplicationUser>();

            var notFoundMails = new List<string>();

            foreach (var email in emails)
            {
                var user = await _repository.GetAsync<ApplicationUser>(
                    u => u.Email == email
                );
                if (user == null)
                {
                    notFoundMails.Add(email);
                }
                else
                {
                    users.Add(user);
                }
            }

            if (notFoundMails.Any())
            {
                return new JsonResult(notFoundMails);
            }

            foreach(var user in users)
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

            return RedirectToRoute(new { id = GroupId});
        }

        public IActionResult OnPostRedirectToIndexOwned()
        {
            TempData["OwnedShown"] = true;
            return RedirectToPage("Index");
        }

        public IActionResult OnPostRedirectToIndexJoined()
        {
            TempData["OwnedShown"] = false;
            return RedirectToPage("Index");
        }
    }
}
