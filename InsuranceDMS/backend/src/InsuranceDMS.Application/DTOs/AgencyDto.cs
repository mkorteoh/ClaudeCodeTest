using InsuranceDMS.Domain.Enums;

namespace InsuranceDMS.Application.DTOs;

public class AgencyDto
{
    public int Id { get; set; }
    public string AgencyName { get; set; } = null!;
    public string? NPN { get; set; }
    public string? TaxId { get; set; }
    public AgencyTier AgencyTier { get; set; }
    public int? ParentAgencyId { get; set; }
    public string? ParentAgencyName { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
    public bool IsMerged { get; set; }
    public int? MergedIntoId { get; set; }
    public DateTime? MergedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? CorporateStateCode { get; set; }
    public List<AgencyLocationDto> Locations { get; set; } = new();
}

public class AgencySummaryDto
{
    public int Id { get; set; }
    public string AgencyName { get; set; } = null!;
    public string? NPN { get; set; }
    public AgencyTier AgencyTier { get; set; }
    public bool IsActive { get; set; }
    public bool IsMerged { get; set; }
    public string? CorporateStateCode { get; set; }
    public int? ParentAgencyId { get; set; }
}

public class CreateAgencyDto
{
    public string AgencyName { get; set; } = null!;
    public string? NPN { get; set; }
    public string? TaxId { get; set; }
    public AgencyTier AgencyTier { get; set; }
    public int? ParentAgencyId { get; set; }
    public string? Notes { get; set; }
    public CreateAgencyLocationDto? InitialLocation { get; set; }
}

public class UpdateAgencyDto : CreateAgencyDto
{
    public bool IsActive { get; set; }
}

public class AgencyHierarchyDto
{
    public int Id { get; set; }
    public string AgencyName { get; set; } = null!;
    public AgencyTier AgencyTier { get; set; }
    public int? ParentAgencyId { get; set; }
    public List<AgencyHierarchyDto> Children { get; set; } = new();
}
