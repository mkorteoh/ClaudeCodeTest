using InsuranceDMS.Domain.Common;

namespace InsuranceDMS.Domain.Entities;

public class License : BaseEntity
{
    public int ProducerId { get; set; }
    public string StateCode { get; set; } = null!;
    public int LicenseTypeId { get; set; }
    public string LicenseNumber { get; set; } = null!;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public DateTime? RenewalDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Producer Producer { get; set; } = null!;
    public State State { get; set; } = null!;
    public LicenseType LicenseType { get; set; } = null!;
}
