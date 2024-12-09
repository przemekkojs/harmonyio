using System.Linq.Expressions;
using System.Security.Claims;
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

namespace Main.Tests;

public class GroupsDetailsModelTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ApplicationRepository> _repositoryMock;
    public GroupsDetailsModelTests()
    {
        // Mock UserManager
        _userManagerMock = Utilities.MockUserManager<ApplicationUser>();

        // Mock ApplicationRepository
        _repositoryMock = new Mock<ApplicationRepository>(null);
    }

    [Fact]
    public async Task OnGet_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = new GroupDetailsModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGet(1);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnGet_GroupNotFound_RedirectsToError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<UsersGroup, bool>>>(),
            It.IsAny<Func<IQueryable<UsersGroup>, IQueryable<UsersGroup>>?>()))
            .ReturnsAsync((UsersGroup?)null);

        var model = new GroupDetailsModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGet(1);

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnGet_UserIsNotInGroup_ReturnsForbid()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var userGroup = new UsersGroup
        {
            Id = 1,
            Name = "gr 1",
            MasterId = "other"
        };

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<UsersGroup, bool>>>(),
            It.IsAny<Func<IQueryable<UsersGroup>, IQueryable<UsersGroup>>?>()))
            .ReturnsAsync((UsersGroup?)userGroup);

        var model = new GroupDetailsModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnGet(1);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnGet_UserIsMember_Success()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var curUser = users[0];
            var otherUser = users[1];
            var masterUser = users[2];

            var group = new UsersGroup
            {
                Id = 1,
                Name = "Group",
                MasterId = masterUser.Id,
                Members = { curUser, otherUser },
                Quizzes =
                {
                    new Quiz
                    {
                        Id = 1,
                        Name = "Future Quiz",
                        OpenDate = DateTime.Now.AddHours(1),
                        CloseDate = DateTime.Now.AddHours(2),
                        IsCreated = true,
                        CreatorId = masterUser.Id,
                        Exercises =
                        {
                            new Exercise
                            {
                                Id = 1,
                                Question = "q1",
                                MaxPoints = 10
                            }
                        }
                    },
                    new Quiz
                    {
                        Id = 2,
                        Name = "Open NotSolved Quiz",
                        OpenDate = DateTime.Now.AddHours(-1),
                        CloseDate = DateTime.Now.AddHours(2),
                        IsCreated = true,
                        CreatorId = masterUser.Id,
                        Exercises =
                        {
                            new Exercise
                            {
                                Id = 2,
                                Question = "q2",
                                MaxPoints = 10,
                                ExerciseSolutions =
                                {
                                    new ExerciseSolution
                                    {
                                        Id = 1,
                                        Answer = "a1",
                                        UserId = otherUser.Id
                                    }
                                }
                            }
                        }
                    },
                    new Quiz
                    {
                        Id = 3,
                        Name = "Open Solved Quiz",
                        OpenDate = DateTime.Now.AddHours(-1),
                        CloseDate = DateTime.Now.AddHours(2),
                        IsCreated = true,
                        CreatorId = masterUser.Id,
                        Exercises =
                        {
                            new Exercise
                            {
                                Id = 3,
                                Question = "q3",
                                MaxPoints = 10,
                                ExerciseSolutions =
                                {
                                    new ExerciseSolution
                                    {
                                        Id = 2,
                                        Answer = "a2",
                                        UserId = curUser.Id
                                    },
                                    new ExerciseSolution
                                    {
                                        Id = 3,
                                        Answer = "a3",
                                        UserId = otherUser.Id
                                    }
                                }
                            }
                        }
                    },
                    new Quiz
                    {
                        Id = 4,
                        Name = "Closed notSolved Quiz",
                        OpenDate = DateTime.Now.AddHours(-2),
                        CloseDate = DateTime.Now.AddHours(-1),
                        IsCreated = true,
                        CreatorId = masterUser.Id,
                        Exercises =
                        {
                            new Exercise
                            {
                                Id = 4,
                                Question = "q4",
                                MaxPoints = 10,
                                ExerciseSolutions =
                                {
                                    new ExerciseSolution
                                    {
                                        Id = 5,
                                        Answer = "a5",
                                        UserId = otherUser.Id
                                    }
                                }
                            }
                        }
                    },
                    new Quiz
                    {
                        Id = 5,
                        Name = "Graded Quiz",
                        OpenDate = DateTime.Now.AddHours(-1),
                        CloseDate = DateTime.Now.AddHours(2),
                        IsCreated = true,
                        CreatorId = masterUser.Id,
                        QuizResults =
                        {
                            new QuizResult
                            {
                                Id = 1,
                                Grade = Enumerations.Grade.Five,
                                GradeDate = DateTime.Now.AddHours(-1),
                                UserId = curUser.Id
                            }
                        },
                        Exercises =
                        {
                            new Exercise
                            {
                                Id = 5,
                                Question = "q5",
                                MaxPoints = 10,
                                ExerciseSolutions =
                                {
                                    new ExerciseSolution
                                    {
                                        Id = 6,
                                        Answer = "a6",
                                        UserId = curUser.Id,
                                        ExerciseResult = new ExerciseResult
                                        {
                                            Id = 1,
                                            Points = 2,
                                            MaxPoints = 10,
                                            AlgorithmPoints = 3,
                                            QuizResultId = 1,
                                        }


                                    },
                                    new ExerciseSolution
                                    {
                                        Id = 7,
                                        Answer = "a7",
                                        UserId = otherUser.Id
                                    }
                                }
                            }
                        }
                    }
                }
            };
            context.UsersGroups.Add(group);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(curUser);
            var repository = new ApplicationRepository(context);

            var model = new GroupDetailsModel(repository, _userManagerMock.Object);

            //Act
            var result = await model.OnGet(1);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PageResult>(result);

            Assert.False(model.IsMaster);
            Assert.False(model.IsAdmin);
            Assert.True(model.IsParticipant);
            Assert.Equal(1, model.Group.Id);
            Assert.Equal(curUser.Id, model.CurrentUserId);

            Assert.Equal(2, model.UserSolvedQuizIds.Count);
            Assert.Contains(3, model.UserSolvedQuizIds);
            Assert.Contains(5, model.UserSolvedQuizIds);

            Assert.Single(model.UserGradedQuizIds);
            Assert.Contains(5, model.UserGradedQuizIds);

            Assert.Single(model.ActiveQuizzes);
            Assert.Contains(2, model.ActiveQuizzes.Select(q => q.Id));

            Assert.Single(model.FutureQuizzes);
            Assert.Contains(1, model.FutureQuizzes.Select(q => q.Id));

            Assert.Equal(2, model.ToGradeQuizzes.Count());
            Assert.Contains(3, model.ToGradeQuizzes.Select(q => q.Id));
            Assert.Contains(4, model.ToGradeQuizzes.Select(q => q.Id));

            Assert.Single(model.GradedQuizzes);
            Assert.Contains(5, model.GradedQuizzes.Select(q => q.Id));

        }
    }

    [Fact]
    public async Task OnGet_UserIsAdmin_Success()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var otherUser1 = users[0];
            var otherUser2 = users[1];
            var masterUser = users[2];

            var group = new UsersGroup
            {
                Id = 1,
                Name = "Group",
                MasterId = masterUser.Id,
                Members = { otherUser1, otherUser2 },
                Quizzes =
                {
                    new Quiz
                    {
                        Id = 1,
                        Name = "Future Quiz",
                        OpenDate = DateTime.Now.AddHours(1),
                        CloseDate = DateTime.Now.AddHours(2),
                        IsCreated = true,
                        CreatorId = masterUser.Id,
                        Exercises =
                        {
                            new Exercise
                            {
                                Id = 1,
                                Question = "q1",
                                MaxPoints = 10
                            }
                        }
                    },
                    new Quiz
                    {
                        Id = 2,
                        Name = "Open Quiz",
                        OpenDate = DateTime.Now.AddHours(-1),
                        CloseDate = DateTime.Now.AddHours(2),
                        IsCreated = true,
                        CreatorId = masterUser.Id,
                        Exercises =
                        {
                            new Exercise
                            {
                                Id = 2,
                                Question = "q2",
                                MaxPoints = 10,
                            }
                        }
                    },
                    new Quiz
                    {
                        Id = 3,
                        Name = "Open notGraded Quiz",
                        OpenDate = DateTime.Now.AddHours(-1),
                        CloseDate = DateTime.Now.AddHours(2),
                        IsCreated = true,
                        CreatorId = masterUser.Id,
                        Exercises =
                        {
                            new Exercise
                            {
                                Id = 3,
                                Question = "q3",
                                MaxPoints = 10,
                                ExerciseSolutions =
                                {
                                    new ExerciseSolution
                                    {
                                        Id = 2,
                                        Answer = "a2",
                                        UserId = otherUser1.Id
                                    },
                                    new ExerciseSolution
                                    {
                                        Id = 3,
                                        Answer = "a3",
                                        UserId = otherUser2.Id
                                    }
                                }
                            }
                        }
                    },
                    new Quiz
                    {
                        Id = 4,
                        Name = "Closed notGraded Quiz",
                        OpenDate = DateTime.Now.AddHours(-2),
                        CloseDate = DateTime.Now.AddHours(-1),
                        IsCreated = true,
                        CreatorId = masterUser.Id,
                        Participants = {otherUser1},
                        Exercises =
                        {
                            new Exercise
                            {
                                Id = 4,
                                Question = "q4",
                                MaxPoints = 10,
                                ExerciseSolutions =
                                {
                                    new ExerciseSolution
                                    {
                                        Id = 5,
                                        Answer = "a5",
                                        UserId = otherUser1.Id
                                    }
                                }
                            }
                        }
                    },
                    new Quiz
                    {
                        Id = 5,
                        Name = "Graded Quiz",
                        OpenDate = DateTime.Now.AddHours(-1),
                        CloseDate = DateTime.Now.AddHours(2),
                        IsCreated = true,
                        CreatorId = masterUser.Id,
                        QuizResults =
                        {
                            new QuizResult
                            {
                                Id = 1,
                                Grade = Enumerations.Grade.Five,
                                GradeDate = DateTime.Now.AddHours(-1),
                                UserId = otherUser1.Id
                            }
                        },
                        Exercises =
                        {
                            new Exercise
                            {
                                Id = 5,
                                Question = "q5",
                                MaxPoints = 10,
                                ExerciseSolutions =
                                {
                                    new ExerciseSolution
                                    {
                                        Id = 6,
                                        Answer = "a6",
                                        UserId = otherUser1.Id,
                                        ExerciseResult = new ExerciseResult
                                        {
                                            Id = 1,
                                            Points = 2,
                                            MaxPoints = 10,
                                            AlgorithmPoints = 3,
                                            QuizResultId = 1,
                                        }


                                    }
                                }
                            }
                        }
                    }
                }
            };
            context.UsersGroups.Add(group);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(masterUser);
            var repository = new ApplicationRepository(context);

            var model = new GroupDetailsModel(repository, _userManagerMock.Object);

            //Act
            var result = await model.OnGet(1);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PageResult>(result);

            Assert.True(model.IsMaster);
            Assert.True(model.IsAdmin);
            Assert.False(model.IsParticipant);
            Assert.Equal(1, model.Group.Id);
            Assert.Equal(masterUser.Id, model.CurrentUserId);

            Assert.Equal(3, model.ActiveQuizzes.Count());
            Assert.Contains(2, model.ActiveQuizzes.Select(q => q.Id));
            Assert.Contains(3, model.ActiveQuizzes.Select(q => q.Id));
            Assert.Contains(5, model.ActiveQuizzes.Select(q => q.Id));

            Assert.Single(model.FutureQuizzes);
            Assert.Contains(1, model.FutureQuizzes.Select(q => q.Id));

            Assert.Equal(2, model.ToGradeQuizzes.Count());
            Assert.Contains(3, model.ToGradeQuizzes.Select(q => q.Id));
            Assert.Contains(4, model.ToGradeQuizzes.Select(q => q.Id));

            Assert.Single(model.GradedQuizzes);
            Assert.Contains(5, model.GradedQuizzes.Select(q => q.Id));
        }
    }

    [Fact]
    public async Task OnPostDeleteUser_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = new GroupDetailsModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostDeleteUser();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnPostDeleteUser_GroupNotFound_RedirectsToError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<UsersGroup, bool>>>(),
            It.IsAny<Func<IQueryable<UsersGroup>, IQueryable<UsersGroup>>?>()))
            .ReturnsAsync((UsersGroup?)null);

        var model = new GroupDetailsModel(_repositoryMock.Object, _userManagerMock.Object);
        model.GroupId = 2;

        // Act
        var result = await model.OnPostDeleteUser();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostDeleteUser_UserIsNotPermittedToDeleteFromMembers_RedirectsToError()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var memberUser = users[0];
            var adminUser = users[1];
            var masterUser = users[2];

            var group = new UsersGroup
            {
                Id = 1,
                Name = "Group",
                MasterId = masterUser.Id,
                Members = { memberUser },
                Admins = { adminUser }
            };
            context.UsersGroups.Add(group);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(memberUser);
            var repository = new ApplicationRepository(context);

            var model = new GroupDetailsModel(repository, _userManagerMock.Object);
            model.GroupId = 1;
            model.UserId = adminUser.Id;
            model.RemoveFromMembers = true;
            //Act
            var result = await model.OnPostDeleteUser();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Error", redirectResult.PageName);
        }
    }

    [Fact]
    public async Task OnPostDeleteUser_UserIsNotPermittedToDeleteFromAdmins_RedirectsToError()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var memberUser = users[0];
            var adminUser = users[1];
            var masterUser = users[2];

            var group = new UsersGroup
            {
                Id = 1,
                Name = "Group",
                MasterId = masterUser.Id,
                Members = { memberUser },
                Admins = { adminUser }
            };
            context.UsersGroups.Add(group);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(adminUser);
            var repository = new ApplicationRepository(context);

            var model = new GroupDetailsModel(repository, _userManagerMock.Object);
            model.GroupId = 1;
            model.UserId = adminUser.Id;
            model.RemoveFromMembers = false;
            //Act
            var result = await model.OnPostDeleteUser();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Error", redirectResult.PageName);
        }
    }

    [Fact]
    public async Task OnPostDeleteUser_MemberToDeleteNotFound_RedirectsToError()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var memberUser = users[0];
            var adminUser = users[1];
            var masterUser = users[2];
            var otherUser = users[3];

            var group = new UsersGroup
            {
                Id = 1,
                Name = "Group",
                MasterId = masterUser.Id,
                Members = { memberUser },
                Admins = { adminUser }
            };
            context.UsersGroups.Add(group);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(adminUser);
            var repository = new ApplicationRepository(context);

            var model = new GroupDetailsModel(repository, _userManagerMock.Object);
            model.GroupId = 1;
            model.UserId = otherUser.Id;
            model.RemoveFromMembers = true;
            //Act
            var result = await model.OnPostDeleteUser();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Error", redirectResult.PageName);
        }
    }

    [Fact]
    public async Task OnPostDeleteUser_AdminToDeleteNotFound_RedirectsToError()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var memberUser = users[0];
            var adminUser = users[1];
            var masterUser = users[2];
            var otherUser = users[3];

            var group = new UsersGroup
            {
                Id = 1,
                Name = "Group",
                MasterId = masterUser.Id,
                Members = { memberUser },
                Admins = { adminUser }
            };
            context.UsersGroups.Add(group);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(adminUser);
            var repository = new ApplicationRepository(context);

            var model = new GroupDetailsModel(repository, _userManagerMock.Object);
            model.GroupId = 1;
            model.UserId = otherUser.Id;
            model.RemoveFromMembers = false;
            //Act
            var result = await model.OnPostDeleteUser();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Error", redirectResult.PageName);
        }
    }

    [Fact]
    public async Task OnPostDeleteUser_MemberDeleted_ReturnsJsonResult()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var memberUser = users[0];
            var adminUser = users[1];
            var masterUser = users[2];
            var otherUser = users[3];

            var group = new UsersGroup
            {
                Id = 1,
                Name = "Group",
                MasterId = masterUser.Id,
                Members = { memberUser },
                Admins = { adminUser }
            };
            context.UsersGroups.Add(group);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(adminUser);
            var repository = new ApplicationRepository(context);

            var model = new GroupDetailsModel(repository, _userManagerMock.Object);
            model.GroupId = 1;
            model.UserId = memberUser.Id;
            model.RemoveFromMembers = true;
            //Act
            var result = await model.OnPostDeleteUser();

            // Assert
            var redirectResult = Assert.IsType<JsonResult>(result);

            var updatedGroup = await context.UsersGroups
                .Include(g => g.Admins)
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == 1);
            Assert.Empty(updatedGroup.Members);
        }
    }

    [Fact]
    public async Task OnPostDeleteUser_AdminDeleted_ReturnsJsonResult()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var memberUser = users[0];
            var adminUser = users[1];
            var masterUser = users[2];
            var otherUser = users[3];

            var group = new UsersGroup
            {
                Id = 1,
                Name = "Group",
                MasterId = masterUser.Id,
                Members = { memberUser },
                Admins = { adminUser }
            };
            context.UsersGroups.Add(group);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(masterUser);
            var repository = new ApplicationRepository(context);

            var model = new GroupDetailsModel(repository, _userManagerMock.Object);
            model.GroupId = 1;
            model.UserId = adminUser.Id;
            model.RemoveFromMembers = false;
            //Act
            var result = await model.OnPostDeleteUser();

            // Assert
            var redirectResult = Assert.IsType<JsonResult>(result);

            var updatedGroup = await context.UsersGroups
                .Include(g => g.Admins)
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == 1);
            Assert.Empty(updatedGroup.Admins);
        }
    }

    [Fact]
    public async Task OnPostAddUsers_UserNotFound_ReturnsForbid()
    {
        // Arrange
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                       .ReturnsAsync((ApplicationUser?)null);

        var model = new GroupDetailsModel(_repositoryMock.Object, _userManagerMock.Object);

        // Act
        var result = await model.OnPostAddUsers();

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public async Task OnPostAddUsers_GroupNotFound_RedirectsToError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<UsersGroup, bool>>>(),
            It.IsAny<Func<IQueryable<UsersGroup>, IQueryable<UsersGroup>>?>()))
            .ReturnsAsync((UsersGroup?)null);

        var model = new GroupDetailsModel(_repositoryMock.Object, _userManagerMock.Object);
        model.GroupId = 2;

        // Act
        var result = await model.OnPostAddUsers();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostAddUsers_UserNotPermittedToAddUsers_RedirectsToError()
    {
        // Arrange
        var appUser = new ApplicationUser { Id = "userId", FirstName = "John", LastName = "Doe" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                        .ReturnsAsync(appUser);

        var other = new ApplicationUser { Id = "other", FirstName = "John", LastName = "Doe" };

        var group = new UsersGroup
        {
            Id = 1,
            Name = "Group",
            MasterId = "other2",
            Admins = { other }
        };
        _repositoryMock.Setup(repo => repo.GetAsync(
            It.IsAny<Expression<Func<UsersGroup, bool>>>(),
            It.IsAny<Func<IQueryable<UsersGroup>, IQueryable<UsersGroup>>?>()))
            .ReturnsAsync((UsersGroup?)group);

        var model = new GroupDetailsModel(_repositoryMock.Object, _userManagerMock.Object);
        model.GroupId = 2;

        // Act
        var result = await model.OnPostAddUsers();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Error", redirectResult.PageName);
    }

    [Fact]
    public async Task OnPostAddUsers_EmailsEmpty_RedirectsToError()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var memberUser1 = users[0];
            var memberUser2 = users[1];
            var adminUser1 = users[2];
            var adminUser2 = users[3];
            var masterUser = users[4];

            var group = new UsersGroup
            {
                Id = 1,
                Name = "Group",
                MasterId = masterUser.Id,
                Members = { memberUser1, memberUser2 },
                Admins = { adminUser1, adminUser2 }
            };
            context.UsersGroups.Add(group);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(adminUser1);
            var repository = new ApplicationRepository(context);

            var model = new GroupDetailsModel(repository, _userManagerMock.Object);
            model.GroupId = 1;
            model.EmailsAsString = "";

            //Act
            var result = await model.OnPostAddUsers();

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Error", redirectResult.PageName);
        }
    }

    [Fact]
    public async Task OnPostAddUsers_HasWrongEmails_ReturnsJsonResultWithWrongEmails()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var memberUser1 = users[0];
            var memberUser2 = users[1];
            var adminUser1 = users[2];
            var adminUser2 = users[3];
            var masterUser = users[4];
            var toAddUser = users[5];
            var requestSentUser = users[6];

            var group = new UsersGroup
            {
                Id = 1,
                Name = "Group",
                MasterId = masterUser.Id,
                Members = { memberUser1, memberUser2 },
                Admins = { adminUser1, adminUser2 }
            };
            var groupRequst = new GroupRequest
            {
                Id = 1,
                ForAdmin = true,
                UserId = requestSentUser.Id,
                GroupId = group.Id
            };

            context.UsersGroups.Add(group);
            context.GroupRequests.Add(groupRequst);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(adminUser1);
            var repository = new ApplicationRepository(context);

            var model = new GroupDetailsModel(repository, _userManagerMock.Object);
            model.GroupId = 1;
            model.AsAdmins = true;
            model.EmailsAsString = $"{toAddUser.Email},{requestSentUser.Email},{adminUser2.Email},notExisting@test.com";

            //Act
            var result = await model.OnPostAddUsers();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);

            var resultData = jsonResult.Value;
            var wrongEmailsProperty = resultData.GetType().GetProperty("wrongEmails");
            Assert.NotNull(wrongEmailsProperty);

            var actualWrongEmails = (List<string>)wrongEmailsProperty.GetValue(resultData);
            var expectedWrongEmails = new List<string> { "notExisting@test.com", adminUser2.Email, requestSentUser.Email };
            Assert.Equal(expectedWrongEmails, actualWrongEmails);
        }
    }

    [Fact]
    public async Task OnPostAddUsers_HasCorrectEmails_ReturnsJsonResultSuccess()
    {
        using (var context = new ApplicationDbContext(Utilities.GetInMemoryOptions()))
        {
            // Arrange
            var users = Seeding.GetSeedingUsers();
            context.Users.AddRange(users);
            var memberUser1 = users[0];
            var memberUser2 = users[1];
            var adminUser1 = users[2];
            var adminUser2 = users[3];
            var masterUser = users[4];
            var toAddUser = users[5];
            var requestSentUser = users[6];

            var group = new UsersGroup
            {
                Id = 1,
                Name = "Group",
                MasterId = masterUser.Id,
                Members = { memberUser1, memberUser2 },
                Admins = { adminUser1, adminUser2 }
            };
            var groupRequst = new GroupRequest
            {
                Id = 1,
                ForAdmin = true,
                UserId = requestSentUser.Id,
                GroupId = group.Id
            };

            context.UsersGroups.Add(group);
            context.GroupRequests.Add(groupRequst);
            await context.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(adminUser1);
            var repository = new ApplicationRepository(context);

            var model = new GroupDetailsModel(repository, _userManagerMock.Object);
            model.GroupId = 1;
            model.AsAdmins = true;
            model.EmailsAsString = $"{toAddUser.Email}";

            //Act
            var result = await model.OnPostAddUsers();

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            Assert.NotNull(jsonResult.Value);

            var resultData = jsonResult.Value;
            var successProperty = resultData.GetType().GetProperty("success");
            Assert.NotNull(successProperty);

            var groupRequests = await context.GroupRequests.ToListAsync();
            Assert.Equal(2, groupRequests.Count);
            Assert.Contains(toAddUser.Id, groupRequests.Select(g => g.UserId));


        }
    }

}