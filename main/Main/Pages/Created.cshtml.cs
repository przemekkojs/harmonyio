using Main.Data;
using Main.Enumerations;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace Main.Pages;

[Authorize]
public class CreatedModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationRepository _repository;

    [BindProperty]
    public DateTime? OpenDate { get; set; }

    [BindProperty]
    public DateTime? CloseDate { get; set; }

    [BindProperty]
    public bool SelfAssign { get; set; } = false;

    [BindProperty]
    public int QuizId { get; set; }

    [BindProperty]
    public string? Emails { get; set; } = "";

    [BindProperty]
    public string? GroupsIds { get; set; } = "";

    public List<Quiz> Sketches { get; set; } = [];
    public List<Quiz> ReadyToGrade { get; set; } = [];
    public List<Quiz> Opened { get; set; } = [];
    public List<Quiz> NotOpened { get; set; } = [];
    public List<Quiz> Closed { get; set; } = [];
    public Dictionary<int, (int, int)> QuizIdsToUsersCompleted { get; set; } = [];
    public Dictionary<int, string> Groups { get; set; } = [];

    public CreatedModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _repository = repository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var appUser = await _userManager.GetUserAsync(User);
        if (appUser == null)
        {
            return Forbid();
        }

        var user = await _repository.GetAsync<ApplicationUser>(
            filter: u => u.Id == appUser.Id,
            modifier: u => u
                .Include(u => u.CreatedQuizes)
                    .ThenInclude(q => q.QuizResults)
                .Include(u => u.CreatedQuizes)
                    .ThenInclude(q => q.Participants)
                .Include(u => u.CreatedQuizes)
                    .ThenInclude(q => q.PublishedToGroup)
                .Include(u => u.CreatedQuizes)
                    .ThenInclude(q => q.Exercises)
                        .ThenInclude(e => e.ExerciseSolutions)
                            .ThenInclude(es => es.ExerciseResult)
                .Include(u => u.CreatedQuizes)
                .Include(u => u.AdminInGroups)
                .Include(u => u.MasterInGroups)
                .AsSplitQuery()
            );

        if (user == null)
        {
            return RedirectToPage("/Error");
        };

        var allCreated = user.CreatedQuizes.Reverse();

        Sketches = allCreated.Where(q => !q.IsCreated).ToList();
        var published = allCreated.Where(q => q.IsCreated);

        QuizIdsToUsersCompleted = published.ToDictionary(
            q => q.Id,
            q => (
                q.Exercises
                    .SelectMany(e => e.ExerciseSolutions)
                    .Select(s => s.UserId)
                    .Distinct()
                    .Count(),
                q.Participants.Count)
        );

        ReadyToGrade = published
            .Where(q => q.Exercises.Any(e => e.ExerciseSolutions.Any()))
            .Where(q =>
                q.Exercises.Any(e => e.ExerciseSolutions.Any(es => es.ExerciseResult?.QuizResultId == null)) ||
                q.QuizResults.Any(qr => qr.Grade == null) ||
                (q.State == QuizState.Closed && QuizIdsToUsersCompleted[q.Id].Item1 != QuizIdsToUsersCompleted[q.Id].Item2)
            ).ToList();
        Opened = published.Where(q => q.State == QuizState.Open).ToList();
        NotOpened = published.Where(q => q.State == QuizState.NotStarted).ToList();
        Closed = published.Where(q => q.State == QuizState.Closed).ToList();


        Groups = user.AdminInGroups
            .Concat(user.MasterInGroups)
            .DistinctBy(g => g.Id)
            .ToDictionary(
                g => g.Id,
                g => g.Name
            );

        return Page();
    }

    public async Task<IActionResult> OnPostAssign()
    {
        var appUser = await _userManager.GetUserAsync(User);
        if (appUser == null)
        {
            return Forbid();
        }

        // parse groupIds as int, delete those that cant be parsed
        var groupIds = GroupsIds != null ?
            GroupsIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(e => e.Trim())
            .Where(e => int.TryParse(e, out _))
            .Select(int.Parse)
            .ToHashSet() : [];

        var emails = Emails != null ?
            Emails
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(e => e.Trim())
            .ToHashSet() : [];

        if (emails.Count == 0 && groupIds.Count == 0)
        {
            return RedirectToPage("Error");
        }

        var quizToAssign = await _repository.GetAsync<Quiz>(
            q => q.Id == QuizId,
            q => q
                .Include(q => q.Participants)
                .Include(q => q.PublishedToGroup)
                .Include(q => q.Requests)
        );

        // check if user is creator of quiz, if not then he cant assign to this quiz
        if (quizToAssign == null || appUser.Id != quizToAssign.CreatorId)
        {
            return RedirectToPage("Error");
        }

        var userGroups = await _repository.GetAsync<ApplicationUser>(
            u => u.Id == appUser.Id,
            u => u
                .Include(u => u.AdminInGroups)
                    .ThenInclude(g => g.Members)
                .Include(u => u.MasterInGroups)
                    .ThenInclude(g => g.Members)
        );

        if (userGroups == null)
        {
            return RedirectToPage("Error");
        }

        var curUserGroups = userGroups.AdminInGroups.Concat(userGroups.MasterInGroups);

        var validUserGroupIds = curUserGroups
            .Select(g => g.Id)
            .ToHashSet();

        // check if user is admin or master in all groups hes trying to assign
        if (!groupIds.All(id => validUserGroupIds.Contains(id)))
        {
            return RedirectToPage("Error");
        }

        var (notFoundEmails, foundUsers) = await _repository.GetAllUsersByEmailAsync(emails);

        if (notFoundEmails.Count != 0)
        {
            return new JsonResult(new { notFoundEmails = notFoundEmails });
        }

        // at this point emails and groupIds are valid

        var puliishedToGroupsIds = quizToAssign.PublishedToGroup.Select(g => g.Id).ToHashSet();

        var newGroups = curUserGroups.Where(
            g => groupIds.Contains(g.Id) &&
            !puliishedToGroupsIds.Contains(g.Id)
        );

        quizToAssign.PublishedToGroup.AddRange(newGroups);

        var allUsersFromNewGroups = newGroups.SelectMany(
            g => g.Members
        );

        var existingParticipantIds = quizToAssign.Participants.Select(p => p.Id).ToHashSet();
        var allNewQuizParticipants = allUsersFromNewGroups
            .Where(u => !existingParticipantIds.Contains(u.Id))
            .ToHashSet();

        var existingRequestsParticipantsIds = quizToAssign.Requests.Select(r => r.UserId).ToHashSet();
        var allNewQuizRequestedUsers = foundUsers
            .Where(u => !existingParticipantIds.Contains(u.Id) &&
                        !allNewQuizParticipants.Contains(u) &&
                        !existingRequestsParticipantsIds.Contains(u.Id))
            .ToHashSet();

        quizToAssign.Participants.AddRange(allNewQuizParticipants);
        _repository.Update(quizToAssign);

        var allNewQuizParticipantsIds = allNewQuizParticipants.Select(u => u.Id).ToHashSet();
        var requestsToDelete = quizToAssign.Requests.Where(r => allNewQuizParticipantsIds.Contains(r.UserId));
        foreach (var request in requestsToDelete)
        {
            _repository.Delete(request);
        }

        foreach (var user in allNewQuizRequestedUsers)
        {
            var request = new QuizRequest
            {
                UserId = user.Id,
                QuizId = quizToAssign.Id,
            };
            _repository.Add(request);
        }


        await _repository.SaveChangesAsync();

        return new JsonResult(
            new
            {
                success = true,
                addedParticipantsCount = allNewQuizParticipants.Count,
                newGroupIds = newGroups.Select(g => g.Id).ToList()
            }
        );
    }

    public async Task<IActionResult> OnPostPublish()
    {
        var appUser = await _userManager.GetUserAsync(User);

        if (appUser == null)
        {
            return Forbid();
        }

        var quizToPublish = await _repository.GetAsync<Quiz>(q => q.Id == QuizId);

        var noClosedDate = CloseDate == null;
        var noOpenDate = OpenDate == null;
        if (quizToPublish == null || quizToPublish.CreatorId != appUser.Id || quizToPublish.IsCreated || noClosedDate || noOpenDate)
        {
            return RedirectToPage("Error");
        }

        if (CloseDate <= OpenDate)
        {
            return new JsonResult(new { error = "OpenAfterCloseDate" });
        }
        if (CloseDate < DateTime.Now)
        {
            return new JsonResult(new { error = "AfterCloseDate" });
        }

        quizToPublish.CloseDate = (DateTime)CloseDate!;
        quizToPublish.OpenDate = (DateTime)OpenDate!;
        quizToPublish.IsCreated = true;

        if (SelfAssign)
        {
            quizToPublish.Participants.Add(appUser);
        }

        _repository.Update(quizToPublish);
        await _repository.SaveChangesAsync();

        return new JsonResult(new { success = true });

    }

    public async Task<IActionResult> OnPostDelete()
    {
        var appUser = await _userManager.GetUserAsync(User);

        if (appUser == null)
            return Forbid();

        var deletedQuiz = await _repository.GetAsync<Quiz>(q => q.Id == QuizId);

        if (deletedQuiz == null || deletedQuiz.CreatorId != appUser.Id || deletedQuiz.IsCreated)
            return RedirectToPage("Error");

        _repository.Delete(deletedQuiz);
        await _repository.SaveChangesAsync();

        return RedirectToPage();
    }
}