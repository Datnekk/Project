using System.Linq.Expressions;

namespace be.Repositories;

public interface IRepositoryAsync<T> where T : class
{
    Task<(IEnumerable<T> Data, int TotalCount)> GetAsync(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            int? pageNumber = null,
            int? pageSize = null,
            CancellationToken cancellationToken = default
        );
    Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}