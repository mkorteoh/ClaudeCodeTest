using InsuranceDMS.Domain.Common;

namespace InsuranceDMS.Domain.Entities;

public class AgencyLocation : AuditableEntity
{
    public int AgencyId { get; set; }
    public string LocationName { get; set; } = null!;
    public bool IsCorporateOffice { get; set; }

    // Contact
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    // Address
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? StateCode { get; set; }
    public string? ZipCode { get; set; }
    public string? County { get; set; }

    // Status
    public bool IsActive { get; set; } = true;

    // M&A
    public bool IsMerged { get; set; }
    public int? OriginalAgencyId { get; set; }
    public DateTime? AcquiredAt { get; set; }

    // Navigation
    public Agency Agency { get; set; } = null!;
    public State? StateNav { get; set; }
    public ICollection<PersonnelLocation> PersonnelLocations { get; set; } = new List<PersonnelLocation>();
}
