using Microsoft.EntityFrameworkCore;
using Ordering.Application.Contracts;
using Ordering.Domain.Common;
using Ordering.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Ordering.Infrastructure.Repositories;

public class RepositoryBase<T> : IRepositoryBase<T> where T : EntityBase
{
    private readonly OrderContext orderContext;

    public RepositoryBase(OrderContext orderContext)
    {
        this.orderContext = orderContext;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
        => await orderContext.Set<T>().ToListAsync();

    public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        => await orderContext.Set<T>().Where(predicate).ToListAsync();

    public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, string? includeString = null)
    {
        IQueryable<T> query = orderContext.Set<T>();

        if (!string.IsNullOrEmpty(includeString)) query = query.Include(includeString);

        if (predicate is not null) query = query.Where(predicate);

        if (orderBy is not null) return await orderBy(query).ToListAsync();

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(int offset, int limit,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
       params string[] includeStrings)
    {
        IQueryable<T> query = orderContext.Set<T>();

        query = query.Skip(offset).Take(limit);

        foreach (var itemInclude in includeStrings)
            query = query.Include(itemInclude);

        if (predicate is not null) query = query.Where(predicate);

        if (orderBy is not null) return await orderBy(query).ToListAsync();

        return await query.ToListAsync();
    }


    public async Task<T?> GetById(int Id)
           => await orderContext.Set<T>().FindAsync(Id);

    public async Task<T> AddAsync(T entity)
    {
        await orderContext.Set<T>().AddAsync(entity);
        await orderContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {

        orderContext.Entry(entity).State = EntityState.Modified;
        await orderContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        orderContext.Set<T>().Remove(entity);
        await orderContext.SaveChangesAsync();

    }


}

