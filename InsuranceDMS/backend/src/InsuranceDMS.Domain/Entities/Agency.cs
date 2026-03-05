using InsuranceDMS.Domain.Common;
using InsuranceDMS.Domain.Enums;

namespace InsuranceDMS.Domain.Entities;

public class Agency : AuditableEntity
{
    public string AgencyName { get; set; } = null!;
    public string? NPN { get; set; }
    public string? TaxId { get; set; }
    public AgencyTier AgencyTier { get; set; }
    public int? ParentAgencyId { get; set; }

    // Contact
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }

    // Location
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? StateCode { get; set; }
    public string? ZipCode { get; set; }
    public string? County { get; set; }

    // Status
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // M&A
    public bool IsMerged { get; set; }
    public int? MergedIntoId { get; set; }
    public DateTime? MergedAt { get; set; }

    // Navigation
    public Agency? ParentAgency { get; set; }
    public ICollection<Agency> ChildAgencies { get; set; } = new List<Agency>();
    public Agency? MergedIntoAgency { get; set; }
    public ICollection<Personnel> Personnel { get; set; } = new List<Personnel>();
    public State? State { get; set; }
}
