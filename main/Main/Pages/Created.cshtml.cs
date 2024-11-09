using System.Collections;
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
    public string Emails { get; set; } = "";

    [BindProperty]
    public string GroupsIds { get; set; } = "";


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

        var allCreated = user.CreatedQuizes;

        Sketches = allCreated.Where(q => !q.IsCreated).ToList();
        var published = allCreated.Where(q => q.IsCreated);

        ReadyToGrade = published
            .Where(q =>
                q.Excersises.Any(e => e.ExcersiseSolutions.Any(es => es.ExcersiseResult == null)) ||
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

        Groups = user.TeacherInGroups.ToDictionary(
            g => g.Id,
            g => g.Name
        );

        return Page();
    }

    public async Task<IActionResult> OnPostAssign()
    {
        var groupsIdsStrings = JsonSerializer.Deserialize<List<string>>(GroupsIds)!;

        var groupsIds = groupsIdsStrings.Select(int.Parse).ToList();

        var emails = new HashSet<string>();

        if (Emails != "" && Emails != null)
        {
            if (Emails.Contains(','))
            {
                foreach (var email in Emails.Split(','))
                {
                    emails.Add(email);
                }
            }
            else
            {
                emails.Add(Emails);
            }
        }

        var quizToAssign = await _repository.GetAsync<Quiz>(
            q => q.Id == QuizId,
            q => q
                .Include(a => a.Participants)
                .Include(a => a.PublishedToGroup)
        );

        if (quizToAssign == null)
        {
            return RedirectToPage("Error");
        }

        var usersToAsign = new List<ApplicationUser>();

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
                usersToAsign.Add(user);
            }
        }

        if (notFoundMails.Any())
        {
            return new JsonResult(new { notFoundEmails = notFoundMails });
        }

        foreach (var user in usersToAsign)
        {
            if (!quizToAssign.Participants.Contains(user))
            {
                quizToAssign.Participants.Add(user);
            }
        }


        var groupsToAsign = await _repository.GetAllAsync<UsersGroup>(
            q => q
                .Where(
                    g =>
                    !Groups.Keys.Contains(g.Id) &&
                    groupsIds.Contains(g.Id)
                )
                .Include(g => g.Students)
        );

        foreach (var group in groupsToAsign)
        {
            foreach (var user in group.Students)
            {
                if (!quizToAssign.Participants.Contains(user))
                {
                    quizToAssign.Participants.Add(user);
                }
            }
        }

        quizToAssign.PublishedToGroup.Clear();

        foreach (var groupId in groupsIds)
        {
            var group = await _repository.GetAsync<UsersGroup>(q => q.Id == groupId);
            if (group == null)
            {
                return RedirectToPage("Error");
            }
            quizToAssign.PublishedToGroup.Add(group);
        }

        _repository.Update(quizToAssign);

        await _repository.SaveChangesAsync();

        return new JsonResult(new { success = true });
    }

    public async Task<IActionResult> OnPostPublish()
    {
        var appUser = await _userManager.GetUserAsync(User);
        if (appUser == null)
        {
            return Forbid();
        }

        var quizToPublic = await _repository.GetAsync<Quiz>(q => q.Id == QuizId && q.CreatorId == appUser.Id);

        if (quizToPublic == null || quizToPublic.IsCreated || CloseDate == null || OpenDate == null)
        {
            return RedirectToPage("Error");
        }

        quizToPublic.CloseDate = (DateTime)CloseDate;
        quizToPublic.OpenDate = (DateTime)OpenDate;
        quizToPublic.IsCreated = true;

        _repository.Update(quizToPublic);
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

    public bool IsReadyToGrade(Quiz quiz) =>
        quiz.QuizResults.Count == 0 && (quiz.State == QuizState.Closed ||
            (QuizesToUsersCompleted[quiz].Item2 > 0 &&
                QuizesToUsersCompleted[quiz].Item1 == QuizesToUsersCompleted[quiz].Item2));
}