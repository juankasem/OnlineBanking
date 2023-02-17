using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineBanking.Application.Contracts.Persistence;
using OnlineBanking.Application.Specifications;
using OnlineBanking.Core.Helpers.Params;
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
    public async Task<IReadOnlyList<T>> GetAllAsync(PaginationParams paginationParams)
    {
        var query = _dbContext.Set<T>().AsNoTracking();
        
         return await ApplyPagination(query, paginationParams.PageNumber, paginationParams.PageSize);
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, PaginationParams paginationParams)
    {
        var query = _dbContext.Set<T>().AsNoTracking()
                                        .Where(predicate);

        return await ApplyPagination(query, paginationParams.PageNumber, paginationParams.PageSize);

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

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<T> GetByIdAsync(Guid id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

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

    private async Task<IReadOnlyList<T>> ApplyPagination(IQueryable<T> query, int pageNumber, int pageSize) =>
         await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
}