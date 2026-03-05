using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using InsuranceDMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InsuranceDMS.Infrastructure.Repositories;

public class AgencyRepository : IAgencyRepository
{
    private readonly AppDbContext _db;
    public AgencyRepository(AppDbContext db) => _db = db;

    public async Task<(List<Agency> Items, int TotalCount)> GetListAsync(AgencyFilter filter, CancellationToken ct = default)
    {
        var q = _db.Agencies.Include(a => a.ParentAgency).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            q = q.Where(a => a.AgencyName.Contains(filter.Search) || (a.NPN != null && a.NPN.Contains(filter.Search)));
        if (filter.Tier.HasValue)
            q = q.Where(a => (byte)a.AgencyTier == filter.Tier.Value);
        if (!string.IsNullOrWhiteSpace(filter.State))
            q = q.Where(a => a.StateCode == filter.State);
        if (filter.IsActive.HasValue)
            q = q.Where(a => a.IsActive == filter.IsActive.Value);
        if (filter.ParentId.HasValue)
            q = q.Where(a => a.ParentAgencyId == filter.ParentId.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(a => a.AgencyName)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Agency?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Agencies
            .Include(a => a.ParentAgency)
            .Include(a => a.ChildAgencies)
            .Include(a => a.State)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<List<Agency>> GetHierarchyAsync(int id, CancellationToken ct = default)
    {
        // Load all agencies and build tree in memory
        var all = await _db.Agencies.ToListAsync(ct);
        var result = new List<Agency>();
        // Ancestors
        var current = all.FirstOrDefault(a => a.Id == id);
        while (current?.ParentAgencyId != null)
        {
            current = all.FirstOrDefault(a => a.Id == current.ParentAgencyId);
            if (current != null) result.Insert(0, current);
        }
        // Self
        var self = all.FirstOrDefault(a => a.Id == id);
        if (self != null) result.Add(self);
        // Descendants (BFS)
        var queue = new Queue<int>(new[] { id });
        while (queue.Count > 0)
        {
            var parentId = queue.Dequeue();
            var children = all.Where(a => a.ParentAgencyId == parentId).ToList();
            foreach (var child in children)
            {
                result.Add(child);
                queue.Enqueue(child.Id);
            }
        }
        return result;
    }

    public async Task<List<Agency>> GetChildrenAsync(int id, CancellationToken ct = default) =>
        await _db.Agencies.Where(a => a.ParentAgencyId == id).ToListAsync(ct);

    public async Task<bool> NpnExistsAsync(string npn, int? excludeId, CancellationToken ct = default) =>
        await _db.Agencies.AnyAsync(a => a.NPN == npn && (!excludeId.HasValue || a.Id != excludeId.Value), ct);

    public void Add(Agency agency) => _db.Agencies.Add(agency);
    public void Update(Agency agency) => _db.Agencies.Update(agency);
    public void Delete(Agency agency) => _db.Agencies.Remove(agency);
}
