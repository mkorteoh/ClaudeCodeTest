using InsuranceDMS.Application.Common;
using InsuranceDMS.Application.DTOs;
using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceDMS.API.Controllers;

[ApiController]
[Route("api/v1/agency-locations")]
public class AgencyLocationsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public AgencyLocationsController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AgencyLocationDto>>>> GetByAgency(
        [FromQuery] int agencyId, CancellationToken ct)
    {
        var locations = await _uow.AgencyLocations.GetByAgencyAsync(agencyId, ct);
        return Ok(ApiResponse<List<AgencyLocationDto>>.Success(locations.Select(AgenciesController.MapLocation).ToList()));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<AgencyLocationDto>>> GetById(int id, CancellationToken ct)
    {
        var location = await _uow.AgencyLocations.GetByIdAsync(id, ct);
        if (location == null) return NotFound(ApiResponse<AgencyLocationDto>.Failure("Location not found"));
        return Ok(ApiResponse<AgencyLocationDto>.Success(AgenciesController.MapLocation(location)));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AgencyLocationDto>>> Create(
        [FromBody] CreateAgencyLocationDto dto, CancellationToken ct)
    {
        var agency = await _uow.Agencies.GetByIdAsync(dto.AgencyId, ct);
        if (agency == null) return BadRequest(ApiResponse<AgencyLocationDto>.Failure("Agency not found"));

        if (dto.IsCorporateOffice)
            await DemoteExistingCorporateOffice(dto.AgencyId, ct);

        var location = new AgencyLocation
        {
            AgencyId = dto.AgencyId, LocationName = dto.LocationName, IsCorporateOffice = dto.IsCorporateOffice,
            Phone = dto.Phone, Email = dto.Email, Website = dto.Website,
            AddressLine1 = dto.AddressLine1, AddressLine2 = dto.AddressLine2, City = dto.City,
            StateCode = dto.StateCode, ZipCode = dto.ZipCode, County = dto.County,
            IsActive = true, CreatedBy = "system"
        };
        _uow.AgencyLocations.Add(location);
        await _uow.SaveChangesAsync(ct);

        var created = await _uow.AgencyLocations.GetByIdAsync(location.Id, ct);
        return CreatedAtAction(nameof(GetById), new { id = location.Id },
            ApiResponse<AgencyLocationDto>.Success(AgenciesController.MapLocation(created!)));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<AgencyLocationDto>>> Update(
        int id, [FromBody] UpdateAgencyLocationDto dto, CancellationToken ct)
    {
        var location = await _uow.AgencyLocations.GetByIdAsync(id, ct);
        if (location == null) return NotFound(ApiResponse<AgencyLocationDto>.Failure("Location not found"));

        if (dto.IsCorporateOffice && !location.IsCorporateOffice)
            await DemoteExistingCorporateOffice(location.AgencyId, ct);

        location.LocationName = dto.LocationName; location.IsCorporateOffice = dto.IsCorporateOffice;
        location.Phone = dto.Phone; location.Email = dto.Email; location.Website = dto.Website;
        location.AddressLine1 = dto.AddressLine1; location.AddressLine2 = dto.AddressLine2;
        location.City = dto.City; location.StateCode = dto.StateCode;
        location.ZipCode = dto.ZipCode; location.County = dto.County;
        location.IsActive = dto.IsActive; location.ModifiedBy = "system";

        _uow.AgencyLocations.Update(location);
        await _uow.SaveChangesAsync(ct);
        return Ok(ApiResponse<AgencyLocationDto>.Success(AgenciesController.MapLocation(location)));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var location = await _uow.AgencyLocations.GetByIdAsync(id, ct);
        if (location == null) return NotFound(ApiResponse<AgencyLocationDto>.Failure("Location not found"));

        var allLocations = await _uow.AgencyLocations.GetByAgencyAsync(location.AgencyId, ct);
        if (allLocations.Count <= 1)
            return BadRequest(ApiResponse<AgencyLocationDto>.Failure("Cannot delete the only location for an agency"));
        if (location.IsCorporateOffice)
            return BadRequest(ApiResponse<AgencyLocationDto>.Failure("Cannot delete the corporate office. Promote another location first."));

        _uow.AgencyLocations.Delete(location);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }

    [HttpPost("{id:int}/set-corporate")]
    public async Task<ActionResult<ApiResponse<AgencyLocationDto>>> SetCorporate(int id, CancellationToken ct)
    {
        var location = await _uow.AgencyLocations.GetByIdAsync(id, ct);
        if (location == null) return NotFound(ApiResponse<AgencyLocationDto>.Failure("Location not found"));

        await DemoteExistingCorporateOffice(location.AgencyId, ct);

        location.IsCorporateOffice = true;
        location.ModifiedBy = "system";
        _uow.AgencyLocations.Update(location);
        await _uow.SaveChangesAsync(ct);
        return Ok(ApiResponse<AgencyLocationDto>.Success(AgenciesController.MapLocation(location)));
    }

    [HttpGet("{locationId:int}/personnel")]
    public async Task<ActionResult<ApiResponse<List<PersonnelLocationDto>>>> GetPersonnel(
        int locationId, CancellationToken ct)
    {
        var assignments = await _uow.AgencyLocations.GetPersonnelAssignmentsAsync(locationId, ct);
        var dtos = assignments.Select(pl => new PersonnelLocationDto
        {
            Id = pl.Id, PersonnelId = pl.PersonnelId,
            PersonnelName = $"{pl.Personnel?.FirstName} {pl.Personnel?.LastName}".Trim(),
            AgencyLocationId = pl.AgencyLocationId, AssignedDate = pl.AssignedDate
        }).ToList();
        return Ok(ApiResponse<List<PersonnelLocationDto>>.Success(dtos));
    }

    [HttpPost("{locationId:int}/personnel")]
    public async Task<ActionResult<ApiResponse<PersonnelLocationDto>>> AssignPersonnel(
        int locationId, [FromBody] AssignPersonnelToLocationDto dto, CancellationToken ct)
    {
        var location = await _uow.AgencyLocations.GetByIdAsync(locationId, ct);
        if (location == null) return NotFound(ApiResponse<PersonnelLocationDto>.Failure("Location not found"));

        var personnel = await _uow.Personnel.GetByIdAsync(dto.PersonnelId, ct);
        if (personnel == null) return BadRequest(ApiResponse<PersonnelLocationDto>.Failure("Personnel not found"));

        if (personnel.AgencyId != location.AgencyId)
            return BadRequest(ApiResponse<PersonnelLocationDto>.Failure("Personnel does not belong to the same agency as this location"));

        var existing = await _uow.AgencyLocations.GetPersonnelAssignmentAsync(locationId, dto.PersonnelId, ct);
        if (existing != null)
            return Conflict(ApiResponse<PersonnelLocationDto>.Failure("Personnel is already assigned to this location"));

        var pl = new PersonnelLocation
        {
            PersonnelId = dto.PersonnelId, AgencyLocationId = locationId,
            AssignedDate = dto.AssignedDate == default ? DateTime.UtcNow : dto.AssignedDate,
            CreatedBy = "system"
        };
        _uow.AgencyLocations.AddPersonnelAssignment(pl);
        await _uow.SaveChangesAsync(ct);

        var result = new PersonnelLocationDto
        {
            Id = pl.Id, PersonnelId = pl.PersonnelId,
            PersonnelName = $"{personnel.FirstName} {personnel.LastName}".Trim(),
            AgencyLocationId = pl.AgencyLocationId, AssignedDate = pl.AssignedDate
        };
        return Ok(ApiResponse<PersonnelLocationDto>.Success(result));
    }

    [HttpDelete("{locationId:int}/personnel/{personnelId:int}")]
    public async Task<ActionResult> RemovePersonnel(int locationId, int personnelId, CancellationToken ct)
    {
        var assignment = await _uow.AgencyLocations.GetPersonnelAssignmentAsync(locationId, personnelId, ct);
        if (assignment == null) return NotFound(ApiResponse<object>.Failure("Assignment not found"));

        _uow.AgencyLocations.RemovePersonnelAssignment(assignment);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }

    private async Task DemoteExistingCorporateOffice(int agencyId, CancellationToken ct)
    {
        var existing = await _uow.AgencyLocations.GetCorporateOfficeAsync(agencyId, ct);
        if (existing != null)
        {
            existing.IsCorporateOffice = false;
            existing.ModifiedBy = "system";
            _uow.AgencyLocations.Update(existing);
        }
    }
}
