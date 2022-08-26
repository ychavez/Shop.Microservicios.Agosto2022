namespace Ordering.Domain.Common;

public interface IMultitenant
{
    public Guid TenantId { get; set; }
}

