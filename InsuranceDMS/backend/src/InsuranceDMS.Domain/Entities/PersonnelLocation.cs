using InsuranceDMS.Domain.Common;

namespace InsuranceDMS.Domain.Entities;

public class PersonnelLocation : BaseEntity
{
    public int PersonnelId { get; set; }
    public int AgencyLocationId { get; set; }
    public DateTime AssignedDate { get; set; }

    // Navigation
    public Personnel Personnel { get; set; } = null!;
    public AgencyLocation AgencyLocation { get; set; } = null!;
}
