using InsuranceDMS.Domain.Entities;

namespace InsuranceDMS.Application.Interfaces;

public interface IPersonnelRepository
{
    Task<(List<Personnel> Items, int TotalCount)> GetListAsync(PersonnelFilter filter, CancellationToken ct = default);
    Task<Personnel?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<List<Personnel>> GetByAgencyAsync(int agencyId, CancellationToken ct = default);
    void Add(Personnel personnel);
    void Update(Personnel personnel);
    void Delete(Personnel personnel);
}

public class PersonnelFilter
{
    public int? AgencyId { get; set; }
    public byte? Type { get; set; }
    public string? Search { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
