using InsuranceDMS.Domain.Entities;

namespace InsuranceDMS.Application.Interfaces;

public interface IAgencyRepository
{
    Task<(List<Agency> Items, int TotalCount)> GetListAsync(AgencyFilter filter, CancellationToken ct = default);
    Task<Agency?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Agency>> GetHierarchyAsync(int id, CancellationToken ct = default);
    Task<List<Agency>> GetChildrenAsync(int id, CancellationToken ct = default);
    Task<bool> NpnExistsAsync(string npn, int? excludeId, CancellationToken ct = default);
    void Add(Agency agency);
    void Update(Agency agency);
    void Delete(Agency agency);
}

public class AgencyFilter
{
    public string? Search { get; set; }
    public byte? Tier { get; set; }
    public string? State { get; set; }
    public bool? IsActive { get; set; }
    public int? ParentId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
