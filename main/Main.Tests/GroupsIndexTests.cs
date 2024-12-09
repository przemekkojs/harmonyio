using Main.Data;
using Main.Models;
using Main.Pages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Main.Tests;

public class GroupsIndexModelTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ApplicationRepository> _repositoryMock;
    public GroupsIndexModelTests()
    {
        // Mock UserManager
        _userManagerMock = Utilities.MockUserManager<ApplicationUser>();

        // Mock ApplicationRepository
        _repositoryMock = new Mock<ApplicationRepository>(null);
    }

    private GroupsIndexModel CreateModel(ApplicationRepository? repository = null)
    {
        var httpContext = new DefaultHttpContext();
        var modelState = new ModelStateDictionary();
        var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(), modelState);
        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var viewData = new ViewDataDictionary(modelMetadataProvider, modelState);
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        var pageContext = new PageContext(actionContext)
        {
            ViewData = viewData
        };

        if (repository != null)
        {
            return new GroupsIndexModel(repository, _userManagerMock.Object)
            {
                PageContext = pageContext,
                TempData = tempData
            };
        }
        else
        {
            return new GroupsIndexModel(_repositoryMock.Object, _userManagerMock.Object)
            {
                PageContext = pageContext,
                TempData = tempData
            };
        }
    }

    [Fact]
    public async Task OnGetAsync_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = new GroupsIndexModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_RepositoryReturnsNull_ReturnsForbid()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };

        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>?>()
            ))
            .ReturnsAsync((ApplicationUser?)null);

        var model = CreateModel();

        // Act
        var result = await model.OnGetAsync();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_TempDataShowJoinedSetsActiveTab_ViewDataUpdatedCorrectly()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };

        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>?>()
            ))
            .ReturnsAsync(appUser);

        var model = CreateModel();

        model.TempData["showJoined"] = true;

        // Act
        var result = await model.OnGetAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Joined", model.ViewData["ActiveTab"]);
    }

    [Fact]
    public async Task OnGetAsync_TempDataShowOwnedSetsActiveTab_ViewDataUpdatedCorrectly()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };

        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>?>()
            ))
            .ReturnsAsync(appUser);

        var model = CreateModel();

        model.TempData["showJoined"] = false;

        // Act
        var result = await model.OnGetAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Owned", model.ViewData["ActiveTab"]);
    }

    [Fact]
    public async Task OnGetAsync_TempDataNotSet_DefaultsToJoined()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };

        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>?>()
            ))
            .ReturnsAsync(appUser);

        var model = CreateModel();

        // Act
        var result = await model.OnGetAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Joined", model.ViewData["ActiveTab"]);
    }

    [Fact]
    public async Task OnPostDeleteGroup_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = CreateModel();

        // Act
        var result = await model.OnPostDeleteGroup();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnPostDeleteGroup_GroupNotFoundOrNotMaster_ReturnsErrorPage()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        var testGroup = new UsersGroup { Id = 1, MasterId = "AnotherUserId" }; // MasterId is different from the user
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<UsersGroup, bool>>>(),
            It.IsAny<Func<IQueryable<UsersGroup>, IQueryable<UsersGroup>>?>()))
            .ReturnsAsync(testGroup);

        var model = CreateModel();
        model.GroupId = 1; // GroupId to delete

        // Act
        var result = await model.OnPostDeleteGroup();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        var redirectResult = result as RedirectToPageResult;
        Assert.Equal("/Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostDeleteGroup_SuccessfulDeletion_RedirectsToPage()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
            context.Users.Add(appUser);

            var testGroup = new UsersGroup { Id = 1, MasterId = "userId" }; // User is master of the group
            context.UsersGroups.Add(testGroup);

            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(appUser);

            var repository = new ApplicationRepository(context);
            var model = CreateModel(repository);
            model.GroupId = 1; // GroupId to delete


            // Act
            var result = await model.OnPostDeleteGroup();


            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = result as RedirectToPageResult;
            Assert.Null(redirectResult.PageName); // Should redirect to the same page

            var deletedGroup = await context.UsersGroups.FirstOrDefaultAsync(g => g.Id == 1);
            Assert.Null(deletedGroup);

            Assert.Equal(false, model.TempData["showJoined"]); // Verify that TempData was set
        }
    }

    [Fact]
    public async Task OnPostCreateGroup_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = CreateModel();

        // Act
        var result = await model.OnPostCreateGroup();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnPostCreateGroup_EmptyGroupName_RedirectsToErrorPage()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync(appUser);

        var model = CreateModel();
        model.GroupName = ""; // Empty group name

        // Act
        var result = await model.OnPostCreateGroup();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        var redirectResult = result as RedirectToPageResult;
        Assert.Equal("/Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostCreateGroup_SuccessfulGroupCreation_RedirectsToDetailsPage()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
            context.Users.Add(appUser);

            await context.SaveChangesAsync(); // Save the user to the context

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(appUser);

            var repository = new ApplicationRepository(context);
            var model = CreateModel(repository);
            model.GroupName = "Test Group"; // Valid group name

            // Act
            var result = await model.OnPostCreateGroup();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);
            var redirectResult = result as RedirectToPageResult;
            Assert.Equal("Details", redirectResult.PageName); // Should redirect to Details page

            var createdGroup = await context.UsersGroups.FirstOrDefaultAsync(g => g.Name == "Test Group");
            Assert.NotNull(createdGroup); // Ensure the group is created in the database
            Assert.Equal("Test Group", createdGroup.Name); // Verify the group name
            Assert.Equal("userId", createdGroup.MasterId); // Verify the master user
        }
    }

    [Fact]
    public async Task OnPostDeclineRequest_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync((ApplicationUser?)null);

        var model = CreateModel();

        // Act
        var result = await model.OnPostDeclineRequest();

        // Assert
        Assert.IsType<ForbidResult>(result);
        _repositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never); // Ensure SaveChangesAsync wasn't called
    }

    [Fact]
    public async Task OnPostAcceptRequest_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync((ApplicationUser?)null);

        var model = CreateModel();

        // Act
        var result = await model.OnPostAcceptRequest();

        // Assert
        Assert.IsType<ForbidResult>(result);
        _repositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never); // Ensure SaveChangesAsync wasn't called
    }

    [Fact]
    public async Task OnPostDeclineRequest_GroupRequestNotFound_ReturnsRedirectToPage()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<GroupRequest, bool>>>(),
            It.IsAny<Func<IQueryable<GroupRequest>, IQueryable<GroupRequest>>?>()))
            .ReturnsAsync((GroupRequest?)null); // No request found

        var model = CreateModel();

        // Act
        var result = await model.OnPostDeclineRequest();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        _repositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never); // Ensure SaveChangesAsync wasn't called
    }

    [Fact]
    public async Task OnPostAcceptRequest_GroupRequestNotFound_ReturnsRedirectToPage()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<GroupRequest, bool>>>(),
            It.IsAny<Func<IQueryable<GroupRequest>, IQueryable<GroupRequest>>?>()))
            .ReturnsAsync((GroupRequest?)null); // No request found

        var model = CreateModel();

        // Act
        var result = await model.OnPostAcceptRequest();

        // Assert
        Assert.IsType<RedirectToPageResult>(result);
        _repositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never); // Ensure SaveChangesAsync wasn't called
    }

    [Fact]
    public async Task OnPostDeclineRequest_UserNotOwnerOfRequest_ReturnsForbid()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        var groupRequest = new GroupRequest { Id = 1, UserId = "otherUserId" };

        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<GroupRequest, bool>>>(),
            It.IsAny<Func<IQueryable<GroupRequest>, IQueryable<GroupRequest>>?>()))
            .ReturnsAsync(groupRequest);

        var model = CreateModel();

        // Act
        var result = await model.OnPostDeclineRequest();

        // Assert
        Assert.IsType<ForbidResult>(result);
        _repositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never); // Ensure SaveChangesAsync wasn't called
    }

    [Fact]
    public async Task OnPostAcceptRequest_UserNotOwnerOfRequest_ReturnsForbid()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        var groupRequest = new GroupRequest { Id = 1, UserId = "otherUserId" };

        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<GroupRequest, bool>>>(),
            It.IsAny<Func<IQueryable<GroupRequest>, IQueryable<GroupRequest>>?>()))
            .ReturnsAsync(groupRequest);

        var model = CreateModel();

        // Act
        var result = await model.OnPostAcceptRequest();

        // Assert
        Assert.IsType<ForbidResult>(result);
        _repositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Never); // Ensure SaveChangesAsync wasn't called
    }

    [Fact]
    public async Task OnPostDeclineRequest_SuccessfulDecline_SetsTempDataAndDdeletesRequest()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
            context.Users.Add(appUser);

            var group = new UsersGroup { Id = 1, Name = "Test Group" };
            context.UsersGroups.Add(group);

            var groupRequest = new GroupRequest { Id = 1, UserId = "userId", GroupId = 1, ForAdmin = true };
            context.GroupRequests.Add(groupRequest);

            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(appUser);

            var repository = new ApplicationRepository(context);
            var model = CreateModel(repository);
            model.RequestId = 1;

            // Act
            var result = await model.OnPostDeclineRequest();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);

            // user wasnt added to group
            Assert.DoesNotContain(group, appUser.AdminInGroups);
            Assert.DoesNotContain(group, appUser.MemberInGroups);

            // request was deleted
            var declinedRequest = await context.GroupRequests.FirstOrDefaultAsync(gr => gr.Id == 1);
            Assert.Null(declinedRequest);

            Assert.True(model.TempData.ContainsKey("showJoined"));
            Assert.False((bool)model.TempData["showJoined"]);
        }
    }

    [Fact]
    public async Task OnPostAcceptRequest_SuccessfulAccept_SetsTempDataAndAddsToGroupAndDeletesRequest()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
            context.Users.Add(appUser);

            var group = new UsersGroup { Id = 1, Name = "Test Group" };
            context.UsersGroups.Add(group);

            var groupRequest = new GroupRequest { Id = 1, UserId = "userId", GroupId = 1, ForAdmin = false };
            context.GroupRequests.Add(groupRequest);

            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                           .ReturnsAsync(appUser);

            var repository = new ApplicationRepository(context);
            var model = CreateModel(repository);
            model.RequestId = 1;

            // Act
            var result = await model.OnPostAcceptRequest();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);

            Assert.Contains(group, appUser.MemberInGroups);
            var acceptedRequest = await context.GroupRequests.FirstOrDefaultAsync(gr => gr.Id == 1);
            Assert.Null(acceptedRequest);

            Assert.True(model.TempData.ContainsKey("showJoined"));
            Assert.True((bool)model.TempData["showJoined"]);
        }
    }
}