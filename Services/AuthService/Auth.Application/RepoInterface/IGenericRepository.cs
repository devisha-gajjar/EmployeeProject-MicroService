using System.Linq.Expressions;

namespace Auth.Infrastructure.Interface;

public interface IGenericRepository<T> where T : class
{
    T? GetById(int id);
    IQueryable<T> GetAll();
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    void Update(T entity);
    public void UpdateRange(IEnumerable<T> entities);
    public void Delete(T entity);
    void Save();
    IQueryable<T> GetQueryableInclude(Expression<Func<T, object>>[] includes = null, string[] deepIncludes = null);
    Task<T?> GetByInclude(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IQueryable<T>>? includes = null);
    Task<bool> Exists(Expression<Func<T, bool>> expression);
}
