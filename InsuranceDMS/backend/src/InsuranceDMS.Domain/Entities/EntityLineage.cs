using InsuranceDMS.Domain.Common;

namespace InsuranceDMS.Domain.Entities;

public class EntityLineage : BaseEntity
{
    public int MergerId { get; set; }
    public string EntityType { get; set; } = null!;
    public int SourceEntityId { get; set; }
    public int? TargetEntityId { get; set; }
    public int SourceAgencyId { get; set; }
    public int TargetAgencyId { get; set; }
    public string Action { get; set; } = "Transferred";
    public DateTime RecordedAt { get; set; }
    public string? RecordedBy { get; set; }

    public Merger Merger { get; set; } = null!;
    public Agency SourceAgency { get; set; } = null!;
    public Agency TargetAgency { get; set; } = null!;
}
