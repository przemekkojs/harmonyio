using System.Linq.Expressions;

namespace Main.Data;

public interface IRepository
{
    Task<List<T>> GetAllAsync<T>(Func<IQueryable<T>, IQueryable<T>>? modifier = null) where T : class;
    Task<T?> GetAsync<T>(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IQueryable<T>>? modifier = null) where T : class;
    void Add<T>(T value) where T : class;
    void Update<T>(T value) where T : class;
    void Delete<T>(T value) where T : class;
    Task SaveChangesAsync();
}