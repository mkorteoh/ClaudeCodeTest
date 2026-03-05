using InsuranceDMS.Domain.Entities;

namespace InsuranceDMS.Application.Interfaces;

public interface ILicenseRepository
{
    Task<(List<License> Items, int TotalCount)> GetListAsync(LicenseFilter filter, CancellationToken ct = default);
    Task<License?> GetByIdAsync(int id, CancellationToken ct = default);
    void Add(License license);
    void Update(License license);
    void Delete(License license);
}

public class LicenseFilter
{
    public int? ProducerId { get; set; }
    public string? StateCode { get; set; }
    public int? LicenseTypeId { get; set; }
    public DateTime? ExpiringBefore { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
