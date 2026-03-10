using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace InsuranceDMS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppDbContext _db;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(AppDbContext db,
        IAgencyRepository agencies,
        IAgencyLocationRepository agencyLocations,
        IPersonnelRepository personnel,
        IProducerRepository producers,
        ILicenseRepository licenses,
        IAppointmentRepository appointments,
        IMergerRepository mergers)
    {
        _db = db;
        Agencies = agencies;
        AgencyLocations = agencyLocations;
        Personnel = personnel;
        Producers = producers;
        Licenses = licenses;
        Appointments = appointments;
        Mergers = mergers;
    }

    public IAgencyRepository Agencies { get; }
    public IAgencyLocationRepository AgencyLocations { get; }
    public IPersonnelRepository Personnel { get; }
    public IProducerRepository Producers { get; }
    public ILicenseRepository Licenses { get; }
    public IAppointmentRepository Appointments { get; }
    public IMergerRepository Mergers { get; }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _db.SaveChangesAsync(ct);

    public async Task BeginTransactionAsync(CancellationToken ct = default) =>
        _transaction = await _db.Database.BeginTransactionAsync(ct);

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(ct);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose() => _transaction?.Dispose();
}
