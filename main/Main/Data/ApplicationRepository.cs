using System.Linq.Expressions;
using Main.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Main.Data;

public class ApplicationRepository : IRepository
{
    public ApplicationDbContext Context => context;

    private readonly ApplicationDbContext context;

    public ApplicationRepository(ApplicationDbContext context)
    {
        this.context = context;
    }

    public virtual async Task<List<T>> GetAllAsync<T>(Func<IQueryable<T>, IQueryable<T>>? modifier = null) where T : class
    {
        var set = context.Set<T>();
        var modifiedSet = modifier is null ? set : modifier(set);
        return await modifiedSet.ToListAsync();
    }

    public virtual async Task<T?> GetAsync<T>(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>>? modifier = null) where T : class
    {
        var set = context.Set<T>();
        var modifiedSet = modifier is null ? set : modifier(set);


        return await modifiedSet.FirstOrDefaultAsync(filter);
    }

    public virtual void Add<T>(T value) where T : class
    {
        context.Set<T>().Add(value);
    }

    public virtual void Update<T>(T value) where T : class
    {
        context.Set<T>().Update(value);
    }

    public virtual void Delete<T>(T value) where T : class
    {
        context.Set<T>().Remove(value);
    }

    public virtual void Clear()
    {
        context.ExerciseResults.RemoveRange(context.ExerciseResults);
        context.ExerciseSolutions.RemoveRange(context.ExerciseSolutions);
        context.Exercises.RemoveRange(context.Exercises);
        context.QuizResults.RemoveRange(context.QuizResults);
        context.Quizes.RemoveRange(context.Quizes);
        context.Users.RemoveRange(context.Users);
        context.UsersGroups.RemoveRange(context.UsersGroups);
        context.GroupRequests.RemoveRange(context.GroupRequests);
        context.MistakeResults.RemoveRange(context.MistakeResults);
    }

    public virtual async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}