namespace InsuranceDMS.Application.DTOs;

public class AgencyLocationDto
{
    public int Id { get; set; }
    public int AgencyId { get; set; }
    public string LocationName { get; set; } = null!;
    public bool IsCorporateOffice { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? StateCode { get; set; }
    public string? ZipCode { get; set; }
    public string? County { get; set; }
    public bool IsActive { get; set; }
    public bool IsMerged { get; set; }
    public int? OriginalAgencyId { get; set; }
    public DateTime? AcquiredAt { get; set; }
    public int PersonnelCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAgencyLocationDto
{
    public int AgencyId { get; set; }
    public string LocationName { get; set; } = null!;
    public bool IsCorporateOffice { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? StateCode { get; set; }
    public string? ZipCode { get; set; }
    public string? County { get; set; }
}

public class UpdateAgencyLocationDto : CreateAgencyLocationDto
{
    public bool IsActive { get; set; }
}

public class PersonnelLocationDto
{
    public int Id { get; set; }
    public int PersonnelId { get; set; }
    public string PersonnelName { get; set; } = null!;
    public int AgencyLocationId { get; set; }
    public string? LocationName { get; set; }
    public DateTime AssignedDate { get; set; }
}

public class AssignPersonnelToLocationDto
{
    public int PersonnelId { get; set; }
    public DateTime AssignedDate { get; set; }
}

public class AbsorbedLocationPreviewDto
{
    public int LocationId { get; set; }
    public string LocationName { get; set; } = null!;
    public int OwningAgencyId { get; set; }
    public string OwningAgencyName { get; set; } = null!;
    public int PersonnelCount { get; set; }
}
