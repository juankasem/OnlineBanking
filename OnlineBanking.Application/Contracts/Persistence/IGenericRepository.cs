
using System.Linq.Expressions;
using OnlineBanking.Application.Specifications;
using OnlineBanking.Core.Helpers.Params;

namespace OnlineBanking.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T: class
    {
        Task<(IReadOnlyList<T>, int)> GetAllAsync(PaginationParams paginationParams);
        Task<(IReadOnlyList<T>, int)> GetAsync(Expression<Func<T, bool>> predicate, PaginationParams paginationParams);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                        string includeString = null,
                                        bool disableTracking = true);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        List<Expression<Func<T, object>>> includes = null,
        bool disableTracking = true);      
        Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec);
        
        Task<T> GetEntityWithSpecAsync(ISpecification<T> spec);

        Task<T> GetByIdAsync(int id);
        Task<T> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> CountAsync(ISpecification<T> spec);
    }
}