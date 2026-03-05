namespace InsuranceDMS.Application.DTOs;

public class ProducerDto
{
    public int Id { get; set; }
    public int PersonnelId { get; set; }
    public string? NPN { get; set; }
    public string? ResidentState { get; set; }
    public string? SSNLast4 { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? EOExpirationDate { get; set; }
    public List<LicenseDto>? Licenses { get; set; }
    public List<AppointmentDto>? Appointments { get; set; }
}

public class UpdateProducerDto
{
    public string? NPN { get; set; }
    public string? ResidentState { get; set; }
    public string? SSNLast4 { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime? EOExpirationDate { get; set; }
}
