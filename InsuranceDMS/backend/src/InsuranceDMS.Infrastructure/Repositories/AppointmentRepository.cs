using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using InsuranceDMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InsuranceDMS.Infrastructure.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _db;
    public AppointmentRepository(AppDbContext db) => _db = db;

    public async Task<(List<CarrierAppointment> Items, int TotalCount)> GetListAsync(AppointmentFilter filter, CancellationToken ct = default)
    {
        var q = _db.CarrierAppointments.Include(a => a.Carrier).Include(a => a.Producer).AsQueryable();

        if (filter.ProducerId.HasValue)
            q = q.Where(a => a.ProducerId == filter.ProducerId.Value);
        if (filter.CarrierId.HasValue)
            q = q.Where(a => a.CarrierId == filter.CarrierId.Value);
        if (!string.IsNullOrWhiteSpace(filter.StateCode))
            q = q.Where(a => a.StateCode == filter.StateCode);
        if (!string.IsNullOrWhiteSpace(filter.Status))
            q = q.Where(a => a.AppointmentStatus == filter.Status);

        var total = await q.CountAsync(ct);
        var items = await q.OrderBy(a => a.Carrier.CarrierName)
            .Skip((filter.Page - 1) * filter.PageSize).Take(filter.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<CarrierAppointment?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.CarrierAppointments.Include(a => a.Carrier).Include(a => a.State).FirstOrDefaultAsync(a => a.Id == id, ct);

    public void Add(CarrierAppointment appointment) => _db.CarrierAppointments.Add(appointment);
    public void Update(CarrierAppointment appointment) => _db.CarrierAppointments.Update(appointment);
    public void Delete(CarrierAppointment appointment) => _db.CarrierAppointments.Remove(appointment);
}
