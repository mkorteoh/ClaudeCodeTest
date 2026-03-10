using InsuranceDMS.Domain.Entities;

namespace InsuranceDMS.Application.Interfaces;

public interface IAgencyLocationRepository
{
    Task<List<AgencyLocation>> GetByAgencyAsync(int agencyId, CancellationToken ct = default);
    Task<AgencyLocation?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AgencyLocation?> GetCorporateOfficeAsync(int agencyId, CancellationToken ct = default);
    Task<List<PersonnelLocation>> GetPersonnelAssignmentsAsync(int locationId, CancellationToken ct = default);
    Task<PersonnelLocation?> GetPersonnelAssignmentAsync(int locationId, int personnelId, CancellationToken ct = default);
    void Add(AgencyLocation location);
    void Update(AgencyLocation location);
    void Delete(AgencyLocation location);
    void AddPersonnelAssignment(PersonnelLocation pl);
    void RemovePersonnelAssignment(PersonnelLocation pl);
}
