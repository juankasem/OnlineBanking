using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Helpers.Params;
using OnlineBanking.Application.Specifications;
using OnlineBanking.Infrastructure.Persistence;
using OnlineBanking.Infrastructure.Repositories.Base;

namespace OnlineBanking.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly OnlineBankDbContext _dbContext;

    public GenericRepository(OnlineBankDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    
    public async Task<(IReadOnlyList<T>, int)> GetAllAsync(PaginationParams paginationParams)
    {
        var query = _dbContext.Set<T>().AsQueryable();
        var totalCount = await query.CountAsync();

       var items = await ApplyPagination(query, paginationParams.PageNumber, paginationParams.PageSize);

      return (items, totalCount);
    }

    public async Task<(IReadOnlyList<T>, int)> GetAsync(Expression<Func<T, bool>> predicate, PaginationParams paginationParams)
    {
        var query = _dbContext.Set<T>()
                              .Where(predicate)
                              .AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await ApplyPagination(query, paginationParams.PageNumber, paginationParams.PageSize);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true)
    {
        IQueryable<T> query = _dbContext.Set<T>();
        if (disableTracking) query = query.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);

        if (predicate != null) query = query.Where(predicate);

        if (orderBy != null)
            return await orderBy(query).ToListAsync();
        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
    {
        IQueryable<T> query = _dbContext.Set<T>();
        if (disableTracking) query = query.AsNoTracking();

        if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

        if (predicate != null) query = query.Where(predicate);

        if (orderBy != null)
            return await orderBy(query).ToListAsync();
        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).ToListAsync();
    }

    public async Task<T> GetEntityWithSpecAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).FirstOrDefaultAsync();
    }

    public async Task<T> GetByIdAsync(int id) => await _dbContext.Set<T>().FindAsync(id);
    

    public async Task<T> GetByIdAsync(Guid id) => await _dbContext.Set<T>().FindAsync(id);
    

    public async Task AddAsync(T entity) => await _dbContext.Set<T>().AddAsync(entity);

    public void Add(T entity) => _dbContext.Set<T>().Add(entity);

    public void Update(T entity)
    {
        _dbContext.Set<T>().Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
    }

    public void Delete(T entity) => _dbContext.Set<T>().Remove(entity);
    
    public async Task<int> CountAsync(ISpecification<T> spec)
    {
        return await ApplySpecification(spec).CountAsync();
    }

    private IQueryable<T> ApplySpecification(ISpecification<T> spec)
    {
        return SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>().AsQueryable(), spec);
    }

    public async Task<IReadOnlyList<T>> ApplyPagination(IQueryable<T> query, int pageNumber, int pageSize) =>
         await query.Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .AsNoTracking()
                    .ToListAsync();
}