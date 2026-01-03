
namespace OnlineBanking.Core.Domain.Abstractions;

public abstract class Entity<T> : IAuditableEntity
{
    public T Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; }

    public DateTime LastModifiedOn { get; set; }

    public string LastModifiedBy { get; set; }

    public bool IsDeleted { get; set; }
}