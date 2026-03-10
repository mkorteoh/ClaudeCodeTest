namespace InsuranceDMS.Application.Interfaces;

public interface IUnitOfWork
{
    IAgencyRepository Agencies { get; }
    IAgencyLocationRepository AgencyLocations { get; }
    IPersonnelRepository Personnel { get; }
    IProducerRepository Producers { get; }
    ILicenseRepository Licenses { get; }
    IAppointmentRepository Appointments { get; }
    IMergerRepository Mergers { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}
