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

public class BrowseModelTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ApplicationRepository> _repositoryMock;
    public BrowseModelTests()
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

        var model = new BrowseModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync(123);

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

        var model = new BrowseModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync(123);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_UserIsntParticipant_ReturnsForbid()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);

            var creatorUser = users[0];
            var curUser = users[1];
            var otherUser = users[2];

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz started",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                Participants = new List<ApplicationUser> { otherUser }
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var pageModel = new BrowseModel(repository, _userManagerMock.Object);


            // Act
            var result = await pageModel.OnGetAsync(1);


            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }

    [Fact]
    public async Task OnGetAsync_QuizNotStarted_ReturnsForbid()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);

            var creatorUser = users[0];
            var curUser = users[1];
            var otherUser = users[2];

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz not started",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(1),
                CloseDate = DateTime.Now.AddHours(2),
                Participants = new List<ApplicationUser> { curUser, otherUser }
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var pageModel = new BrowseModel(repository, _userManagerMock.Object);


            // Act
            var result = await pageModel.OnGetAsync(1);


            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }

    [Fact]
    public async Task OnGetAsync_QuizOpenAndNoResultAndUserIsParticipant_RedirectsToSolve()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);

            var creatorUser = users[0];
            var curUser = users[1];
            var otherUser = users[2];

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz with Participant",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                Participants = new List<ApplicationUser> { curUser, otherUser }
            };
            context.Quizes.Add(quiz);

            var quizResult = new QuizResult
            {
                QuizId = 1,
                UserId = curUser.Id,
                Grade = null // no grade
            };
            context.QuizResults.Add(quizResult);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var pageModel = new BrowseModel(repository, _userManagerMock.Object);


            // Act
            var result = await pageModel.OnGetAsync(1);


            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Solve", redirectResult.PageName); // check if redirected to solve
            Assert.Equal(quiz.Code, redirectResult.RouteValues["code"]); // check if redirected to correct quiz  solve
        }
    }

    [Fact]
    public async Task OnGetAsync_QuizResultExists_ReturnsPage()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);

            var creatorUser = users[0];
            var curUser = users[1]; // Simulating the existing participant
            var otherUser = users[2];

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz closed",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                Participants = new List<ApplicationUser> { curUser, otherUser },
                Excersises = new List<Excersise>
                {
                    new Excersise
                    {
                        Question = "q1",
                        MaxPoints = 12,
                        ExcersiseSolutions = new List<ExcersiseSolution>
                        {
                            // no solution for this question for cur user to check empty behavior
                            new ExcersiseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a1",
                            }
                        }
                    },
                    new Excersise
                    {
                        Question = "q2",
                        MaxPoints = 13,
                        ExcersiseSolutions = new List<ExcersiseSolution>
                        {
                            new ExcersiseSolution
                            {
                                UserId = curUser.Id,
                                Answer = "cur user a2",
                                ExcersiseResult = new ExcersiseResult
                                {
                                    Points = 10,
                                    MaxPoints = 13,
                                    Comment = "cur user comment2",
                                    AlgorithmPoints = 11
                                }
                            },
                            new ExcersiseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a2",
                                ExcersiseResult = new ExcersiseResult
                                {
                                    Points = 11,
                                    MaxPoints = 13,
                                    Comment = "other user comment2",
                                    AlgorithmPoints = 12
                                }
                            }
                        }
                    },
                }
            };

            context.Quizes.Add(quiz);

            var quizResult = new QuizResult
            {
                QuizId = 1,
                UserId = curUser.Id,
                Grade = Grade.Four
            };
            context.QuizResults.Add(quizResult);

            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var pageModel = new BrowseModel(repository, _userManagerMock.Object);


            // Act
            var result = await pageModel.OnGetAsync(1);


            // Assert
            Assert.NotNull(result);
            Assert.IsType<PageResult>(result);

            Assert.Equal(Grade.Four.AsString(), pageModel.GradeString);

            Assert.Equal(2, pageModel.Questions.Count);
            Assert.Equal("q1", pageModel.Questions[0]);
            Assert.Equal("q2", pageModel.Questions[1]);

            Assert.Equal(2, pageModel.Answers.Count);
            Assert.Equal("", pageModel.Answers[0]); // there was no answer for this question 
            Assert.Equal("cur user a2", pageModel.Answers[1]);

            Assert.Equal(2, pageModel.ExerciseResults.Count);

            // no answer to this one, so also no result, so should assign default
            Assert.Equal(0, pageModel.ExerciseResults[0].Points);
            Assert.Equal(12, pageModel.ExerciseResults[0].MaxPoints);
            Assert.Equal(string.Empty, pageModel.ExerciseResults[0].Comment);

            Assert.Equal(10, pageModel.ExerciseResults[1].Points);
            Assert.Equal(13, pageModel.ExerciseResults[1].MaxPoints);
            Assert.Equal("cur user comment2", pageModel.ExerciseResults[1].Comment);
        }
    }
}