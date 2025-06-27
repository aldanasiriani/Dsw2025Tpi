using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dsw2025Tpi.Data.Repositories;

public class EfRepository<T> : IRepository<T> where T : EntityBase
{
    private readonly Dsw2025TpiContext _context;
    private readonly DbSet<T> _dbSet;

    public EfRepository(Dsw2025TpiContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetById(Guid id, params string[] include)
    {
        return await Include(_context.Set<T>(), include).FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<T>?> GetAll(params string[] include)
    {
        return await Include(_context.Set<T>(), include).ToListAsync();
    }

    public async Task<T?> First(Expression<Func<T, bool>> predicate, params string[] include)
    {
        return await Include(_context.Set<T>(), include).FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<T>?> GetFiltered(Expression<Func<T, bool>> predicate, params string[] include)
    {
        return await Include(_context.Set<T>(), include).Where(predicate).ToListAsync();
    }

    public async Task<T> Add(T entity)
    {
        await _context.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> Update(T entity)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<T> Delete(T entity)
    {
        _context.Remove(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    private static IQueryable<T> Include(IQueryable<T> query, string[] includes)
    {
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return query;
    }
}