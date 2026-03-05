using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using InsuranceDMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InsuranceDMS.Infrastructure.Repositories;

public class ProducerRepository : IProducerRepository
{
    private readonly AppDbContext _db;
    public ProducerRepository(AppDbContext db) => _db = db;

    public async Task<(List<Producer> Items, int TotalCount)> GetListAsync(ProducerFilter filter, CancellationToken ct = default)
    {
        var q = _db.Producers.Include(p => p.Personnel).ThenInclude(pr => pr.Agency).AsQueryable();

        if (filter.AgencyId.HasValue)
            q = q.Where(p => p.Personnel.AgencyId == filter.AgencyId.Value);
        if (!string.IsNullOrWhiteSpace(filter.State))
            q = q.Where(p => p.ResidentState == filter.State);
        if (!string.IsNullOrWhiteSpace(filter.Npn))
            q = q.Where(p => p.NPN == filter.Npn);
        if (!string.IsNullOrWhiteSpace(filter.Search))
            q = q.Where(p => p.Personnel.FirstName.Contains(filter.Search) || p.Personnel.LastName.Contains(filter.Search) || (p.NPN != null && p.NPN.Contains(filter.Search)));

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(p => p.Personnel.LastName)
            .Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<Producer?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.Producers
            .Include(p => p.Personnel).ThenInclude(pr => pr.Agency)
            .Include(p => p.Licenses).ThenInclude(l => l.LicenseType)
            .Include(p => p.Appointments).ThenInclude(a => a.Carrier)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Producer?> GetByPersonnelIdAsync(int personnelId, CancellationToken ct = default) =>
        await _db.Producers.FirstOrDefaultAsync(p => p.PersonnelId == personnelId, ct);

    public async Task<List<License>> GetLicensesAsync(int producerId, CancellationToken ct = default) =>
        await _db.Licenses.Where(l => l.ProducerId == producerId).Include(l => l.LicenseType).ToListAsync(ct);

    public async Task<List<CarrierAppointment>> GetAppointmentsAsync(int producerId, CancellationToken ct = default) =>
        await _db.CarrierAppointments.Where(a => a.ProducerId == producerId).Include(a => a.Carrier).ToListAsync(ct);

    public void Add(Producer producer) => _db.Producers.Add(producer);
    public void Update(Producer producer) => _db.Producers.Update(producer);
}
