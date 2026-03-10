using Microsoft.EntityFrameworkCore;
using Employee.Shared.Interfaces;
using System.Linq.Expressions;

namespace Employee.Shared.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    // Changed from AuthDbContext to the base DbContext
    protected readonly DbContext _db;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(DbContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }

    public T? GetById(int id)
    {
        return _dbSet.Find(id);
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet;
    }

    // Notice: ALL Save() calls have been removed from these methods!
    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public IQueryable<T> GetQueryableInclude(Expression<Func<T, object>>[] includes = null, string[] deepIncludes = null)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

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
        var query = _dbSet.AsNoTracking().Where(expression);

        if (includes != null)
        {
            query = includes(query);
        }

        return await query.FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<bool> Exists(Expression<Func<T, bool>> expression)
    {
        return await _dbSet.AsNoTracking().AnyAsync(expression);
    }

}