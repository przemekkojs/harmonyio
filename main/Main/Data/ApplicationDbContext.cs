using Main.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Main.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Excersise> Excersises { get; set; }
    public DbSet<ExcersiseResult> ExcersiseResults { get; set; }
    public DbSet<ExcersiseSolution> ExcersiseSolutions { get; set; }
    public DbSet<Quiz> Quizes { get; set; }
    public DbSet<QuizResult> QuizResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relacja jeden-do-wielu między ApplicationUser a Quiz (twórca quizu)
        modelBuilder.Entity<Quiz>()
            .HasOne(q => q.Creator)
            .WithMany(u => u.CreatedQuizes)
            .HasForeignKey(q => q.CreatorId)
            .OnDelete(DeleteBehavior.Cascade); // Usuń quizy przy usunięciu użytkownika

        // Relacja wiele-do-wielu między Quiz a ApplicationUser (uczestnicy)
        modelBuilder.Entity<Quiz>()
            .HasMany(q => q.Participants)
            .WithMany(u => u.ParticipatedQuizes)
            .UsingEntity(j => j.ToTable("QuizParticipants"));

        // Relacja jeden-do-wielu między Quiz a Excersise
        modelBuilder.Entity<Excersise>()
            .HasOne(e => e.Quiz)
            .WithMany(q => q.Excersises)
            .HasForeignKey(e => e.QuizId)
            .OnDelete(DeleteBehavior.Cascade); // Usuń ćwiczenia przy usunięciu quizu

        // Relacja jeden-do-wielu między ExcersiseSolution a Excersise
        modelBuilder.Entity<ExcersiseSolution>()
            .HasOne(es => es.Excersise)
            .WithMany(e => e.ExcersiseSolutions)
            .HasForeignKey(es => es.ExcersiseId)
            .OnDelete(DeleteBehavior.Cascade); // Usuń rozwiązania przy usunięciu ćwiczenia

        // Relacja jeden-do-wielu między ExcersiseSolution a ApplicationUser
        modelBuilder.Entity<ExcersiseSolution>()
            .HasOne(es => es.User)
            .WithMany(u => u.ExcersiseSolutions)
            .HasForeignKey(es => es.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Usuń rozwiązania przy usunięciu użytkownika

        // Relacja jeden-do-jednego między ExcersiseSolution a ExcersiseResult
        modelBuilder.Entity<ExcersiseSolution>()
            .HasOne(es => es.ExcersiseResult)
            .WithOne(er => er.ExcersiseSolution)
            .HasForeignKey<ExcersiseResult>(er => er.ExcersiseSolutionId)
            .OnDelete(DeleteBehavior.Cascade); // Usuń wynik przy usunięciu rozwiązania

        // Relacja jeden-do-wielu między QuizResult a Quiz
        modelBuilder.Entity<QuizResult>()
            .HasOne(qr => qr.Quiz)
            .WithMany(q => q.QuizResults)
            .HasForeignKey(qr => qr.QuizId)
            .OnDelete(DeleteBehavior.Cascade); // Usuń wyniki przy usunięciu quizu

        // Relacja jeden-do-wielu między QuizResult a ApplicationUser
        modelBuilder.Entity<QuizResult>()
            .HasOne(qr => qr.User)
            .WithMany(u => u.QuizResults)
            .HasForeignKey(qr => qr.UserId)
            .OnDelete(DeleteBehavior.Cascade); // Usuń wyniki przy usunięciu użytkownika
    }
}