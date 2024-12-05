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

public class GradeModelTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ApplicationRepository> _repositoryMock;
    public GradeModelTests()
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

        var model = new GradeModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync(1);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnPost_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = new GradeModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPost();

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

        var model = new GradeModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPost_QuizNotFound_RedirectsToError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)null);

        var model = new GradeModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPost();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_UserCantGradeQuiz_ReturnsForbid()
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
            IsCreated = true,
            OpenDate = DateTime.Now.AddHours(-1),
            CloseDate = DateTime.Now.AddHours(1),
            PublishedToGroup = new List<UsersGroup> {
                new UsersGroup
                {
                    Id = 1,
                    Name = "G1",
                    MasterId = "other",
                    Admins = new List<ApplicationUser> {
                        new ApplicationUser { Id = "otheruser", FirstName = "John", LastName = "Doe" }
                    }
                }
            }
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        var model = new GradeModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync(1);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnPost_UserCantGradeQuiz_ReturnsForbid()
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
            IsCreated = true,
            OpenDate = DateTime.Now.AddHours(-1),
            CloseDate = DateTime.Now.AddHours(1),
            PublishedToGroup = new List<UsersGroup> {
                new UsersGroup
                {
                    Id = 1,
                    Name = "G1",
                    MasterId = "other",
                    Admins = new List<ApplicationUser> {
                        new ApplicationUser { Id = "otheruser", FirstName = "John", LastName = "Doe" }
                    }
                }
            }
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        var model = new GradeModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPost();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnGetAsync_NoSolutions_ReturnsError()
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
            CreatorId = appUser.Id,
            IsCreated = true,
            OpenDate = DateTime.Now.AddHours(-1),
            CloseDate = DateTime.Now.AddHours(1),
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<Quiz, bool>>>(),
            It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)returnedQuiz);

        var model = new GradeModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGetAsync(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_QuizClosed_FillsMissingExerciseSolutionsAndReturnsPage()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);

            var curUser = users[0];
            var participant1 = users[1];
            var participant2 = users[2];

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz open",
                Code = "quiz-code",
                CreatorId = curUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-2),
                CloseDate = DateTime.Now.AddHours(-1),
                Participants = new List<ApplicationUser> { participant1, participant2 },
                Exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        Id = 1,
                        Question = "q1",
                        MaxPoints = 10,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = participant1.Id,
                                Answer = "p1 a1",
                            }
                        }
                    },
                    new Exercise
                    {
                        Id = 2,
                        Question = "q2",
                        MaxPoints = 11,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = participant1.Id,
                                Answer = "p1 a2",
                            }
                        }
                    },
                }
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var pageModel = new GradeModel(repository, _userManagerMock.Object);


            // Act
            var result = await pageModel.OnGetAsync(1);


            // Assert
            Assert.NotNull(result);
            Assert.IsType<PageResult>(result);

            var solutions = await context.ExerciseSolutions.Include(es => es.ExerciseResult).ToListAsync();
            Assert.Equal(4, solutions.Count);
            var newSolutions = solutions.Where(s => s.UserId == participant2.Id).ToList();
            Assert.Equal(string.Empty, newSolutions[0].Answer);
            Assert.Equal(string.Empty, newSolutions[1].Answer);

            Assert.NotNull(newSolutions[0].ExerciseResult);
            Assert.NotNull(newSolutions[1].ExerciseResult);

            Assert.Equal(0, newSolutions[0].ExerciseResult!.Points);
            Assert.Equal(0, newSolutions[1].ExerciseResult!.Points);

            Assert.Equal(string.Empty, newSolutions[0].ExerciseResult!.Comment);
            Assert.Equal(string.Empty, newSolutions[1].ExerciseResult!.Comment);

            Assert.Equal(0, newSolutions[0].ExerciseResult!.AlgorithmPoints);
            Assert.Equal(0, newSolutions[1].ExerciseResult!.AlgorithmPoints);

            Assert.Equal(10, newSolutions.Where(s => s.ExerciseId == 1).Select(s => s.ExerciseResult!.MaxPoints).First());
            Assert.Equal(11, newSolutions.Where(s => s.ExerciseId == 2).Select(s => s.ExerciseResult!.MaxPoints).First());

            Assert.Empty(newSolutions[0].ExerciseResult!.MistakeResults);
            Assert.Empty(newSolutions[1].ExerciseResult!.MistakeResults);
        }
    }

    [Fact]
    public async Task OnGetAsync_Success()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var curUser = new ApplicationUser
            {
                Id = "user1Id",
                FirstName = "User1Name",
                LastName = "User1LastName",
                Email = "user1@mail.com",
                NormalizedEmail = "USER1@MAIL.COM"
            };
            var participant1 = new ApplicationUser
            {
                Id = "user2Id",
                FirstName = "Aa",
                LastName = "Bb",
                Email = "user2@mail.com",
                NormalizedEmail = "USER2@MAIL.COM"
            };
            var participant2 = new ApplicationUser
            {
                Id = "user3Id",
                FirstName = "Cc",
                LastName = "Dd",
                Email = "user3@mail.com",
                NormalizedEmail = "USER3@MAIL.COM"
            };
            context.Users.AddRange(curUser, participant1, participant2);

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz name",
                Code = "quiz-code",
                CreatorId = curUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-2),
                CloseDate = DateTime.Now.AddHours(-1),
                Participants = new List<ApplicationUser> { participant2, participant1 },
                Exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        Id = 1,
                        Question = "q1",
                        MaxPoints = 10,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = participant1.Id,
                                Answer = "p1 a1",
                                ExerciseResult = new ExerciseResult
                                {
                                    Points = 9,
                                    MaxPoints = 10,
                                    Comment = "p1 comment 1",
                                    AlgorithmPoints = 8
                                }
                            }
                        }
                    },
                    new Exercise
                    {
                        Id = 2,
                        Question = "q2",
                        MaxPoints = 11,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = participant1.Id,
                                Answer = "p1 a2",
                                ExerciseResult = new ExerciseResult
                                {
                                    Points = 6,
                                    MaxPoints = 7,
                                    Comment = "p1 comment 2",
                                    AlgorithmPoints = 5
                                }
                            }
                        }
                    },
                },
                QuizResults = new List<QuizResult> {
                    new QuizResult
                    {
                        UserId = participant1.Id,
                        Grade = Enumerations.Grade.Two,
                    }
                }
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var pageModel = new GradeModel(repository, _userManagerMock.Object);


            // Act
            var result = await pageModel.OnGetAsync(1);


            // Assert
            Assert.NotNull(result);
            Assert.IsType<PageResult>(result);

            Assert.Equal(1, pageModel.QuizId);
            Assert.Equal("Quiz name", pageModel.QuizName);
            Assert.False(pageModel.ShareAlgorithmOpinion);

            Assert.Equal("q1", pageModel.Exercises[0].Question);
            Assert.Equal("q2", pageModel.Exercises[1].Question);

            // check if user with no answer was added
            Assert.Equal(2, pageModel.Users.Count);

            // check alfabetical order
            Assert.Equal("user2Id", pageModel.Users[0].Id);
            Assert.Equal("user3Id", pageModel.Users[1].Id);

            Assert.Equal(2, pageModel.UserIds.Count);
            Assert.Equal("user2Id", pageModel.UserIds[0]);
            Assert.Equal("user3Id", pageModel.UserIds[1]);

            Assert.Equal(2, pageModel.Grades.Count);
            Assert.Equal(Enumerations.Grade.Two, pageModel.Grades[0]);
            Assert.Null(pageModel.Grades[1]);

            Assert.Equal(2, pageModel.Solutions.Count);
            Assert.Equal(2, pageModel.Solutions[0].Count);
            Assert.Equal(2, pageModel.Solutions[1].Count);
            Assert.Equal("p1 a1", pageModel.Solutions[0][0]);
            Assert.Equal("p1 a2", pageModel.Solutions[0][1]);
            Assert.Equal("", pageModel.Solutions[1][0]);
            Assert.Equal("", pageModel.Solutions[1][1]);

            Assert.Equal(2, pageModel.Points.Count);
            Assert.Equal(2, pageModel.Points[0].Count);
            Assert.Equal(2, pageModel.Points[1].Count);
            Assert.Equal(9, pageModel.Points[0][0]);
            Assert.Equal(6, pageModel.Points[0][1]);
            Assert.Equal(0, pageModel.Points[1][0]);
            Assert.Equal(0, pageModel.Points[1][1]);

            Assert.Equal(2, pageModel.Comments.Count);
            Assert.Equal(2, pageModel.Comments[0].Count);
            Assert.Equal(2, pageModel.Comments[1].Count);
            Assert.Equal("p1 comment 1", pageModel.Comments[0][0]);
            Assert.Equal("p1 comment 2", pageModel.Comments[0][1]);
            Assert.Equal("", pageModel.Comments[1][0]);
            Assert.Equal("", pageModel.Comments[1][1]);

            Assert.Equal(2, pageModel.PointSuggestions.Count);
            Assert.Equal(2, pageModel.PointSuggestions[0].Count);
            Assert.Equal(2, pageModel.PointSuggestions[1].Count);
            Assert.Equal(8, pageModel.PointSuggestions[0][0]);
            Assert.Equal(5, pageModel.PointSuggestions[0][1]);
            Assert.Equal(0, pageModel.PointSuggestions[1][0]);
            Assert.Equal(0, pageModel.PointSuggestions[1][1]);

            Assert.Equal(2, pageModel.Opinions.Count);
            Assert.Equal(2, pageModel.Opinions[0].Count);
            Assert.Equal(2, pageModel.Opinions[1].Count);
            Assert.Empty(pageModel.Opinions[0][0]);
            Assert.Empty(pageModel.Opinions[0][1]);
            Assert.Empty(pageModel.Opinions[1][0]);
            Assert.Empty(pageModel.Opinions[1][1]);
        }
    }

    [Fact]
    public async Task OnPost_Success()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var curUser = new ApplicationUser
            {
                Id = "user1Id",
                FirstName = "User1Name",
                LastName = "User1LastName",
                Email = "user1@mail.com",
                NormalizedEmail = "USER1@MAIL.COM"
            };
            var participant1 = new ApplicationUser
            {
                Id = "user2Id",
                FirstName = "Aa",
                LastName = "Bb",
                Email = "user2@mail.com",
                NormalizedEmail = "USER2@MAIL.COM"
            };
            var participant2 = new ApplicationUser
            {
                Id = "user3Id",
                FirstName = "Cc",
                LastName = "Dd",
                Email = "user3@mail.com",
                NormalizedEmail = "USER3@MAIL.COM"
            };
            context.Users.AddRange(curUser, participant1, participant2);

            var quiz = new Quiz
            {
                Id = 1,
                Name = "Quiz name",
                Code = "quiz-code",
                CreatorId = curUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-2),
                CloseDate = DateTime.Now.AddHours(-1),
                Participants = new List<ApplicationUser> { participant2, participant1 },
                Exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        Id = 1,
                        Question = "q1",
                        MaxPoints = 10,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = participant1.Id,
                                Answer = "p1 a1",
                                ExerciseResult = new ExerciseResult
                                {
                                    Points = 9,
                                    MaxPoints = 10,
                                    Comment = "p1 comment 1",
                                    AlgorithmPoints = 8
                                }
                            },
                            new ExerciseSolution
                            {
                                UserId = participant2.Id,
                                Answer = "p2 a1",
                                ExerciseResult = new ExerciseResult
                                {
                                    Points = 0,
                                    MaxPoints = 4,
                                    Comment = "",
                                    AlgorithmPoints = 2
                                }
                            }
                        }
                    },
                    new Exercise
                    {
                        Id = 2,
                        Question = "q2",
                        MaxPoints = 11,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = participant1.Id,
                                Answer = "p1 a2",
                                ExerciseResult = new ExerciseResult
                                {
                                    Points = 6,
                                    MaxPoints = 7,
                                    Comment = "p1 comment 2",
                                    AlgorithmPoints = 5
                                }
                            },
                            new ExerciseSolution
                            {
                                UserId = participant2.Id,
                                Answer = "p2 a2",
                                ExerciseResult = new ExerciseResult
                                {
                                    Points = 0,
                                    MaxPoints = 6,
                                    Comment = "",
                                    AlgorithmPoints = 4
                                }
                            }
                        }
                    },
                },
                QuizResults = new List<QuizResult> {
                    new QuizResult
                    {
                        UserId = participant1.Id,
                        Grade = Enumerations.Grade.Two,
                    }
                }
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var pageModel = new GradeModel(repository, _userManagerMock.Object);
            pageModel.QuizId = 1;
            pageModel.UserIds = [participant1.Id, participant2.Id];
            pageModel.Grades = [Enumerations.Grade.Five, Enumerations.Grade.Three];
            pageModel.Points = [[11, 4], [2, 3]];
            pageModel.Comments = [["changed p1 c1", ""], ["changed p2 c1", null]];
            pageModel.ShareAlgorithmOpinion = true;

            // Act
            var result = await pageModel.OnPost();


            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Created", redirectResult.PageName);

            var exerciseResults = await context.ExerciseResults.Include(er => er.QuizResult).Include(er => er.ExerciseSolution).ToListAsync();
            Assert.Equal(4, exerciseResults.Count);

            var p1Results = exerciseResults.Where(er => er.ExerciseSolution.UserId == participant1.Id).ToList();
            var p2Results = exerciseResults.Where(er => er.ExerciseSolution.UserId == participant2.Id).ToList();

            Assert.Equal(2, p1Results.Count);
            Assert.DoesNotContain(11, p1Results.Select(r => r.Points)); // check if points over max were channed to max
            Assert.Equal(10, p1Results.Where(r => r.ExerciseSolution.ExerciseId == 1).Select(r => r.Points).First());
            Assert.Equal(4, p1Results.Where(r => r.ExerciseSolution.ExerciseId == 2).Select(r => r.Points).First());
            Assert.Equal("changed p1 c1", p1Results.Where(r => r.ExerciseSolution.ExerciseId == 1).Select(r => r.Comment).First());
            Assert.Equal("", p1Results.Where(r => r.ExerciseSolution.ExerciseId == 2).Select(r => r.Comment).First());

            Assert.Equal(2, p2Results.Count);
            Assert.Equal(2, p2Results.Where(r => r.ExerciseSolution.ExerciseId == 1).Select(r => r.Points).First());
            Assert.Equal(3, p2Results.Where(r => r.ExerciseSolution.ExerciseId == 2).Select(r => r.Points).First());
            Assert.Equal("changed p2 c1", p2Results.Where(r => r.ExerciseSolution.ExerciseId == 1).Select(r => r.Comment).First());
            Assert.Equal("", p2Results.Where(r => r.ExerciseSolution.ExerciseId == 2).Select(r => r.Comment).First());

            Assert.DoesNotContain(null, exerciseResults.Select(er => er.QuizResult));
            Assert.Equal(Enumerations.Grade.Five, p1Results.Select(r => r.QuizResult!.Grade).First());
            Assert.Equal(Enumerations.Grade.Three, p2Results.Select(r => r.QuizResult!.Grade).First());

            var quizResults = await context.QuizResults.ToListAsync();
            Assert.Equal(2, quizResults.Count);
        }
    }
}