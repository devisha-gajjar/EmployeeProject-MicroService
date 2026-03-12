using System.Linq.Expressions;
using Auth.Infrastructure.Data;
using Employee.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Repositories;

public class GenericRepository<T>(AuthDbContext db) : IGenericRepository<T> where T : class
{
    protected readonly AuthDbContext _db = db;

    public T? GetById(int id)
    {
        return _db.Set<T>().Find(id);
    }

    public IQueryable<T> GetAll()
    {
        return _db.Set<T>();
    }

    public void Add(T entity)
    {
        _db.Set<T>().Add(entity);
        Save();
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _db.Set<T>().AddRange(entities);
        Save();
    }

    public void Update(T entity)
    {
        _db.Set<T>().Update(entity);
        Save();
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _db.Set<T>().UpdateRange(entities);
        Save();
    }

    public void Save() => _db.SaveChanges();

    public void Delete(T entity)
    {
        _db.Set<T>().Remove(entity);
        Save();
    }

    public IQueryable<T> GetQueryableInclude(Expression<Func<T, object>>[] includes = null, string[] deepIncludes = null)
    {
        IQueryable<T> query = _db.Set<T>().AsNoTracking();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        if (deepIncludes != null)
        {
            foreach (var deepInclude in deepIncludes)
            {
                query = query.Include(deepInclude);
            }
        }

        return query;
    }

    public async Task<T?> GetByInclude(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IQueryable<T>>? includes = null)
    {
        var query = _db.Set<T>().AsNoTracking().Where(expression);

        if (includes != null)
        {
            query = includes(query);
        }

        return await query.FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<bool> Exists(Expression<Func<T, bool>> expression)
    {
        return await _db.Set<T>().AsNoTracking().Where(expression).AnyAsync();
    }
}
