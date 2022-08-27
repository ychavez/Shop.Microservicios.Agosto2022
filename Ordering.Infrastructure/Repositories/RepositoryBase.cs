using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Common;
using Ordering.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Ordering.Infrastructure.Repositories;

public class RepositoryBase<T> where T : EntityBase
{
    private readonly OrderContext orderContext;

    public RepositoryBase(OrderContext orderContext)
    {
        this.orderContext = orderContext;
    }


    /// <summary>
    /// Nos trae todos los registros, equivalente a select * from
    /// </summary>
    /// <returns></returns>
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


    //Single              --- select top 2 * from mi_tabla where id = 1 //error si hay mas de uno y espera no null
    //SingleorDefault     --- select top 2 * from mi_tabla where id = 1 //error si hay mas de uno 
    //First               --- select top 1 tiene que haber datos
    //FirstOrDefault        --- select top 1 
    //Find                 --- nos trae el Id select * from datos where id= 1

}

