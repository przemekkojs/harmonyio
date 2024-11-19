using System.Collections;
using System.Collections.Frozen;
using System.Security.Claims;
using System.Text.Json;
using Main.Data;
using Main.Enumerations;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using NuGet.Packaging;
using NuGet.Protocol;

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
    public Dictionary<Quiz, (int, int)> QuizesToUsersCompleted { get; set; } = [];
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
                    .ThenInclude(q => q.Excersises)
                        .ThenInclude(e => e.ExcersiseSolutions)
                            .ThenInclude(es => es.ExcersiseResult)
                .Include(u => u.CreatedQuizes)
                .Include(u => u.TeacherInGroups)
                .Include(u => u.MasterInGroups)
            );

        if (user == null)
        {
            return RedirectToPage("/Error");
        };

        var allCreated = user.CreatedQuizes.Reverse();

        Sketches = allCreated.Where(q => !q.IsCreated).ToList();
        var published = allCreated.Where(q => q.IsCreated);

        ReadyToGrade = published
            .Where(q =>
                q.Excersises.Any(e => e.ExcersiseSolutions.Any(es => es.ExcersiseResult?.QuizResultId == null)) ||
                q.QuizResults.Any(qr => qr.Grade == null)
            ).ToList();
        Opened = published.Where(q => q.State == QuizState.Open).ToList();
        NotOpened = published.Where(q => q.State == QuizState.NotStarted).ToList();
        Closed = published.Where(q => q.State == QuizState.Closed).ToList();

        QuizesToUsersCompleted = published.ToDictionary(
            q => q,
            q => (
                q.Excersises
                    .SelectMany(e => e.ExcersiseSolutions)
                    .Select(s => s.UserId)
                    .Distinct()
                    .Count(),
                q.Participants.Count)
        );

        Groups = user.TeacherInGroups
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

        var quizToAssign = await _repository.GetAsync<Quiz>(
            q => q.Id == QuizId,
            q => q
                .Include(q => q.Participants)
                .Include(q => q.PublishedToGroup)
        );

        // check if user is creator of quiz, if not then he cant assign to this quiz
        if (quizToAssign == null || appUser.Id != quizToAssign.CreatorId)
        {
            return RedirectToPage("Error");
        }

        var userGroups = await _repository.GetAsync<ApplicationUser>(
            u => u.Id == appUser.Id,
            u => u
                .Include(u => u.TeacherInGroups)
                .Include(u => u.MasterInGroups)
        );

        if (userGroups == null)
        {
            return RedirectToPage("Error");
        }

        // parse groupIds as int, delete those that cant be parsed, can also return error
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

        var validUserGroupIds = userGroups.TeacherInGroups
            .Select(g => g.Id)
            .Concat(userGroups.MasterInGroups.Select(g => g.Id))
            .ToHashSet();

        // check if user is teacher or master in all groups hes trying to assign
        if (!groupIds.All(id => validUserGroupIds.Contains(id)))
        {
            return RedirectToPage("Error");
        }

        var foundUsers = await _repository.GetAllAsync<ApplicationUser>(
            u => u.Where(u => u.Email != null && emails.Contains(u.Email))
        );

        var usersByEmail = foundUsers.ToDictionary(u => u.Email!, u => u);
        var notFoundMails = emails
                .Where(email => !usersByEmail.ContainsKey(email))
                .ToList();

        if (notFoundMails.Count() != 0)
        {
            return new JsonResult(new { notFoundEmails = notFoundMails });
        }

        // at this point emails and groupIds are valid

        var newGroups = await _repository.GetAllAsync<UsersGroup>(
            g => g.Where(
                g => groupIds.Contains(g.Id) &&
                !quizToAssign.PublishedToGroup.Contains(g))
                .Include(g => g.Students)
        );
        quizToAssign.PublishedToGroup.AddRange(newGroups);

        var allUsersFromNewGroups = newGroups.SelectMany(
            g => g.Students
        );

        var existingParticipantIds = quizToAssign.Participants.Select(p => p.Id).ToHashSet();
        var allNewQuizParticipants = foundUsers
            .Concat(allUsersFromNewGroups)
            .Where(u => !existingParticipantIds.Contains(u.Id))
            .ToHashSet();

        quizToAssign.Participants.AddRange(allNewQuizParticipants);

        _repository.Update(quizToAssign);
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
            return Forbid();

        var quizToPublish = await _repository.GetAsync<Quiz>(q => q.Id == QuizId && q.CreatorId == appUser.Id);
        var noClosedDate = CloseDate == null;
        var noOpenDate = OpenDate == null;

        if (quizToPublish == null || quizToPublish.IsCreated || noClosedDate || noOpenDate)
            return RedirectToPage("Error");

        // TODO: Jaki� popup, �e nie mo�na
        if (!quizToPublish.IsValid)
            return Page();

        quizToPublish.CloseDate = (DateTime)CloseDate!;
        quizToPublish.OpenDate = (DateTime)OpenDate!;
        quizToPublish.IsCreated = true;

        _repository.Update(quizToPublish);
        await _repository.SaveChangesAsync();

        return RedirectToPage();

    }

    public async Task<IActionResult> OnPostDelete()
    {
        var appUser = await _userManager.GetUserAsync(User);
        if (appUser == null)
        {
            return Forbid();
        }

        var deletedQuiz = await _repository.GetAsync<Quiz>(q => q.Id == QuizId && q.CreatorId == appUser.Id);
        if (deletedQuiz == null)
        {
            return RedirectToPage("Error");
        }

        _repository.Delete(deletedQuiz);
        await _repository.SaveChangesAsync();

        return RedirectToPage();
    }
}