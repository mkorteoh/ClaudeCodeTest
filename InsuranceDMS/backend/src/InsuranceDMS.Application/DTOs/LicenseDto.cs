namespace InsuranceDMS.Application.DTOs;

public class LicenseDto
{
    public int Id { get; set; }
    public int ProducerId { get; set; }
    public string StateCode { get; set; } = null!;
    public int LicenseTypeId { get; set; }
    public string? LicenseTypeCode { get; set; }
    public string LicenseNumber { get; set; } = null!;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? RenewalDate { get; set; }
    public bool IsActive { get; set; }
}

public class CreateLicenseDto
{
    public int ProducerId { get; set; }
    public string StateCode { get; set; } = null!;
    public int LicenseTypeId { get; set; }
    public string LicenseNumber { get; set; } = null!;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? RenewalDate { get; set; }
}

public class UpdateLicenseDto : CreateLicenseDto
{
    public bool IsActive { get; set; }
}
