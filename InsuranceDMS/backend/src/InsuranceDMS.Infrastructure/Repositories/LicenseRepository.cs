using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using InsuranceDMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InsuranceDMS.Infrastructure.Repositories;

public class LicenseRepository : ILicenseRepository
{
    private readonly AppDbContext _db;
    public LicenseRepository(AppDbContext db) => _db = db;

    public async Task<(List<License> Items, int TotalCount)> GetListAsync(LicenseFilter filter, CancellationToken ct = default)
    {
        var q = _db.Licenses.Include(l => l.LicenseType).Include(l => l.Producer).AsQueryable();

        if (filter.ProducerId.HasValue)
            q = q.Where(l => l.ProducerId == filter.ProducerId.Value);
        if (!string.IsNullOrWhiteSpace(filter.StateCode))
            q = q.Where(l => l.StateCode == filter.StateCode);
        if (filter.LicenseTypeId.HasValue)
            q = q.Where(l => l.LicenseTypeId == filter.LicenseTypeId.Value);
        if (filter.ExpiringBefore.HasValue)
            q = q.Where(l => l.ExpirationDate <= filter.ExpiringBefore.Value);
        if (filter.IsActive.HasValue)
            q = q.Where(l => l.IsActive == filter.IsActive.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(l => l.ExpirationDate)
            .Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<License?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Licenses.Include(l => l.LicenseType).Include(l => l.State).FirstOrDefaultAsync(l => l.Id == id, ct);

    public void Add(License license) => _db.Licenses.Add(license);
    public void Update(License license) => _db.Licenses.Update(license);
    public void Delete(License license) => _db.Licenses.Remove(license);
}
