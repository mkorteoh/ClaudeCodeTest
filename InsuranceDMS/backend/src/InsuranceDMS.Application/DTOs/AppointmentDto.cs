namespace InsuranceDMS.Application.DTOs;

public class AppointmentDto
{
    public int Id { get; set; }
    public int ProducerId { get; set; }
    public int CarrierId { get; set; }
    public string? CarrierName { get; set; }
    public string StateCode { get; set; } = null!;
    public DateTime? AppointmentDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public string AppointmentStatus { get; set; } = null!;
}

public class CreateAppointmentDto
{
    public int ProducerId { get; set; }
    public int CarrierId { get; set; }
    public string StateCode { get; set; } = null!;
    public DateTime? AppointmentDate { get; set; }
    public string AppointmentStatus { get; set; } = "Active";
}

public class UpdateAppointmentDto : CreateAppointmentDto
{
    public DateTime? TerminationDate { get; set; }
}
