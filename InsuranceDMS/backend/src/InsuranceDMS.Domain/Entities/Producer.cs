using InsuranceDMS.Domain.Common;

namespace InsuranceDMS.Domain.Entities;

public class Producer : SoftDeleteEntity
{
    public int PersonnelId { get; set; }
    public string? NPN { get; set; }
    public string? ResidentState { get; set; }
    public string? SSNLast4 { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? EOExpirationDate { get; set; }

    public Personnel Personnel { get; set; } = null!;
    public ICollection<License> Licenses { get; set; } = new List<License>();
    public ICollection<CarrierAppointment> Appointments { get; set; } = new List<CarrierAppointment>();
    public State? ResidentStateNav { get; set; }
}
