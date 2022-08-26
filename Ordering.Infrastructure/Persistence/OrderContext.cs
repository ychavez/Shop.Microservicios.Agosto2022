using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;
using System.Linq.Expressions;
using System.Reflection;

namespace Ordering.Infrastructure.Persistence;

public class OrderContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public Guid TenantId { get; private set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {

        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedBy = "User";
                    break;
                case EntityState.Modified:
                    entry.Entity.lastModifiedDate = DateTime.UtcNow;
                    entry.Entity.LastmodifiedBy = "User";
                    break;
            }
        }

        foreach (var entry in ChangeTracker.Entries().Where(x =>
        x.State == EntityState.Added && x.Entity is IMultitenant))
        {
            var entity = entry.Entity as IMultitenant;

            entity!.TenantId = Guid.Parse("92517AA9-B9B9-4D91-8B22-EEFA7A3560EE");

        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var entityType = entity.ClrType;

            if (!typeof(IMultitenant).IsAssignableFrom(entityType)) continue;

            var method = typeof(OrderContext).GetMethod(nameof(MultitenantExpression),
                BindingFlags.NonPublic | BindingFlags.Static)?.MakeGenericMethod(entityType);

            var filter = method?.Invoke(null, new object[] { this })!;

            entity.SetQueryFilter((LambdaExpression)filter);

            entity.AddIndex(entity.FindProperty(nameof(IMultitenant.TenantId))!);

        }

    }

    private static LambdaExpression MultitenantExpression<T>
        (OrderContext context) where T : EntityBase, IMultitenant
    {
        Expression<Func<T, bool>> tenantFilter = x => x.TenantId == context.TenantId;
       
        return tenantFilter;
    }


}

