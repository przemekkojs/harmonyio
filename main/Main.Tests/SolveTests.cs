using Main.Data;
using Main.Enumerations;
using Main.GradingAlgorithm;
using Main.Models;
using Main.Pages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Main.Tests;

public class SolveModelTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ApplicationRepository> _repositoryMock;
    private readonly Mock<IGradingAlgorithm> _algorithmMock;

    public SolveModelTests()
    {
        // Mock UserManager
        _userManagerMock = Utilities.MockUserManager<ApplicationUser>();

        // Mock ApplicationRepository
        _repositoryMock = new Mock<ApplicationRepository>(null);

        // Mock Algorithm
        _algorithmMock = new Mock<IGradingAlgorithm>();
        // just return dummy values for algorithm
        _algorithmMock.Setup(am => am.Grade(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .Returns((1, 2, []));
    }

    [Fact]
    public async Task OnGetAsync_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = new SolveModel(_repositoryMock.Object, _userManagerMock.Object, _algorithmMock.Object);

        // Act
        var result = await model.OnGetAsync("code");

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnPost_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = new SolveModel(_repositoryMock.Object, _userManagerMock.Object, _algorithmMock.Object);

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

        var model = new SolveModel(_repositoryMock.Object, _userManagerMock.Object, _algorithmMock.Object);

        // Act
        var result = await model.OnGetAsync("code");

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

        var model = new SolveModel(_repositoryMock.Object, _userManagerMock.Object, _algorithmMock.Object);

        // Act
        var result = await model.OnPost();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGetAsync_QuizResultExists_RedirectsToBrowse()
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
                Name = "Quiz open",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                Participants = new List<ApplicationUser> { curUser },
                QuizResults = new List<QuizResult> { new QuizResult { UserId = curUser.Id, Grade = Grade.Two } }
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var model = new SolveModel(repository, _userManagerMock.Object, _algorithmMock.Object);


            // Act
            var result = await model.OnGetAsync("quiz-code");


            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Browse", redirectResult.PageName); // check if redirected to browse
            Assert.Equal(quiz.Id, redirectResult.RouteValues["id"]); // check if redirected to correct quiz  solve
        }
    }

    [Fact]
    public async Task OnPost_QuizResultExists_RedirectsToBrowse()
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
                Name = "Quiz open",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                Participants = new List<ApplicationUser> { curUser },
                QuizResults = new List<QuizResult> { new QuizResult { UserId = curUser.Id, Grade = Grade.Two } }
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var model = new SolveModel(repository, _userManagerMock.Object, _algorithmMock.Object);
            model.QuizId = 1;

            // Act
            var result = await model.OnPost();


            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Browse", redirectResult.PageName); // check if redirected to browse
            Assert.Equal(quiz.Id, redirectResult.RouteValues["id"]); // check if redirected to correct quiz  solve
        }
    }

    [Fact]
    public async Task OnGetAsync_QuizClosedAndUserIsParticipant_RedirectsToBrowse()
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
                CloseDate = DateTime.Now.AddHours(-1),
                Participants = new List<ApplicationUser> { curUser }
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var model = new SolveModel(repository, _userManagerMock.Object, _algorithmMock.Object);


            // Act
            var result = await model.OnGetAsync("quiz-code");


            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Browse", redirectResult.PageName); // check if redirected to browse
            Assert.Equal(quiz.Id, redirectResult.RouteValues["id"]); // check if redirected to correct quiz  solve
        }
    }

    [Fact]
    public async Task OnPost_QuizClosedAndUserIsParticipant_RedirectsToBrowse()
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
                CloseDate = DateTime.Now.AddHours(-1),
                Participants = new List<ApplicationUser> { curUser }
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var model = new SolveModel(repository, _userManagerMock.Object, _algorithmMock.Object);
            model.QuizId = 1;

            // Act
            var result = await model.OnPost();


            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Browse", redirectResult.PageName); // check if redirected to browse
            Assert.Equal(quiz.Id, redirectResult.RouteValues["id"]); // check if redirected to correct quiz  solve
        }
    }

    [Fact]
    public async Task OnGetAsync_QuizNotStarted_RedirectsToForbid()
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
            var model = new SolveModel(repository, _userManagerMock.Object, _algorithmMock.Object);


            // Act
            var result = await model.OnGetAsync("quiz-code");


            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }

    [Fact]
    public async Task OnPost_QuizNotStarted_RedirectsToForbid()
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
                Name = "Quiz not starrted",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(1),
                CloseDate = DateTime.Now.AddHours(2)
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var model = new SolveModel(repository, _userManagerMock.Object, _algorithmMock.Object);
            model.QuizId = 1;

            // Act
            var result = await model.OnPost();


            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }

    [Fact]
    public async Task OnGetAsync_UserIsNotParticipant_AddsUserToParticipantsAndReturnsPage()
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
                Name = "Quiz open",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                Participants = new List<ApplicationUser> { otherUser },
                QuizResults = new List<QuizResult> { new QuizResult { UserId = otherUser.Id, Grade = Grade.Two } },
                Exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        Question = "q1",
                        MaxPoints = 12,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a1",
                            }
                        }
                    },
                    new Exercise
                    {
                        Question = "q2",
                        MaxPoints = 13,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a2",
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
            var model = new SolveModel(repository, _userManagerMock.Object, _algorithmMock.Object);


            // Act
            var result = await model.OnGetAsync("quiz-code");


            // Assert
            Assert.IsType<PageResult>(result);

            var updatedQuiz = await context.Quizes.Include(q => q.Participants)
                                             .FirstOrDefaultAsync(q => q.Id == 1);
            Assert.NotNull(updatedQuiz);
            Assert.Contains(updatedQuiz.Participants, u => u.Id == curUser.Id);

            Assert.All(model.Answers, a => Assert.Equal(string.Empty, a));
        }
    }

    [Fact]
    public async Task OnPost_UserIsNotParticipant_RedirectsToForbid()
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
                Name = "Quiz started",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(2)
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            var repository = new ApplicationRepository(context);
            var model = new SolveModel(repository, _userManagerMock.Object, _algorithmMock.Object);
            model.QuizId = 1;

            // Act
            var result = await model.OnPost();


            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }

    [Fact]
    public async Task OnGetAsync_UserIsParticipantAndHasSolutions_RetrievesSolutionsAndReturnsPage()
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
                Name = "Quiz open",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                Participants = new List<ApplicationUser> { otherUser, curUser },
                QuizResults = new List<QuizResult> { new QuizResult { UserId = otherUser.Id, Grade = Grade.Two } },
                Exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        Question = "q1",
                        MaxPoints = 12,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a1",
                            },
                            new ExerciseSolution
                            {
                                UserId = curUser.Id,
                                Answer = "a1",
                            }
                        }
                    },
                    new Exercise
                    {
                        Question = "q2",
                        MaxPoints = 13,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a2",
                            },
                            new ExerciseSolution
                            {
                                UserId = curUser.Id,
                                Answer = "a2",
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
            var model = new SolveModel(repository, _userManagerMock.Object, _algorithmMock.Object);


            // Act
            var result = await model.OnGetAsync("quiz-code");


            // Assert
            Assert.IsType<PageResult>(result);

            Assert.Equal("a1", model.Answers[0]);
            Assert.Equal("a2", model.Answers[1]);
        }
    }



    [Fact]
    public async Task OnPost_WrongNumberOfAnswers_RedirectToError()
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
                Name = "Quiz open",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                Participants = new List<ApplicationUser> { otherUser, curUser },
                QuizResults = new List<QuizResult> { new QuizResult { UserId = otherUser.Id, Grade = Grade.Two } },
                Exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        Question = "q1",
                        MaxPoints = 12,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a1",
                            }
                        }
                    },
                    new Exercise
                    {
                        Question = "q2",
                        MaxPoints = 13,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a2",
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
            var model = new SolveModel(repository, _userManagerMock.Object, _algorithmMock.Object);
            model.QuizId = 1;
            model.Answers = new List<string> { "only a1" };


            // Act
            var result = await model.OnPost();


            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Error", redirectResult.PageName);
        }
    }


    [Fact]
    public async Task OnPost_AddsAnswersWhenUserHadNoAnswers()
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
                Name = "Quiz open",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                Participants = new List<ApplicationUser> { otherUser, curUser },
                QuizResults = new List<QuizResult> { new QuizResult { UserId = otherUser.Id, Grade = Grade.Two } },
                Exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        Question = "q1",
                        MaxPoints = 12,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a1",
                            }
                        }
                    },
                    new Exercise
                    {
                        Question = "q2",
                        MaxPoints = 13,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a2",
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
            var model = new SolveModel(repository, _userManagerMock.Object, _algorithmMock.Object);
            model.QuizId = 1;
            model.Answers = new List<string> { "a1", "a2" };


            // Act
            var result = await model.OnPost();


            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("Assigned", redirectResult.PageName);

            var updatedQuiz = await context.Quizes
                                   .Include(q => q.Exercises)
                                       .ThenInclude(e => e.ExerciseSolutions)
                                           .ThenInclude(es => es.ExerciseResult)
                                   .FirstOrDefaultAsync(q => q.Id == 1);
            Assert.NotNull(updatedQuiz);
            var userSolutions = updatedQuiz.Exercises.SelectMany(e => e.ExerciseSolutions.Where(es => es.UserId == curUser.Id)).ToList();
            Assert.Equal(2, userSolutions.Count);
            Assert.Equal("a1", userSolutions[0].Answer);
            Assert.Equal("a2", userSolutions[1].Answer);

            Assert.Equal(0, userSolutions[0].ExerciseResult?.Points);
            Assert.Equal(0, userSolutions[1].ExerciseResult?.Points);

            Assert.Equal(string.Empty, userSolutions[1].ExerciseResult?.Comment);
            Assert.Equal(string.Empty, userSolutions[1].ExerciseResult?.Comment);
        }
    }

    [Fact]
    public async Task OnPost_DoesntDoAnythingWhenUsersSendsSameAnswers()
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
                Name = "Quiz open",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                Participants = new List<ApplicationUser> { otherUser, curUser },
                QuizResults = new List<QuizResult> { new QuizResult { UserId = otherUser.Id, Grade = Grade.Two } },
                Exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        Question = "q1",
                        MaxPoints = 12,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a1",
                            },
                            new ExerciseSolution{
                                UserId = curUser.Id,
                                Answer = "a1"
                            }
                        }
                    },
                    new Exercise
                    {
                        Question = "q2",
                        MaxPoints = 13,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a2",
                            },
                            new ExerciseSolution{
                                UserId = curUser.Id,
                                Answer = "a2"
                            }
                        }
                    },
                }
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            _repositoryMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Quiz, bool>>>(),
                It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)quiz);

            var model = new SolveModel(_repositoryMock.Object, _userManagerMock.Object, _algorithmMock.Object);
            model.QuizId = 1;
            model.Answers = new List<string> { "a1", "a2" };


            // Act
            var result = await model.OnPost();


            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            _repositoryMock.Verify(r => r.Delete(It.IsAny<ExerciseSolution>()), Times.Never);
            _repositoryMock.Verify(r => r.Add(It.IsAny<ExerciseSolution>()), Times.Never);
            _repositoryMock.Verify(r => r.Add(It.IsAny<ExerciseResult>()), Times.Never);
        }
    }

    [Fact]
    public async Task OnPost_ChangesSolutionWhenNew()
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
                Name = "Quiz open",
                Code = "quiz-code",
                CreatorId = creatorUser.Id,
                IsCreated = true,
                OpenDate = DateTime.Now.AddHours(-1),
                CloseDate = DateTime.Now.AddHours(1),
                Participants = new List<ApplicationUser> { otherUser, curUser },
                QuizResults = new List<QuizResult> { new QuizResult { UserId = otherUser.Id, Grade = Grade.Two } },
                Exercises = new List<Exercise>
                {
                    new Exercise
                    {
                        Question = "q1",
                        MaxPoints = 12,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a1",
                            },
                            new ExerciseSolution{
                                UserId = curUser.Id,
                                Answer = "a1"
                            }
                        }
                    },
                    new Exercise
                    {
                        Question = "q2",
                        MaxPoints = 13,
                        ExerciseSolutions = new List<ExerciseSolution>
                        {
                            new ExerciseSolution
                            {
                                UserId = otherUser.Id,
                                Answer = "other user a2",
                            },
                            new ExerciseSolution{
                                UserId = curUser.Id,
                                Answer = "a2"
                            }
                        }
                    },
                }
            };

            context.Quizes.Add(quiz);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);

            _repositoryMock.Setup(repo => repo.GetAsync(
                It.IsAny<Expression<Func<Quiz, bool>>>(),
                It.IsAny<Func<IQueryable<Quiz>, IQueryable<Quiz>>?>()))
            .ReturnsAsync((Quiz?)quiz);

            var model = new SolveModel(_repositoryMock.Object, _userManagerMock.Object, _algorithmMock.Object);
            model.QuizId = 1;
            model.Answers = new List<string> { "a3", "a2" };


            // Act
            var result = await model.OnPost();


            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            _repositoryMock.Verify(r => r.Delete(It.IsAny<ExerciseSolution>()), Times.Once);
            _repositoryMock.Verify(r => r.Add(It.IsAny<ExerciseSolution>()), Times.Once);
            _repositoryMock.Verify(r => r.Add(It.IsAny<ExerciseResult>()), Times.Once);
        }
    }


}