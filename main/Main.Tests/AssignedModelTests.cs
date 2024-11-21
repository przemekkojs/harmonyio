using System.Security.Claims;
using Main.Data;
using Main.Enumerations;
using Main.Models;
using Main.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

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

            Assert.Single(pageModel.SolvedOpen);
            Assert.Equal("Quiz 1", pageModel.SolvedOpen[0].Name);
            Assert.Equal(QuizState.Open, pageModel.SolvedOpen[0].State);

            Assert.Single(pageModel.NotSolvedPlanned);
            Assert.Equal("Quiz 4", pageModel.NotSolvedPlanned[0].Name);
            Assert.Equal(QuizState.NotStarted, pageModel.NotSolvedPlanned[0].State);

            Assert.Single(pageModel.NotSolvedOpen);
            Assert.Equal("Quiz 3", pageModel.NotSolvedOpen[0].Name);
            Assert.Equal(QuizState.Open, pageModel.NotSolvedOpen[0].State);

            Assert.Equal(2, pageModel.Closed.Count); // one quiz with solution, one without solution
            Assert.Contains(pageModel.Closed, q => q.Name == "Quiz 2" && q.State == QuizState.Closed);
            Assert.Contains(pageModel.Closed, q => q.Name == "Quiz 5" && q.State == QuizState.Closed);

            Assert.Equal(2, pageModel.GradedQuizes.Count); // one open and one closed has grade
            Assert.Contains(1, pageModel.GradedQuizes.Keys);
            Assert.Contains(2, pageModel.GradedQuizes.Keys);
        }
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
                Excersises = new List<Excersise>
                {
                    new Excersise
                    {
                        ExcersiseSolutions = participants.Select(u => new ExcersiseSolution { UserId = u.Id }).ToList()
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
                Excersises = new List<Excersise>
                {
                    new Excersise
                    {
                        ExcersiseSolutions = participants.Select(u => new ExcersiseSolution { UserId = u.Id }).ToList()
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
                Excersises = new List<Excersise>(),
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
                Excersises = new List<Excersise>(),
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
                Excersises = new List<Excersise>(),
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