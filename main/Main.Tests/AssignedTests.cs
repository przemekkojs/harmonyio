using Main.Data;
using Main.Enumerations;
using Main.Models;
using Main.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Main.Tests;

public class AssignedModelTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ApplicationRepository> _repositoryMock;

    public AssignedModelTests()
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

        var model = new AssignedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_QueryIsNull_ReturnsError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
            It.IsAny<Func<IQueryable<ApplicationUser>, IQueryable<ApplicationUser>>?>()))
            .ReturnsAsync((ApplicationUser?)null);

        var model = new AssignedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_CategorizesQuizzesCorrectly()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);

            var creatorUser = users[0];
            var otherUsers = users.Skip(1).ToList();

            var quizes = GetSeedingQuizes(creatorUser.Id, otherUsers);
            context.Quizes.AddRange(quizes);

            var quizResults = GetSeedingQuizResults(otherUsers[0].Id);
            context.QuizResults.AddRange(quizResults);

            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(otherUsers[0]);

            var repository = new ApplicationRepository(context);
            var pageModel = new AssignedModel(repository, _userManagerMock.Object);


            // Act
            var result = await pageModel.OnGetAsync();


            // Assert
            Assert.NotNull(result);

            Assert.Single(pageModel.WaitingForGrade);
            Assert.Equal("Quiz 5", pageModel.WaitingForGrade[0].Name);

            Assert.Single(pageModel.NotSolvedPlanned);
            Assert.Equal("Quiz 4", pageModel.NotSolvedPlanned[0].Name);
            Assert.Equal(QuizState.NotStarted, pageModel.NotSolvedPlanned[0].State);

            Assert.Single(pageModel.NotSolvedOpen);
            Assert.Equal("Quiz 3", pageModel.NotSolvedOpen[0].Name);
            Assert.Equal(QuizState.Open, pageModel.NotSolvedOpen[0].State);

            Assert.Equal(2, pageModel.Graded.Count); // one quiz with solution, one without solution
            Assert.Contains(pageModel.Graded, q => q.Name == "Quiz 1");
            Assert.Contains(pageModel.Graded, q => q.Name == "Quiz 2");

            Assert.Equal(2, pageModel.GradedQuizes.Count); // one open and one closed has grade
            Assert.Contains(1, pageModel.GradedQuizes.Keys);
            Assert.Contains(2, pageModel.GradedQuizes.Keys);
        }
    }

    [Fact]
    public async Task OnPostDeclineRequest_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = new AssignedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostDeclineRequest();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnPostDeclineRequest_RequestNotFound_ReturnsForbid()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<QuizRequest, bool>>>(),
            It.IsAny<Func<IQueryable<QuizRequest>, IQueryable<QuizRequest>>?>()))
            .ReturnsAsync((QuizRequest?)null);

        var model = new AssignedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostDeclineRequest();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnPostDeclineRequest_DeletesRequest()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var quizRequest = new QuizRequest
        {
            Id = 1,
            UserId = appUser.Id,
            QuizId = 1,
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<QuizRequest, bool>>>(),
            It.IsAny<Func<IQueryable<QuizRequest>, IQueryable<QuizRequest>>?>()))
            .ReturnsAsync((QuizRequest?)quizRequest);

        var model = new AssignedModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostDeclineRequest();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Null(redirectResult.PageName);

        _repositoryMock.Verify(r => r.Delete(It.IsAny<QuizRequest>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

    }



    private List<Quiz> GetSeedingQuizes(string creatorUserId, List<ApplicationUser> participants)
    {
        // create 5 quizes: open and closed with solutions; open, not opened, closed without solutions
        return new List<Quiz>{
            // Create quiz 1: Open, with solutions
            new Quiz
            {
                Id = 1,
                Name = "Quiz 1",
                CreatorId = creatorUserId,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                IsCreated = true,
                Exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        ExerciseSolutions = participants.Select(u => new ExerciseSolution { UserId = u.Id }).ToList()
                    }
                },
                Participants = participants
            },

            // Create quiz 2: Closed, with solutions
            new Quiz
            {
                Id = 2,
                Name = "Quiz 2",
                CreatorId = creatorUserId,
                OpenDate = DateTime.Now.AddHours(-2),
                CloseDate = DateTime.Now.AddHours(-1),
                IsCreated = true,
                Exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        ExerciseSolutions = participants.Select(u => new ExerciseSolution { UserId = u.Id }).ToList()
                    }
                },
                Participants = participants
            },

            // Create quiz 3: Open, with no solutions
            new Quiz
            {
                Id = 3,
                Name = "Quiz 3",
                CreatorId = creatorUserId,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                IsCreated = true,
                Exercises = new List<Exercise>(),
                Participants = participants
            },

            // Create quiz 4: Not started, with no solutions
            new Quiz
            {
                Id = 4,
                Name = "Quiz 4",
                CreatorId = creatorUserId,
                OpenDate = DateTime.Now.AddHours(1),
                CloseDate = DateTime.Now.AddHours(2),
                IsCreated = true,
                Exercises = new List<Exercise>(),
                Participants = participants
            },

            // Create quiz 5: Closed, with no solutions
            new Quiz
            {
                Id = 5,
                Name = "Quiz 5",
                CreatorId = creatorUserId,
                OpenDate = DateTime.Now.AddHours(-3),
                CloseDate = DateTime.Now.AddHours(-2),
                IsCreated = true,
                Exercises = new List<Exercise>(),
                Participants = participants
            }
        };
    }

    private List<QuizResult> GetSeedingQuizResults(string userId)
    {
        return new List<QuizResult>() {
            new QuizResult
            {
                QuizId = 1,
                UserId = userId,
                Grade = Grade.TwoMinus
            },
            new QuizResult
            {
                QuizId = 2,
                UserId = userId,
                Grade = Grade.TwoPlus
            }
        };
    }
}