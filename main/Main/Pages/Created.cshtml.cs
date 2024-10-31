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
using NuGet.Protocol;

namespace Main.Pages;

[Authorize] 
public class CreatedModel : PageModel
{   
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationRepository _repository;

    [BindProperty]
    public DateTime? OpenDate { get; set; } = null;

    [BindProperty]
    public DateTime? CloseDate { get; set; } = null;

    [BindProperty]
    public int ChosenQuizId { get; set; }

    [BindProperty]
    public string Emails { get; set; } = "";

    [BindProperty]
    public string GroupsIds { get; set; } = "";

    [BindProperty]
    public Dictionary<int, string> Groups { get; set; } = null!;

    public CreatedModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _repository = repository;
    }

    public ICollection<Quiz> UsersQuizes { get; set; } = null!;
    public Dictionary<Quiz, (int, int)> QuizesToUsersCompleted { get; set; } = null!;
    

    public async Task<bool> Init()
    {
        var appUser = await _userManager.GetUserAsync(User);
        if (appUser == null) return false;

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
                .Include(u => u.TeacherInGroups));

        if (user == null) return false;

        UsersQuizes = user.CreatedQuizes;
        QuizesToUsersCompleted = UsersQuizes.ToDictionary(
            q => q,
            q => (q.Excersises.Any() ? q.Excersises.First().ExcersiseSolutions.Count : 0, q.Participants.Count)
        );

        Groups = user.TeacherInGroups.ToDictionary(
            g => g.Id,
            g => g.Name
        );
        
        return true;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        var success = await Init();
        if (!success)
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostPublish()
    {
        var quizToPublic = await _repository.GetAsync<Quiz>(q => q.Id == ChosenQuizId);

        if (quizToPublic == null || quizToPublic.IsCreated)
        {
            return RedirectToPage("Error");
        }

        quizToPublic.CloseDate = (DateTime)CloseDate!;

        quizToPublic.OpenDate = (DateTime)OpenDate!;

        quizToPublic.IsCreated = true;

        _repository.Update(quizToPublic);

        await _repository.SaveChangesAsync();

        return RedirectToPage();

    }

    public async Task<IActionResult> OnPostAssign()
    {
        var groupsIdsStrings = JsonSerializer.Deserialize<List<string>>(GroupsIds)!;

        var groupsIds = groupsIdsStrings.Select(int.Parse).ToList();

        var emails = Emails.Split(',').ToHashSet();

        var quizToAssign = await _repository.GetAsync<Quiz>(
            q => q.Id == ChosenQuizId,
            q => q
                .Include(a => a.Participants)
        );

        if (quizToAssign == null)
        {
            return RedirectToPage("Error");
        }

        var usersToAsign = await _repository.GetAllAsync<ApplicationUser>(
            q => q
                .Where(u => emails.Any(e => e == u.Email))
        );

        var groupsToAsign = await _repository.GetAllAsync<UsersGroup>(
            q => q
                .Where(
                    g =>
                    !Groups.Keys.Contains(g.Id) &&
                    groupsIds.Contains(g.Id)
                )
                .Include(g => g.Students)
        );

        foreach(var group in groupsToAsign)
        {
            foreach (var user in group.Students)
            {
                usersToAsign.Add(user);
            }
        }

        foreach(var user in usersToAsign)
        {
            if (!quizToAssign.Participants.Contains(user))
            {
                quizToAssign.Participants.Add(user);
            }
        }

        quizToAssign.PublishedToEmails.Clear();

        foreach(var email in emails)
        {
            quizToAssign.PublishedToEmails.Add(email);
        }

        quizToAssign.PublishedToGroupIds.Clear();

        foreach(var groupId in groupsIds)
        {
            quizToAssign.PublishedToGroupIds.Add(groupId);
        }

        _repository.Update(quizToAssign);

        await _repository.SaveChangesAsync();
        
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDelete()
    {
        var deletedQuiz = await _repository.GetAsync<Quiz>(q => q.Id == ChosenQuizId);
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