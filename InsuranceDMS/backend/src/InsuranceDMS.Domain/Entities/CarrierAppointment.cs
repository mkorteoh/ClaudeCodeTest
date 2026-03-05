using InsuranceDMS.Domain.Common;

namespace InsuranceDMS.Domain.Entities;

public class CarrierAppointment : BaseEntity
{
    public int ProducerId { get; set; }
    public int CarrierId { get; set; }
    public string StateCode { get; set; } = null!;
    public DateTime? AppointmentDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string AppointmentStatus { get; set; } = "Active";

    public Producer Producer { get; set; } = null!;
    public Carrier Carrier { get; set; } = null!;
    public State State { get; set; } = null!;
}
