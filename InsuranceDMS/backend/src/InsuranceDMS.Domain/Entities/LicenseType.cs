namespace InsuranceDMS.Domain.Entities;

public class LicenseType
{
    public int LicenseTypeId { get; set; }
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
}
