using InsuranceDMS.Domain.Entities;

namespace InsuranceDMS.Application.Interfaces;

public interface IAppointmentRepository
{
    Task<(List<CarrierAppointment> Items, int TotalCount)> GetListAsync(AppointmentFilter filter, CancellationToken ct = default);
    Task<CarrierAppointment?> GetByIdAsync(int id, CancellationToken ct = default);
    void Add(CarrierAppointment appointment);
    void Update(CarrierAppointment appointment);
    void Delete(CarrierAppointment appointment);
}

public class AppointmentFilter
{
    public int? ProducerId { get; set; }
    public int? CarrierId { get; set; }
    public string? StateCode { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
