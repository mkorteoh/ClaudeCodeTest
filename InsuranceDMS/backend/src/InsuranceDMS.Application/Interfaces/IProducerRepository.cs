using InsuranceDMS.Domain.Entities;

namespace InsuranceDMS.Application.Interfaces;

public interface IProducerRepository
{
    Task<(List<Producer> Items, int TotalCount)> GetListAsync(ProducerFilter filter, CancellationToken ct = default);
    Task<Producer?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Producer?> GetByPersonnelIdAsync(int personnelId, CancellationToken ct = default);
    Task<List<License>> GetLicensesAsync(int producerId, CancellationToken ct = default);
    Task<List<CarrierAppointment>> GetAppointmentsAsync(int producerId, CancellationToken ct = default);
    void Add(Producer producer);
    void Update(Producer producer);
}

public class ProducerFilter
{
    public int? AgencyId { get; set; }
    public string? State { get; set; }
    public string? Search { get; set; }
    public string? Npn { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
