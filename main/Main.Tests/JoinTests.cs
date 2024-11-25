using System.Linq.Expressions;
using System.Security.Claims;
using Main.Data;
using Main.Enumerations;
using Main.Models;
using Main.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Main.Tests;

public class JoinModelTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ApplicationRepository> _repositoryMock;
    public JoinModelTests()
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

        var model = new JoinModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync("code");

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
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)null);

        var model = new JoinModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync("code");

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_UserIsParticipant_DoesntAddParticipantAndReturnsPage()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);

            var creatorUser = users[0];
            var participantUser = users[1]; // Simulating the existing participant

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz with Participant",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                Participants = new List<ApplicationUser> { participantUser }
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(participantUser);

            var repository = new ApplicationRepository(context);
            var pageModel = new JoinModel(repository, _userManagerMock.Object);


            // Act
            var result = await pageModel.OnGetAsync("quiz-code");


            // Assert
            Assert.NotNull(result);
            Assert.IsType<PageResult>(result);

            var updatedQuiz = await context.Quizes.Include(q => q.Participants)
                                             .FirstOrDefaultAsync(q => q.Id == 1);
            Assert.NotNull(updatedQuiz);
            Assert.Single(updatedQuiz.Participants); // Ensure only one participant
            Assert.Contains(updatedQuiz.Participants, u => u.Id == participantUser.Id);
        }
    }

    [Fact]
    public async Task OnGetAsync_QuizIsClosed_DoesntAddParticipantAndReturnsPage()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);

            var creatorUser = users[0];
            var curUser = users[1];

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz closed",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-2),
                CloseDate = DateTime.Now.AddHours(-1)
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var pageModel = new JoinModel(repository, _userManagerMock.Object);


            // Act
            var result = await pageModel.OnGetAsync("quiz-code");


            // Assert
            Assert.NotNull(result);
            Assert.IsType<PageResult>(result);

            var updatedQuiz = await context.Quizes.Include(q => q.Participants)
                                             .FirstOrDefaultAsync(q => q.Id == 1);
            Assert.NotNull(updatedQuiz);
            Assert.Empty(updatedQuiz.Participants); // Ensure only one participant
        }
    }

    [Fact]
    public async Task OnGetAsync_QuizIsntClosedAndUserIsntParticipant_AddsParticipantAndReturnsPage()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);

            var creatorUser = users[0];
            var curUser = users[1];

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz closed",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1)
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var pageModel = new JoinModel(repository, _userManagerMock.Object);


            // Act
            var result = await pageModel.OnGetAsync("quiz-code");


            // Assert
            Assert.NotNull(result);
            Assert.IsType<PageResult>(result);

            var updatedQuiz = await context.Quizes.Include(q => q.Participants)
                                             .FirstOrDefaultAsync(q => q.Id == 1);
            Assert.NotNull(updatedQuiz);
            Assert.Single(updatedQuiz.Participants); // Ensure participant is added
            Assert.Contains(updatedQuiz.Participants, u => u.Id == curUser.Id);
        }
    }
}