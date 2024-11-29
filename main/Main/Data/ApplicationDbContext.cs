using System.Text.Json;
using Main.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Main.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<ExerciseResult> ExerciseResults { get; set; }
    public DbSet<ExerciseSolution> ExerciseSolutions { get; set; }
    public DbSet<Quiz> Quizes { get; set; }
    public DbSet<QuizResult> QuizResults { get; set; }
    public DbSet<UsersGroup> UsersGroups { get; set; }
    public DbSet<GroupRequest> GroupRequests { get; set; }
    public DbSet<MistakeResult> MistakeResults { get; set; }
    public DbSet<QuizRequest> QuizRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relacja jeden-do-wielu między ApplicationUser a Quiz (twórca quizu)
        modelBuilder.Entity<Quiz>()
            .HasOne(q => q.Creator)
            .WithMany(u => u.CreatedQuizes)
            .HasForeignKey(q => q.CreatorId)
            .OnDelete(DeleteBehavior.NoAction); // Usuń quizy przy usunięciu użytkownika

        // Relacja wiele-do-wielu między Quiz a ApplicationUser (uczestnicy)
        modelBuilder.Entity<Quiz>()
            .HasMany(q => q.Participants)
            .WithMany(u => u.ParticipatedQuizes)
            .UsingEntity<Dictionary<string, object>>(
                "QuizParticipants",
                j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey("ParticipantId").OnDelete(DeleteBehavior.NoAction),
                j => j.HasOne<Quiz>().WithMany().HasForeignKey("QuizId").OnDelete(DeleteBehavior.NoAction)
            );

        // Relacja wiele-do-wielu między Quiz a Group (grupy przypisane do quizu)
        modelBuilder.Entity<Quiz>()
            .HasMany(q => q.PublishedToGroup)
            .WithMany(u => u.Quizzes)
            .UsingEntity<Dictionary<string, object>>(
                "PublishedToQuizzes",
                j => j.HasOne<UsersGroup>().WithMany().HasForeignKey("GroupId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Quiz>().WithMany().HasForeignKey("QuizId").OnDelete(DeleteBehavior.NoAction)
            );

        // Relacja jeden-do-wielu między UsersGroup a ApplicationUser (założyciel)
        modelBuilder.Entity<UsersGroup>()
            .HasOne(g => g.MasterUser)
            .WithMany(u => u.MasterInGroups)
            .HasForeignKey(g => g.MasterId)
            .OnDelete(DeleteBehavior.NoAction); // Nie usuwaj grupy przy usunięciu założyciela 

        // Relacja jeden-do-wielu między Quiz a Exercise
        modelBuilder.Entity<Exercise>()
            .HasOne(e => e.Quiz)
            .WithMany(q => q.Exercises)
            .HasForeignKey(e => e.QuizId)
            .OnDelete(DeleteBehavior.Cascade); // Usuń ćwiczenia przy usunięciu quizu

        // Relacja jeden-do-wielu między ExerciseSolution a Exercise
        modelBuilder.Entity<ExerciseSolution>()
            .HasOne(es => es.Exercise)
            .WithMany(e => e.ExerciseSolutions)
            .HasForeignKey(es => es.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict); // Usuń rozwiązania przy usunięciu ćwiczenia

        // Relacja jeden-do-wielu między ExerciseSolution a ApplicationUser
        modelBuilder.Entity<ExerciseSolution>()
            .HasOne(es => es.User)
            .WithMany(u => u.ExerciseSolutions)
            .HasForeignKey(es => es.UserId)
            .OnDelete(DeleteBehavior.NoAction); // Usuń rozwiązania przy usunięciu użytkownika

        // Relacja jeden-do-jednego między ExerciseSolution a ExerciseResult
        modelBuilder.Entity<ExerciseSolution>()
            .HasOne(es => es.ExerciseResult)
            .WithOne(er => er.ExerciseSolution)
            .HasForeignKey<ExerciseResult>(er => er.ExerciseSolutionId)
            .OnDelete(DeleteBehavior.Cascade); // Usuń wynik przy usunięciu rozwiązania

        // Relacja jeden-do-wielu między QuizResult a Quiz
        modelBuilder.Entity<QuizResult>()
            .HasOne(qr => qr.Quiz)
            .WithMany(q => q.QuizResults)
            .HasForeignKey(qr => qr.QuizId)
            .OnDelete(DeleteBehavior.NoAction); // Usuń wyniki przy usunięciu quizu

        // Relacja jeden-do-wielu między QuizResult a ApplicationUser
        modelBuilder.Entity<QuizResult>()
            .HasOne(qr => qr.User)
            .WithMany(u => u.QuizResults)
            .HasForeignKey(qr => qr.UserId)
            .OnDelete(DeleteBehavior.NoAction); // Usuń wyniki przy usunięciu użytkownika

        // Relacja jeden-do-wielu między QuizResult a ExerciseResult
        modelBuilder.Entity<ExerciseResult>()
            .HasOne(er => er.QuizResult)
            .WithMany(qr => qr.ExerciseResults)
            .HasForeignKey(er => er.QuizResultId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacja wiele-do-wielu między UserGroup a ApplicationUser (uczestnicy) [oba]
        modelBuilder.Entity<ApplicationUser>()
            .HasMany(q => q.TeacherInGroups)
            .WithMany(u => u.Teachers)
            .UsingEntity<Dictionary<string, object>>(
                "GroupLeader",
                j => j.HasOne<UsersGroup>().WithMany().HasForeignKey("GroupId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.NoAction)
            );

        modelBuilder.Entity<ApplicationUser>()
            .HasMany(q => q.StudentInGroups)
            .WithMany(u => u.Students)
            .UsingEntity<Dictionary<string, object>>(
                "UserGroup",
                j => j.HasOne<UsersGroup>().WithMany().HasForeignKey("GroupId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.NoAction)
            );

        // Relacja jeden-do-wielu między GroupRequest a ApplicationUser
        modelBuilder.Entity<GroupRequest>()
            .HasOne(gr => gr.User)
            .WithMany(u => u.GroupRequests)
            .HasForeignKey(gr => gr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacja jeden-do-wielu między GroupRequest a Group
        modelBuilder.Entity<GroupRequest>()
            .HasOne(gr => gr.Group)
            .WithMany(g => g.Requests)
            .HasForeignKey(gr => gr.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacja jeden-do-wielu między QuizRequest a ApplicationUser
        modelBuilder.Entity<QuizRequest>()
            .HasOne(qr => qr.User)
            .WithMany(u => u.QuizRequests)
            .HasForeignKey(qr => qr.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacja jeden-do-wielu między QuizRequest a Quiz
        modelBuilder.Entity<QuizRequest>()
            .HasOne(gr => gr.Quiz)
            .WithMany(q => q.Requests)
            .HasForeignKey(gr => gr.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacja jeden-do-wielu między ExerciseResult a MistakeResult
        modelBuilder.Entity<MistakeResult>()
            .HasOne(mr => mr.ExerciseResult)
            .WithMany(er => er.MistakeResults)
            .HasForeignKey(mr => mr.ExerciseResultId)
                .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}