using InsuranceDMS.Domain.Entities;
using InsuranceDMS.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace InsuranceDMS.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Agency> Agencies => Set<Agency>();
    public DbSet<Personnel> Personnel => Set<Personnel>();
    public DbSet<Producer> Producers => Set<Producer>();
    public DbSet<License> Licenses => Set<License>();
    public DbSet<CarrierAppointment> CarrierAppointments => Set<CarrierAppointment>();
    public DbSet<Carrier> Carriers => Set<Carrier>();
    public DbSet<State> States => Set<State>();
    public DbSet<LicenseType> LicenseTypes => Set<LicenseType>();
    public DbSet<AgencyLocation> AgencyLocations => Set<AgencyLocation>();
    public DbSet<PersonnelLocation> PersonnelLocations => Set<PersonnelLocation>();
    public DbSet<Merger> Mergers => Set<Merger>();
    public DbSet<MergerParticipant> MergerParticipants => Set<MergerParticipant>();
    public DbSet<EntityLineage> EntityLineages => Set<EntityLineage>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Global soft-delete filter
        modelBuilder.Entity<Agency>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<AgencyLocation>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Personnel>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Producer>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<Carrier>().HasQueryFilter(x => !x.IsDeleted);
    }
}
