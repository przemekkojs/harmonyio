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
        public int RequestId { get; set; }
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
                au => au.Id == appUser.Id,
                au => au
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
                    .Include(u => u.Requests)
                        .ThenInclude(r => r.Group)
                        .ThenInclude(g => g.MasterUser)
            );

            if (appUser == null)
            {
                return Forbid();
            }

            AppUser = appUser;

            var showJoined = TempData["showJoined"] as bool?;
            if (showJoined.HasValue)
            {
                ViewData["ActiveTab"] = showJoined.Value ? "Joined" : "Owned";
            }
            else
            {
                ViewData["ActiveTab"] = "Joined";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteGroup()
        {
            var appUser = await _userManager.GetUserAsync(User);

            if (appUser == null)
            {
                return Forbid();
            }

            var group = await _repository.GetAsync<UsersGroup>(q => q.Id == GroupId);

            // only master user can delete the group
            if (group == null || group.MasterId != appUser.Id)
            {
                return RedirectToPage("Error");
            }

            _repository.Delete(group);
            await _repository.SaveChangesAsync();

            TempData["showJoined"] = false;
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCreateGroup()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Forbid();
            }

            if (GroupName == "")
            {
                //Model error instead?
                return RedirectToPage("Error");
            }

            var group = new UsersGroup()
            {
                MasterUser = currentUser,
                Name = GroupName
            };

            _repository.Add(group);
            await _repository.SaveChangesAsync();

            return RedirectToPage("Details", new { id = group.Id });
        }

        public async Task<IActionResult> OnPostDeclineRequest()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return Forbid();
            }

            var groupRequest = await _repository.GetAsync<GroupRequest>(q => q.Id == RequestId);
            // group could be deleted between loading the screen and accepting, so just redirect to page and do nothing
            if (groupRequest == null)
            {
                return RedirectToPage();
            }

            if (currentUser.Id != groupRequest.UserId)
            {
                return Forbid();
            }

            if (groupRequest.ForTeacher)
            {
                TempData["showJoined"] = false;
            }
            else
            {
                TempData["showJoined"] = true;
            }

            _repository.Delete(groupRequest);
            await _repository.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAcceptRequest()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Forbid();
            }

            var groupRequest = await _repository.GetAsync<GroupRequest>(q => q.Id == RequestId);
            // group could be deleted between loading the screen and accepting, so just redirect to page and do nothing
            if (groupRequest == null)
            {
                return RedirectToPage();
            }

            if (currentUser.Id != groupRequest.UserId)
            {
                return Forbid();
            }

            var user = await _repository.GetAsync<ApplicationUser>(
                filter: u => u.Id == currentUser.Id,
                modifier: u => u
                    .Include(u => u.Requests)
                        .ThenInclude(r => r.Group)
                    .Include(u => u.StudentInGroups)
                    .Include(u => u.TeacherInGroups));

            if (user == null)
            {
                return RedirectToPage("Error");
            }

            if (groupRequest.ForTeacher)
            {
                TempData["showJoined"] = false;
                user.TeacherInGroups.Add(groupRequest.Group);
            }
            else
            {
                TempData["showJoined"] = true;
                user.StudentInGroups.Add(groupRequest.Group);
            }

            _repository.Update(user);
            _repository.Delete(groupRequest);
            await _repository.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
