using InsuranceDMS.Domain.Entities;

namespace InsuranceDMS.Application.Interfaces;

public interface IMergerRepository
{
    Task<(List<Merger> Items, int TotalCount)> GetListAsync(MergerFilter filter, CancellationToken ct = default);
    Task<Merger?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<EntityLineage>> GetLineageAsync(int mergerId, CancellationToken ct = default);
    Task<List<EntityLineage>> GetAgencyLineageAsync(int agencyId, CancellationToken ct = default);
    void Add(Merger merger);
    void Update(Merger merger);
}

public class MergerFilter
{
    public string? Status { get; set; }
    public int? AgencyId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
