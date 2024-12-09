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
    public class GroupDetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationRepository _repository;

        public GroupDetailsModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public string CurrentUserId { get; set; } = "";

        public bool IsAdmin { get; set; }

        public bool IsMaster { get; set; }

        public bool IsParticipant { get; set; }

        public UsersGroup Group { get; set; } = null!;

        public IEnumerable<Quiz> ActiveQuizzes { get; set; } = [];
        public IEnumerable<Quiz> FutureQuizzes { get; set; } = [];
        public IEnumerable<Quiz> ToGradeQuizzes { get; set; } = [];
        public IEnumerable<Quiz> GradedQuizzes { get; set; } = [];

        public HashSet<int> UserSolvedQuizIds { get; set; } = [];
        public HashSet<int> UserGradedQuizIds { get; set; } = [];


        [BindProperty]
        public string UserId { get; set; } = "";

        [BindProperty]
        public bool RemoveFromMembers { get; set; }

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
                    .Include(g => g.Admins)
                    .Include(g => g.Members)
                    .Include(g => g.MasterUser)
                    .Include(g => g.Quizzes)
                        .ThenInclude(q => q.Exercises)
                            .ThenInclude(e => e.ExerciseSolutions)
                                .ThenInclude(es => es.ExerciseResult)
                    .Include(g => g.Quizzes)
                        .ThenInclude(q => q.Creator)
                    .Include(g => g.Quizzes)
                        .ThenInclude(q => q.Participants)
                    .Include(g => g.Quizzes)
                        .ThenInclude(q => q.QuizResults)
                    .Include(g => g.Quizzes)
                    .Include(g => g.Requests)
                        .ThenInclude(r => r.User)
                    .AsSplitQuery()
                );

            if (group == null)
            {
                return RedirectToPage("/Error");
            }
            IsMaster = group.MasterId == appUser.Id;

            IsAdmin = IsMaster || group.Admins.Contains(appUser);

            IsParticipant = group.Members.Contains(appUser);

            Group = group;

            CurrentUserId = appUser.Id;

            if (!IsAdmin && !group.Members.Contains(appUser))
            {
                return Forbid();
            }

            var quizzes = group.Quizzes.Reverse();
            var openOrClosedQuizzes = quizzes.Where(q => q.State != Enumerations.QuizState.NotStarted);

            UserSolvedQuizIds = openOrClosedQuizzes.Where(q => q.Exercises.Any(e => e.ExerciseSolutions.Any(es => es.UserId == CurrentUserId))).Select(q => q.Id).ToHashSet();
            UserGradedQuizIds = openOrClosedQuizzes.Where(q => q.QuizResults.Any(qr => qr.UserId == CurrentUserId && qr.Grade != null)).Select(q => q.Id).ToHashSet();

            ActiveQuizzes = IsAdmin ?
                openOrClosedQuizzes.Where(q => q.State == Enumerations.QuizState.Open) :
                openOrClosedQuizzes.Where(
                    q => q.State == Enumerations.QuizState.Open &&
                    !UserSolvedQuizIds.Contains(q.Id));


            FutureQuizzes = quizzes.Where(q => q.State == Enumerations.QuizState.NotStarted);

            ToGradeQuizzes = IsAdmin ?
                openOrClosedQuizzes.Where(
                    q => q.Exercises.Any(e => e.ExerciseSolutions.Any(es => es.ExerciseResult?.QuizResultId == null)) ||
                        q.QuizResults.Any(qr => qr.Grade == null) ||
                        (q.State == Enumerations.QuizState.Closed && q.QuizResults.Where(qr => qr.Grade != null).Count() != q.Participants.Count)
                    ) :
                openOrClosedQuizzes.Where(
                    q => (UserSolvedQuizIds.Contains(q.Id) && !UserGradedQuizIds.Contains(q.Id)) ||
                    (q.State == Enumerations.QuizState.Closed && !UserGradedQuizIds.Contains(q.Id)));

            GradedQuizzes = IsAdmin ?
                openOrClosedQuizzes.Where(
                    q => (
                            q.QuizResults.Count > 0 &&
                            q.Exercises.All(e => e.ExerciseSolutions.All(es => es.ExerciseResult != null && es.ExerciseResult.QuizResultId != null)) &&
                            q.QuizResults.All(qr => qr.Grade != null)
                        ) ||
                        (
                            q.State == Enumerations.QuizState.Closed &&
                            q.QuizResults.Where(qr => qr.Grade != null).Count() == q.Participants.Count
                        )) :
                openOrClosedQuizzes.Where(q => UserGradedQuizIds.Contains(q.Id));

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
                    .Include(g => g.Admins)
                    .Include(g => g.Members)
            );
            if (group == null)
            {
                return RedirectToPage("/Error");
            }

            var success = false;
            if (RemoveFromMembers)
            {
                success = HandleRemoveFromMembers(appUser, group);
            }
            else
            {
                success = HandleRemoveFromAdmins(appUser, group);
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

        private bool HandleRemoveFromMembers(ApplicationUser appUser, UsersGroup group)
        {
            // Allow only Master or Admin to remove members
            if (group.MasterId != appUser.Id && !group.Admins.Any(t => t.Id == appUser.Id))
            {
                return false;
            }

            // Find and remove the member
            var member = group.Members.FirstOrDefault(u => u.Id == UserId);
            if (member == null)
            {
                return false;
            }

            group.Members.Remove(member);
            return true;
        }

        private bool HandleRemoveFromAdmins(ApplicationUser appUser, UsersGroup group)
        {
            // Only Master can remove a admin
            if (group.MasterId != appUser.Id)
            {
                return false;
            }

            // Find and remove the admin
            var admin = group.Admins.FirstOrDefault(u => u.Id == UserId);
            if (admin == null)
            {
                return false;
            }

            group.Admins.Remove(admin);
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
                    .Include(g => g.Admins)
                    .Include(g => g.Members)
            );

            if (group == null ||
                (
                    group.MasterId != appUser.Id &&
                    !group.Admins.Any(t => t.Id == appUser.Id)
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

            // var foundUsers = await _repository.GetAllAsync<ApplicationUser>(
            //     u => u.Where(u => u.Email != null && emails.Contains(u.Email))
            // );

            // HashSet<string> wrongEmails = new HashSet<string>();

            // var usersByEmail = foundUsers.ToDictionary(u => u.Email!, u => u);
            // var notFoundMails = emails.Except(usersByEmail.Keys.Select(e => e));

            // wrongEmails.AddRange(notFoundMails);

            var (wrongEmails, foundUsers) = await _repository.GetAllUsersByEmailAsync(emails);

            // check if users are already added to group
            if (AsAdmins)
            {
                wrongEmails.AddRange(group.Admins.Where(foundUsers.Contains).Select(u => u.Email!));
            }
            else
            {
                wrongEmails.AddRange(group.Members.Where(foundUsers.Contains).Select(u => u.Email!));
            }

            // check if users have request sent 
            var requestSentEmails = (await _repository.GetAllAsync<GroupRequest>(
                gr => gr
                    .Where(gr => gr.GroupId == GroupId && gr.ForAdmin == AsAdmins)
                    .Include(gr => gr.User)
                ))
                .Select(gr => gr.User.Email!)
                .Where(emails.Contains);

            wrongEmails.AddRange(requestSentEmails);

            if (wrongEmails.Count() != 0)
            {
                return new JsonResult(new { wrongEmails = wrongEmails.ToList() });
            }

            foreach (var user in foundUsers)
            {
                _repository.Add(new GroupRequest()
                {
                    Group = group,
                    ForAdmin = AsAdmins,
                    User = user
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
