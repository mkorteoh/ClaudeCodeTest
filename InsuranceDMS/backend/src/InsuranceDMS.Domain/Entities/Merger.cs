using InsuranceDMS.Domain.Common;
using InsuranceDMS.Domain.Enums;

namespace InsuranceDMS.Domain.Entities;

public class Merger : BaseEntity
{
    public int SurvivingAgencyId { get; set; }
    public MergerStatus Status { get; set; } = MergerStatus.Draft;
    public string? InitiatedBy { get; set; }
    public DateTime InitiatedAt { get; set; }
    public DateTime? ExecutedAt { get; set; }
    public string? ExecutedBy { get; set; }
    public string? Notes { get; set; }

    public MergerType MergerType { get; set; } = MergerType.Agency;

    public Agency SurvivingAgency { get; set; } = null!;
    public ICollection<MergerParticipant> Participants { get; set; } = new List<MergerParticipant>();
    public ICollection<EntityLineage> Lineage { get; set; } = new List<EntityLineage>();
}
