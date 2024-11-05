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
    public class GroupsIndexModel : PageModel
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        public GroupsIndexModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public ApplicationUser AppUser { get; set; } = null!;

        [BindProperty]
        public int GroupId { get; set; }

        [BindProperty]
        public string GroupName { get; set; } = "";

        public async Task<IActionResult> OnGetAsync()
        {
            var appUser = await _userManager.GetUserAsync(User);

            if (appUser == null)
            {
                return Forbid();
            }

            appUser = await _repository.GetAsync<ApplicationUser>(
                q => q.Id == appUser.Id,
                q => q
                    .Include(u => u.StudentInGroups)
                        .ThenInclude(g => g.MasterUser)
                    .Include(u => u.StudentInGroups)
                        .ThenInclude(g => g.Students)
                    .Include(u => u.StudentInGroups)
                        .ThenInclude(g => g.Teachers)
                    .Include(u => u.TeacherInGroups)
                        .ThenInclude(g => g.MasterUser)
                    .Include(u => u.TeacherInGroups)
                        .ThenInclude(g => g.Students)
                    .Include(u => u.TeacherInGroups)
                        .ThenInclude(g => g.Teachers)
                    .Include(u => u.MasterInGroups)
                        .ThenInclude(g => g.Students)
                    .Include(u => u.MasterInGroups)
                        .ThenInclude(g => g.Teachers)
            );

            if (appUser == null)
            {
                return Forbid();
            }
            
            // foreach (var group in appUser.StudentInGroups.Concat(appUser.TeacherInGroups))
            // {
            //     if (group.MasterId == null)
            //     {
            //         var teacher = group.Teachers.First();

            //         group.MasterUser = teacher;

            //         group.Teachers.Remove(teacher);

            //         _repository.Update(group);
            //     }
            // }

            // await _repository.SaveChangesAsync();

            AppUser = appUser;


            return Page();

        }

        public async Task<IActionResult> OnPostDeleteGroup()
        {
            var appUser = await _userManager.GetUserAsync(User);

            if (appUser == null)
            {
                return RedirectToPage("Error");
            }

            var group = await _repository.GetAsync<UsersGroup>(q => q.Id == GroupId );

            if (group == null || 
                (
                    group.MasterId != appUser.Id &&
                    !group.Teachers.Any(t => t.Id == appUser.Id)
                ))
            {
                return RedirectToPage("Error");
            }

            _repository.Delete(group);

            await _repository.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCreateGroup()
        {
            var appUser = await _userManager.GetUserAsync(User);

            if (appUser == null)
            {
                return RedirectToPage("Error");
            }

            if (GroupName == "")
            {
                //Model error instead?
                return RedirectToPage("Error");
            }

            var group = new UsersGroup()
            {
                MasterUser = appUser,
                Name = GroupName
            };

            _repository.Add(group);

            await _repository.SaveChangesAsync();

            return RedirectToPage("Details", new { id = group.Id });
        }

        // public IActionResult OnPostRedirectToDetailsAdmin()
        // {
        //     TempData["IsAdmin"] = true;
        //     return RedirectToPage("Details");
        // }

        // public IActionResult OnPostRedirectToDetailsUser()
        // {
        //     TempData["IsAdmin"] = false;
        //     return RedirectToPage("Details");
        // }
    }
}
