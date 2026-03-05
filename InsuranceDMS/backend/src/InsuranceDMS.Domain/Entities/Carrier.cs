using InsuranceDMS.Domain.Common;

namespace InsuranceDMS.Domain.Entities;

public class Carrier : AuditableEntity
{
    public new int Id { get; set; }
    public string CarrierName { get; set; } = null!;
    public string? NAIC { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<CarrierAppointment> Appointments { get; set; } = new List<CarrierAppointment>();
}
