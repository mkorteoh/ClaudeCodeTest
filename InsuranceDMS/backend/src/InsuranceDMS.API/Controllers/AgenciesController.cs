using InsuranceDMS.Application.Common;
using InsuranceDMS.Application.DTOs;
using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using InsuranceDMS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceDMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AgenciesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public AgenciesController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AgencySummaryDto>>>> GetList(
        [FromQuery] string? search, [FromQuery] byte? tier, [FromQuery] string? state,
        [FromQuery] bool? isActive, [FromQuery] int? parentId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default)
    {
        var filter = new AgencyFilter { Search = search, Tier = tier, State = state, IsActive = isActive, ParentId = parentId, Page = page, PageSize = pageSize };
        var (items, total) = await _uow.Agencies.GetListAsync(filter, ct);
        var dtos = items.Select(MapSummary).ToList();
        return Ok(ApiResponse<List<AgencySummaryDto>>.Success(dtos, new PaginationMeta { Page = page, PageSize = pageSize, TotalCount = total }));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<AgencyDto>>> GetById(int id, CancellationToken ct)
    {
        var agency = await _uow.Agencies.GetByIdAsync(id, ct);
        if (agency == null) return NotFound(ApiResponse<AgencyDto>.Failure("Agency not found"));
        return Ok(ApiResponse<AgencyDto>.Success(MapDto(agency)));
    }

    [HttpGet("{id:int}/hierarchy")]
    public async Task<ActionResult<ApiResponse<List<AgencySummaryDto>>>> GetHierarchy(int id, CancellationToken ct)
    {
        var items = await _uow.Agencies.GetHierarchyAsync(id, ct);
        return Ok(ApiResponse<List<AgencySummaryDto>>.Success(items.Select(MapSummary).ToList()));
    }

    [HttpGet("{id:int}/children")]
    public async Task<ActionResult<ApiResponse<List<AgencySummaryDto>>>> GetChildren(int id, CancellationToken ct)
    {
        var items = await _uow.Agencies.GetChildrenAsync(id, ct);
        return Ok(ApiResponse<List<AgencySummaryDto>>.Success(items.Select(MapSummary).ToList()));
    }

    [HttpGet("{id:int}/personnel")]
    public async Task<ActionResult<ApiResponse<List<PersonnelDto>>>> GetPersonnel(int id, CancellationToken ct)
    {
        var items = await _uow.Personnel.GetByAgencyAsync(id, ct);
        return Ok(ApiResponse<List<PersonnelDto>>.Success(items.Select(MapPersonnel).ToList()));
    }

    [HttpGet("{id:int}/lineage")]
    public async Task<ActionResult<ApiResponse<List<EntityLineageDto>>>> GetLineage(int id, CancellationToken ct)
    {
        var items = await _uow.Mergers.GetAgencyLineageAsync(id, ct);
        return Ok(ApiResponse<List<EntityLineageDto>>.Success(items.Select(MapLineage).ToList()));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AgencyDto>>> Create([FromBody] CreateAgencyDto dto, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(dto.NPN) && await _uow.Agencies.NpnExistsAsync(dto.NPN, null, ct))
            return Conflict(ApiResponse<AgencyDto>.Failure("NPN already exists"));

        var agency = new Agency
        {
            AgencyName = dto.AgencyName, NPN = dto.NPN, TaxId = dto.TaxId, AgencyTier = dto.AgencyTier,
            ParentAgencyId = dto.ParentAgencyId, Phone = dto.Phone, Email = dto.Email, Website = dto.Website,
            AddressLine1 = dto.AddressLine1, AddressLine2 = dto.AddressLine2, City = dto.City,
            StateCode = dto.StateCode, ZipCode = dto.ZipCode, County = dto.County, Notes = dto.Notes,
            IsActive = true, CreatedBy = "system"
        };
        _uow.Agencies.Add(agency);
        await _uow.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = agency.Id }, ApiResponse<AgencyDto>.Success(MapDto(agency)));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<AgencyDto>>> Update(int id, [FromBody] UpdateAgencyDto dto, CancellationToken ct)
    {
        var agency = await _uow.Agencies.GetByIdAsync(id, ct);
        if (agency == null) return NotFound(ApiResponse<AgencyDto>.Failure("Agency not found"));
        if (!string.IsNullOrWhiteSpace(dto.NPN) && await _uow.Agencies.NpnExistsAsync(dto.NPN, id, ct))
            return Conflict(ApiResponse<AgencyDto>.Failure("NPN already exists"));

        agency.AgencyName = dto.AgencyName; agency.NPN = dto.NPN; agency.TaxId = dto.TaxId;
        agency.AgencyTier = dto.AgencyTier; agency.ParentAgencyId = dto.ParentAgencyId;
        agency.Phone = dto.Phone; agency.Email = dto.Email; agency.Website = dto.Website;
        agency.AddressLine1 = dto.AddressLine1; agency.AddressLine2 = dto.AddressLine2;
        agency.City = dto.City; agency.StateCode = dto.StateCode; agency.ZipCode = dto.ZipCode;
        agency.County = dto.County; agency.Notes = dto.Notes; agency.IsActive = dto.IsActive;
        agency.ModifiedBy = "system";

        _uow.Agencies.Update(agency);
        await _uow.SaveChangesAsync(ct);
        return Ok(ApiResponse<AgencyDto>.Success(MapDto(agency)));
    }

    [HttpPatch("{id:int}")]
    public async Task<ActionResult<ApiResponse<AgencyDto>>> Patch(int id, [FromBody] Dictionary<string, object?> patch, CancellationToken ct)
    {
        var agency = await _uow.Agencies.GetByIdAsync(id, ct);
        if (agency == null) return NotFound(ApiResponse<AgencyDto>.Failure("Agency not found"));

        if (patch.TryGetValue("isActive", out var isActive) && isActive is not null)
            agency.IsActive = Convert.ToBoolean(isActive);
        if (patch.TryGetValue("notes", out var notes))
            agency.Notes = notes?.ToString();
        agency.ModifiedBy = "system";

        _uow.Agencies.Update(agency);
        await _uow.SaveChangesAsync(ct);
        return Ok(ApiResponse<AgencyDto>.Success(MapDto(agency)));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var agency = await _uow.Agencies.GetByIdAsync(id, ct);
        if (agency == null) return NotFound(ApiResponse<AgencyDto>.Failure("Agency not found"));
        _uow.Agencies.Delete(agency);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }

    private static AgencySummaryDto MapSummary(Agency a) => new()
    {
        Id = a.Id, AgencyName = a.AgencyName, NPN = a.NPN, AgencyTier = a.AgencyTier,
        IsActive = a.IsActive, IsMerged = a.IsMerged, StateCode = a.StateCode, ParentAgencyId = a.ParentAgencyId
    };

    private static AgencyDto MapDto(Agency a) => new()
    {
        Id = a.Id, AgencyName = a.AgencyName, NPN = a.NPN, TaxId = a.TaxId, AgencyTier = a.AgencyTier,
        ParentAgencyId = a.ParentAgencyId, ParentAgencyName = a.ParentAgency?.AgencyName,
        Phone = a.Phone, Email = a.Email, Website = a.Website, AddressLine1 = a.AddressLine1,
        AddressLine2 = a.AddressLine2, City = a.City, StateCode = a.StateCode, ZipCode = a.ZipCode,
        County = a.County, IsActive = a.IsActive, Notes = a.Notes, IsMerged = a.IsMerged,
        MergedIntoId = a.MergedIntoId, MergedAt = a.MergedAt, CreatedAt = a.CreatedAt, ModifiedAt = a.ModifiedAt
    };

    private static PersonnelDto MapPersonnel(Personnel p) => new()
    {
        Id = p.Id, AgencyId = p.AgencyId, AgencyName = p.Agency?.AgencyName ?? "",
        FirstName = p.FirstName, LastName = p.LastName, MiddleName = p.MiddleName,
        Email = p.Email, Phone = p.Phone, PersonnelType = p.PersonnelType,
        Title = p.Title, HireDate = p.HireDate, TerminationDate = p.TerminationDate,
        IsActive = p.IsActive, CreatedAt = p.CreatedAt
    };

    private static EntityLineageDto MapLineage(EntityLineage l) => new()
    {
        Id = l.Id, MergerId = l.MergerId, EntityType = l.EntityType,
        SourceEntityId = l.SourceEntityId, TargetEntityId = l.TargetEntityId,
        SourceAgencyId = l.SourceAgencyId, SourceAgencyName = l.SourceAgency?.AgencyName,
        TargetAgencyId = l.TargetAgencyId, TargetAgencyName = l.TargetAgency?.AgencyName,
        Action = l.Action, RecordedAt = l.RecordedAt
    };
}
