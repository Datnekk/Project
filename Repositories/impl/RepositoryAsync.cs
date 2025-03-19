using System.Linq.Expressions;
using be.Data;
using Microsoft.EntityFrameworkCore;

namespace be.Repositories.impl;

public class RepositoryAsync<T> : IRepositoryAsync<T> where T : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _dbSet;

    public RepositoryAsync(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<(IEnumerable<T> Data, int TotalCount)> GetAsync(
        Expression<Func<T, bool>> filter, 
        Func<IQueryable<T>, 
        IOrderedQueryable<T>> orderBy, 
        int? pageNumber = null, 
        int? pageSize = null, 
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if(filter != null){
            query = query.Where(filter);
        }

        int totalCount = await query.CountAsync(cancellationToken);

        if(orderBy != null){
            query = orderBy(query);
        }

        if(pageNumber.HasValue && pageSize.HasValue ){
            query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        var data = await query.ToListAsync(cancellationToken);
        return (data, totalCount);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if(entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken) 
               ?? throw new InvalidOperationException($"Entity with ID {id} not found.");
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        _dbSet.Update(entity);
        return _context.SaveChangesAsync(cancellationToken);
    }
}