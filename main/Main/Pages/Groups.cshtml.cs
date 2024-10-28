using System.ComponentModel.DataAnnotations;
using Main.Data;
using Main.Enumerations;
using Main.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Main.Pages;

[Authorize] 
public class GroupsModel : PageModel
{   
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationRepository _repository;

    [BindProperty]
    [Required(ErrorMessage = "Podaj nazwę grupy")]
    public string GroupName { get; set; } = "";
    [BindProperty]
    public int RequestId { get; set; }
    [BindProperty]
    public int GroupId { get; set; }
    [BindProperty]
    public bool AsTeacher { get; set; }


    public ICollection<GroupRequest> Requests { get; set; } = [];

    public ICollection<UsersGroup> StudentInGroups { get; set; } = [];

    public ICollection<UsersGroup> TeacherInGroups { get; set; } = [];


    public GroupsModel(ApplicationRepository repository, UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
        _repository = repository;
    }

    public async Task<bool> Init()
    {
        var appUser = await _userManager.GetUserAsync(User);
        if (appUser == null) return false;

        var user = await _repository.GetAsync<ApplicationUser>(
            filter: u => u.Id == appUser.Id,
            modifier: u => u
                .Include(u => u.Requests)
                    .ThenInclude(r => r.Group)
                .Include(u => u.StudentInGroups)
                .Include(u => u.TeacherInGroups));

        if (user == null) return false;

        Requests = user.Requests;

        StudentInGroups = user.StudentInGroups;

        TeacherInGroups = user.TeacherInGroups;

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

    public async Task<IActionResult> OnPostCreateGroup()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToPage("Error");
        }

        var group = new UsersGroup(){
            Name = GroupName,
            Teachers = [user]
        };

        _repository.Add(group);

        await _repository.SaveChangesAsync();

        return RedirectToPage("GroupDetails", new { id = group.Id});
    }

    public async Task<IActionResult> OnPostRemoveRequest()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return RedirectToPage("Error");
        }

        var groupRequest = await _repository.GetAsync<GroupRequest>(q => q.Id == RequestId);

        
        if (groupRequest == null || currentUser.Id != groupRequest.UserId)
        {
            return RedirectToPage("Error");
        }
        
        _repository.Delete(groupRequest);

        await _repository.SaveChangesAsync();

        var success = await Init();
        if (!success)
        {
            return Forbid();
        }

        return RedirectToPage();


    }

    public async Task<IActionResult> OnPostQuitGroup()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return RedirectToPage("Error");
        }

        var group = await _repository.GetAsync<UsersGroup>(
            g => g.Id == GroupId,
            q => q
                .Include(g => g.Teachers)
                .Include(g => g.Students)
                );

        
        if (group == null)
        {
            return RedirectToPage("Error");
        }

        if (AsTeacher)
        {
            group.Teachers.Remove(currentUser);
        }
        else
        {
            group.Students.Remove(currentUser);
        }
        
        _repository.Update(group);

        await _repository.SaveChangesAsync();

        return RedirectToPage();

    }

    public async Task<IActionResult> OnPostAcceptRequest()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return RedirectToPage("Error");
        }

        var groupRequest = await _repository.GetAsync<GroupRequest>(q => q.Id == RequestId);

        
        if (groupRequest == null || currentUser.Id != groupRequest.UserId)
        {
            return RedirectToPage("Error");
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
            user.TeacherInGroups.Add(groupRequest.Group);
        }
        else
        {
            user.StudentInGroups.Add(groupRequest.Group);
        }        

        _repository.Update(user);

        _repository.Delete(groupRequest);

        await _repository.SaveChangesAsync();

        return RedirectToPage();
    }
}