using System.Linq.Expressions;
using System.Security.Claims;
using Main.Data;
using Main.Models;
using Main.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Main.Tests;

public class CreatedModelTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ApplicationRepository> _repositoryMock;
    public CreatedModelTests()
    {
        // Mock UserManager
        _userManagerMock = Utilities.MockUserManager<ApplicationUser>();

        // Mock ApplicationRepository
        _repositoryMock = new Mock<ApplicationRepository>(null);
    }

    [Fact]
    public async Task OnGetAsync_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_QuizNotFound_RedirectsToError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
            It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>?>()))
            .ReturnsAsync((ApplicationUser?)null);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostAsssgn_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostAssign();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnPostAssign_NoEmailsAndGroupsProvided_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);
        model.GroupsIds = "";
        model.Emails = "";

        // Act
        var result = await model.OnPostAssign();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
        _repositoryMock.Verify(r => r.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()), Times.Never);

        _repositoryMock.Verify(r => r.GetAsync(
            It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
            It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>?>()), Times.Never);
    }

    [Fact]
    public async Task OnPostAssiin_QuizNotFound_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)null);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);
        model.GroupsIds = "1";

        // Act
        var result = await model.OnPostAssign();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
        _repositoryMock.Verify(r => r.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()), Times.Once);

        _repositoryMock.Verify(r => r.GetAsync(
            It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
            It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>?>()), Times.Never);
    }

    [Fact]
    public async Task OnPostAssign_UserIsNotQuizCreator_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var returnedQuiz = new Quiz
        {
            Id = 1,
            Name = "Quiz name",
            Code = "quiz-code",
            CreatorId = "other",
            IsCreated = false,
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);
        model.GroupsIds = "1";

        // Act
        var result = await model.OnPostAssign();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
        _repositoryMock.Verify(r => r.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()), Times.Once);
        _repositoryMock.Verify(r => r.GetAsync(
            It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
            It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>?>()), Times.Never);
    }

    [Fact]
    public async Task OnPostAssign_UserGroupsNotFound_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var returnedQuiz = new Quiz
        {
            Id = 1,
            Name = "Quiz name",
            Code = "quiz-code",
            CreatorId = "userId",
            IsCreated = false,
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
            It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>?>()))
            .ReturnsAsync((ApplicationUser?)null);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);
        model.GroupsIds = "1";

        // Act
        var result = await model.OnPostAssign();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);

        _repositoryMock.Verify(r => r.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()), Times.Once);
        _repositoryMock.Verify(r => r.GetAsync(
            It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
            It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>?>()), Times.Once);
    }

    [Fact]
    public async Task OnPostAssign_UserIsTryingToAssignNotHisGroups_ReturnsError()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var curUser = users[0];

            var groups = new List<UsersGroup> {
                new UsersGroup {
                    Id = 1,
                    Name = "G1",
                    MasterId = users[1].Id
                }
            };
            context.UsersGroups.AddRange(groups);

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz name",
                Code = "quiz-code",
                CreatorId = curUser.Id
            };
            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);
            var repository = new ApplicationRepository(context);

            var model = new CreatedModel(repository, _userManagerMock.Object);
            model.QuizId = 1;
            model.GroupsIds = "1";

            //Act
            var result = await model.OnPostAssign();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Error", redirectResult.PageName);
        }
    }

    [Fact]
    public async Task OnPostAssign_UserIsTryingToAssignNotExistingUsers_ReturnsNotFoundEmailsJson()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var curUser = users[0];

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz name",
                Code = "quiz-code",
                CreatorId = curUser.Id
            };
            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);
            var repository = new ApplicationRepository(context);

            var model = new CreatedModel(repository, _userManagerMock.Object);
            model.QuizId = 1;
            model.Emails = $"{users[1].Email},{users[2].Email},idontexist@test.com";

            //Act
            var result = await model.OnPostAssign();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);

            var resultData = jsonResult.Value;

            var notFoundEmailsProperty = resultData.GetType().GetProperty("notFoundEmails");
            Assert.NotNull(notFoundEmailsProperty);

            var actualNotFoundEmails = (List<string>)notFoundEmailsProperty.GetValue(resultData);
            var expectedNotFoundEmails = new List<string> { "idontexist@test.com" };
            Assert.Equal(expectedNotFoundEmails, actualNotFoundEmails);
        }
    }

    [Fact]
    public async Task OnPostAssign_Success()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var curUser = users[0];

            var group1 = new UsersGroup
            {
                Id = 1,
                Name = "Group1",
                MasterId = curUser.Id,
                Members = new List<ApplicationUser> { users[1], users[2] }
            };
            var group2 = new UsersGroup
            {
                Id = 2,
                Name = "Group2",
                MasterId = users[1].Id,
                Admins = new List<ApplicationUser> { curUser },
                Members = new List<ApplicationUser> { users[2], users[3] }
            };
            var group3 = new UsersGroup
            {
                Id = 3,
                Name = "Group3",
                MasterId = users[1].Id,
                Admins = new List<ApplicationUser> { curUser },
                Members = new List<ApplicationUser> { users[4] }
            };
            context.UsersGroups.AddRange(group1, group2, group3);

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz name",
                Code = "quiz-code",
                CreatorId = curUser.Id,
                Participants = new List<ApplicationUser> { users[1], users[4] },
                PublishedToGroup = new List<UsersGroup> { group3 },
            };
            context.Quizes.Add(quiz);

            var request = new QuizRequest
            {
                Id = 1,
                UserId = users[3].Id,
                QuizId = 1
            };
            context.QuizRequests.Add(request);

            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);
            var repository = new ApplicationRepository(context);

            var model = new CreatedModel(repository, _userManagerMock.Object);
            model.QuizId = 1;
            model.Emails = $"{users[1].Email},{users[2].Email},{users[5].Email}";
            model.GroupsIds = "1,2,3";

            //Act
            var result = await model.OnPostAssign();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);

            var resultData = jsonResult.Value;

            var notFoundEmailsProperty = resultData.GetType().GetProperty("notFoundEmails");
            Assert.Null(notFoundEmailsProperty);

            var successProperty = resultData.GetType().GetProperty("success");
            Assert.NotNull(successProperty);
            var addedParticipantsCountProperty = resultData.GetType().GetProperty("addedParticipantsCount");
            Assert.NotNull(addedParticipantsCountProperty);
            var newGroupIdsProperty = resultData.GetType().GetProperty("newGroupIds");
            Assert.NotNull(addedParticipantsCountProperty);

            var actualSuccess = (bool)successProperty.GetValue(resultData);
            Assert.True(actualSuccess);

            var actualAddedParticipantsCount = (int)addedParticipantsCountProperty.GetValue(resultData);
            Assert.Equal(2, actualAddedParticipantsCount); //users[2] from group1 or group2 (added once) and users[3] from group2

            var actualNewGroupIds = (List<int>)newGroupIdsProperty.GetValue(resultData);
            Assert.Equal(2, actualNewGroupIds.Count);
            Assert.Contains(1, actualNewGroupIds);
            Assert.Contains(2, actualNewGroupIds);


            var updatedQuiz = await context.Quizes
                .Include(q => q.Participants)
                .Include(q => q.PublishedToGroup)
                .Include(q => q.Requests)
                .FirstOrDefaultAsync(q => q.Id == 1);

            Assert.Equal(4, updatedQuiz!.Participants.Count);

            // check if only one user who was added by email has request - others where added through group
            // also check if request was deleted for user who was added through group
            Assert.Single(updatedQuiz.Requests);
            Assert.Contains(users[5].Id, updatedQuiz.Requests.Select(r => r.UserId));

            Assert.Equal(3, updatedQuiz.PublishedToGroup.Count);

        }
    }

    [Fact]
    public async Task OnPostPublish_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostPublish();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnPostPublish_QuizNotFound_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)null);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostPublish();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostPublish_UserIsNotQuizCreator_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var returnedQuiz = new Quiz
        {
            Id = 1,
            Name = "Quiz name",
            Code = "quiz-code",
            CreatorId = "other",
            IsCreated = false,
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostPublish();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostPublish_QuizPublished_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var returnedQuiz = new Quiz
        {
            Id = 1,
            Name = "Quiz name",
            Code = "quiz-code",
            CreatorId = "userId",
            IsCreated = true,
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostPublish();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostPublish_DatesNotProvided_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var returnedQuiz = new Quiz
        {
            Id = 1,
            Name = "Quiz name",
            Code = "quiz-code",
            CreatorId = "userId",
            IsCreated = false,
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);
        model.OpenDate = null;
        model.CloseDate = null;

        // Act
        var result = await model.OnPostPublish();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostPublish_WrongDatesProvided_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var returnedQuiz = new Quiz
        {
            Id = 1,
            Name = "Quiz name",
            Code = "quiz-code",
            CreatorId = "userId",
            IsCreated = false,
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);
        model.OpenDate = DateTime.Now.AddHours(-1);
        model.CloseDate = DateTime.Now.AddHours(-2);

        // Act
        var result = await model.OnPostPublish();

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);

        var resultData = jsonResult.Value;

        var errorProperty = resultData.GetType().GetProperty("error");
        Assert.NotNull(errorProperty);

        var actualErrorProperty = (string)errorProperty.GetValue(resultData);
        Assert.Equal("OpenAfterCloseDate", actualErrorProperty);
    }

    [Fact]
    public async Task OnPostPublish_Success()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var returnedQuiz = new Quiz
        {
            Id = 1,
            Name = "Quiz name",
            Code = "quiz-code",
            CreatorId = "userId",
            IsCreated = false,
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);
        model.OpenDate = DateTime.Now.AddHours(-2);
        model.CloseDate = DateTime.Now.AddHours(2);

        // Act
        var result = await model.OnPostPublish();

        // Assert

        // Assert
        var jsonResult = Assert.IsType<JsonResult>(result);
        Assert.NotNull(jsonResult.Value);

        var resultData = jsonResult.Value;

        var successProperty = resultData.GetType().GetProperty("success");
        Assert.NotNull(successProperty);

        var actualSuccessProperty = (bool)successProperty.GetValue(resultData);
        Assert.True(actualSuccessProperty);

        _repositoryMock.Verify(r => r.Update(It.IsAny<Quiz>()), Times.Once);
        Assert.True(returnedQuiz.IsCreated);
    }



    [Fact]
    public async Task OnPostDelete_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostDelete();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnPostDelete_QuizNotFound_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)null);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostDelete();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostDelete_UserIsNotQuizCreator_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var returnedQuiz = new Quiz
        {
            Id = 1,
            Name = "Quiz name",
            Code = "quiz-code",
            CreatorId = "other",
            IsCreated = false,
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostDelete();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostDelete_QuizCreated_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var returnedQuiz = new Quiz
        {
            Id = 1,
            Name = "Quiz name",
            Code = "quiz-code",
            CreatorId = "userId",
            IsCreated = true,
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostDelete();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostDelete_Success()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var returnedQuiz = new Quiz
        {
            Id = 1,
            Name = "Quiz name",
            Code = "quiz-code",
            CreatorId = "userId",
            IsCreated = false,
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        var model = new CreatedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostDelete();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Null(redirectResult.PageName);
        _repositoryMock.Verify(r => r.Delete(It.IsAny<Quiz>()), Times.Once);
    }



}