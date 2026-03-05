namespace InsuranceDMS.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "system";
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}

public abstract class SoftDeleteEntity : BaseEntity
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}

public abstract class AuditableEntity : SoftDeleteEntity
{
    public byte[]? RowVersion { get; set; }
}
