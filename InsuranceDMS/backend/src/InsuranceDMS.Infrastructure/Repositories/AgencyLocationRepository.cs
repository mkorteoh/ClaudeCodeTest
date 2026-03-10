using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using InsuranceDMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InsuranceDMS.Infrastructure.Repositories;

public class AgencyLocationRepository : IAgencyLocationRepository
{
    private readonly AppDbContext _db;
    public AgencyLocationRepository(AppDbContext db) => _db = db;

    public async Task<List<AgencyLocation>> GetByAgencyAsync(int agencyId, CancellationToken ct = default) =>
        await _db.AgencyLocations
            .Include(l => l.PersonnelLocations)
            .Where(l => l.AgencyId == agencyId)
            .OrderBy(l => l.LocationName)
            .ToListAsync(ct);

    public async Task<AgencyLocation?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _db.AgencyLocations
            .Include(l => l.PersonnelLocations).ThenInclude(pl => pl.Personnel)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task<AgencyLocation?> GetCorporateOfficeAsync(int agencyId, CancellationToken ct = default) =>
        await _db.AgencyLocations
            .FirstOrDefaultAsync(l => l.AgencyId == agencyId && l.IsCorporateOffice, ct);

    public async Task<List<PersonnelLocation>> GetPersonnelAssignmentsAsync(int locationId, CancellationToken ct = default) =>
        await _db.PersonnelLocations
            .Include(pl => pl.Personnel)
            .Where(pl => pl.AgencyLocationId == locationId)
            .ToListAsync(ct);

    public async Task<PersonnelLocation?> GetPersonnelAssignmentAsync(int locationId, int personnelId, CancellationToken ct = default) =>
        await _db.PersonnelLocations
            .FirstOrDefaultAsync(pl => pl.AgencyLocationId == locationId && pl.PersonnelId == personnelId, ct);

    public void Add(AgencyLocation location) => _db.AgencyLocations.Add(location);
    public void Update(AgencyLocation location) => _db.AgencyLocations.Update(location);
    public void Delete(AgencyLocation location) => _db.AgencyLocations.Remove(location);
    public void AddPersonnelAssignment(PersonnelLocation pl) => _db.PersonnelLocations.Add(pl);
    public void RemovePersonnelAssignment(PersonnelLocation pl) => _db.PersonnelLocations.Remove(pl);
}
