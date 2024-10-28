using System.Linq.Expressions;
using Main.Models;
using Microsoft.EntityFrameworkCore;

namespace Main.Data;

public class ApplicationRepository : IRepository
{
    private readonly ApplicationDbContext context;
    public ApplicationDbContext Context => context;

    public ApplicationRepository(ApplicationDbContext context)
    {
        this.context = context;
    }
    
    public async Task<List<T>> GetAllAsync<T>(Func<IQueryable<T>, IQueryable<T>>? modifier = null) where T : class
    {
        var set = context.Set<T>();
        var modifiedSet = modifier is null ? set : modifier(set);
        return await modifiedSet.ToListAsync();
    }

    public async Task<T?> GetAsync<T>(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>>? modifier = null) where T : class
    {
        var set = context.Set<T>();
        var modifiedSet = modifier is null ? set : modifier(set);


        return await modifiedSet.FirstOrDefaultAsync(filter);
    }

    public void Add<T>(T value) where T : class
    {
        context.Set<T>().Add(value);
    }

    public void Update<T>(T value) where T : class
    {
        context.Set<T>().Update(value);
    }

    public void Delete<T>(T value) where T : class
    {
        context.Set<T>().Remove(value);
    }

    public void Clear()
    {
        context.ExcersiseResults.RemoveRange(context.ExcersiseResults);
        context.ExcersiseSolutions.RemoveRange(context.ExcersiseSolutions);
        context.Excersises.RemoveRange(context.Excersises);
        context.QuizResults.RemoveRange(context.QuizResults);
        context.Quizes.RemoveRange(context.Quizes);
        context.Users.RemoveRange(context.Users);
        context.UsersGroups.RemoveRange(context.UsersGroups);
        context.GroupRequests.RemoveRange(context.GroupRequests);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
    
}