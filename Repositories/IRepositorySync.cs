using System.Linq.Expressions;

namespace be.Repositories;

public interface IRepositorySync<T> where T : class
{
    (IEnumerable<T> Data, int TotalCount) Get(
            Expression<Func<T, bool>> filter,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
            int? pageNumber = null,
            int? pageSize = null
        );
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(int id);
}