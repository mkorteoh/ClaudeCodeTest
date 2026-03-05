using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using InsuranceDMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InsuranceDMS.Infrastructure.Repositories;

public class PersonnelRepository : IPersonnelRepository
{
    private readonly AppDbContext _db;
    public PersonnelRepository(AppDbContext db) => _db = db;

    public async Task<(List<Personnel> Items, int TotalCount)> GetListAsync(PersonnelFilter filter, CancellationToken ct = default)
    {
        var q = _db.Personnel.Include(p => p.Agency).Include(p => p.Producer).AsQueryable();

        if (filter.AgencyId.HasValue)
            q = q.Where(p => p.AgencyId == filter.AgencyId.Value);
        if (filter.Type.HasValue)
            q = q.Where(p => (byte)p.PersonnelType == filter.Type.Value);
        if (!string.IsNullOrWhiteSpace(filter.Search))
            q = q.Where(p => p.FirstName.Contains(filter.Search) || p.LastName.Contains(filter.Search) || (p.Email != null && p.Email.Contains(filter.Search)));
        if (filter.IsActive.HasValue)
            q = q.Where(p => p.IsActive == filter.IsActive.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
            .Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Personnel?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Personnel
            .Include(p => p.Agency)
            .Include(p => p.Producer).ThenInclude(pr => pr!.Licenses).ThenInclude(l => l.LicenseType)
            .Include(p => p.Producer).ThenInclude(pr => pr!.Appointments).ThenInclude(a => a.Carrier)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<List<Personnel>> GetByAgencyAsync(int agencyId, CancellationToken ct = default) =>
        await _db.Personnel.Where(p => p.AgencyId == agencyId).Include(p => p.Producer).ToListAsync(ct);

    public void Add(Personnel personnel) => _db.Personnel.Add(personnel);
    public void Update(Personnel personnel) => _db.Personnel.Update(personnel);
    public void Delete(Personnel personnel) => _db.Personnel.Remove(personnel);
}
