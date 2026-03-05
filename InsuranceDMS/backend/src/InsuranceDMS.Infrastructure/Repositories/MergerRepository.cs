using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using InsuranceDMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InsuranceDMS.Infrastructure.Repositories;

public class MergerRepository : IMergerRepository
{
    private readonly AppDbContext _db;
    public MergerRepository(AppDbContext db) => _db = db;

    public async Task<(List<Merger> Items, int TotalCount)> GetListAsync(MergerFilter filter, CancellationToken ct = default)
    {
        var q = _db.Mergers.Include(m => m.SurvivingAgency).Include(m => m.Participants).AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Status))
            q = q.Where(m => m.Status.ToString() == filter.Status);
        if (filter.AgencyId.HasValue)
            q = q.Where(m => m.SurvivingAgencyId == filter.AgencyId.Value ||
                m.Participants.Any(p => p.AbsorbedAgencyId == filter.AgencyId.Value));

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(m => m.InitiatedAt)
            .Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Merger?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Mergers
            .Include(m => m.SurvivingAgency)
            .Include(m => m.Participants).ThenInclude(p => p.AbsorbedAgency)
            .FirstOrDefaultAsync(m => m.Id == id, ct);

    public async Task<List<EntityLineage>> GetLineageAsync(int mergerId, CancellationToken ct = default) =>
        await _db.EntityLineages
            .Include(l => l.SourceAgency).Include(l => l.TargetAgency)
            .Where(l => l.MergerId == mergerId)
            .ToListAsync(ct);

    public async Task<List<EntityLineage>> GetAgencyLineageAsync(int agencyId, CancellationToken ct = default) =>
        await _db.EntityLineages
            .Include(l => l.SourceAgency).Include(l => l.TargetAgency)
            .Where(l => l.SourceAgencyId == agencyId || l.TargetAgencyId == agencyId)
            .ToListAsync(ct);

    public void Add(Merger merger) => _db.Mergers.Add(merger);
    public void Update(Merger merger) => _db.Mergers.Update(merger);
}
