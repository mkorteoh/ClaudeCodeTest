using InsuranceDMS.Domain.Common;

namespace InsuranceDMS.Domain.Entities;

public class MergerParticipant : BaseEntity
{
    public int MergerId { get; set; }
    public int AbsorbedAgencyId { get; set; }
    public int PersonnelTransferred { get; set; }

    public Merger Merger { get; set; } = null!;
    public Agency AbsorbedAgency { get; set; } = null!;
}
