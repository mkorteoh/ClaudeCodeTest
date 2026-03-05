using InsuranceDMS.Domain.Common;
using InsuranceDMS.Domain.Enums;

namespace InsuranceDMS.Domain.Entities;

public class Personnel : SoftDeleteEntity
{
    public int AgencyId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? MiddleName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public PersonnelType PersonnelType { get; set; }
    public string? Title { get; set; }
    public DateTime? HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public bool IsActive { get; set; } = true;

    public Agency Agency { get; set; } = null!;
    public Producer? Producer { get; set; }
}
