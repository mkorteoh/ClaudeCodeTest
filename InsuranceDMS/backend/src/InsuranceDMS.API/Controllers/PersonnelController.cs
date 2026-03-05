using InsuranceDMS.Application.Common;
using InsuranceDMS.Application.DTOs;
using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using InsuranceDMS.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceDMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PersonnelController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public PersonnelController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<PersonnelDto>>>> GetList(
        [FromQuery] int? agencyId, [FromQuery] byte? type, [FromQuery] string? search,
        [FromQuery] bool? isActive, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default)
    {
        var filter = new PersonnelFilter { AgencyId = agencyId, Type = type, Search = search, IsActive = isActive, Page = page, PageSize = pageSize };
        var (items, total) = await _uow.Personnel.GetListAsync(filter, ct);
        return Ok(ApiResponse<List<PersonnelDto>>.Success(items.Select(MapDto).ToList(),
            new PaginationMeta { Page = page, PageSize = pageSize, TotalCount = total }));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<PersonnelDto>>> GetById(int id, CancellationToken ct)
    {
        var p = await _uow.Personnel.GetByIdAsync(id, ct);
        if (p == null) return NotFound(ApiResponse<PersonnelDto>.Failure("Personnel not found"));
        return Ok(ApiResponse<PersonnelDto>.Success(MapDto(p)));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PersonnelDto>>> Create([FromBody] CreatePersonnelDto dto, CancellationToken ct)
    {
        var personnel = new Personnel
        {
            AgencyId = dto.AgencyId, FirstName = dto.FirstName, LastName = dto.LastName,
            MiddleName = dto.MiddleName, Email = dto.Email, Phone = dto.Phone,
            PersonnelType = dto.PersonnelType, Title = dto.Title, HireDate = dto.HireDate,
            IsActive = true, CreatedBy = "system"
        };
        _uow.Personnel.Add(personnel);

        if (dto.PersonnelType == PersonnelType.Producer)
        {
            var producer = new Producer
            {
                Personnel = personnel, NPN = dto.NPN, ResidentState = dto.ResidentState,
                SSNLast4 = dto.SSNLast4, DateOfBirth = dto.DateOfBirth, EOExpirationDate = dto.EOExpirationDate,
                CreatedBy = "system"
            };
            _uow.Producers.Add(producer);
        }

        await _uow.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = personnel.Id }, ApiResponse<PersonnelDto>.Success(MapDto(personnel)));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<PersonnelDto>>> Update(int id, [FromBody] UpdatePersonnelDto dto, CancellationToken ct)
    {
        var personnel = await _uow.Personnel.GetByIdAsync(id, ct);
        if (personnel == null) return NotFound(ApiResponse<PersonnelDto>.Failure("Personnel not found"));

        personnel.FirstName = dto.FirstName; personnel.LastName = dto.LastName; personnel.MiddleName = dto.MiddleName;
        personnel.Email = dto.Email; personnel.Phone = dto.Phone; personnel.Title = dto.Title;
        personnel.HireDate = dto.HireDate; personnel.TerminationDate = dto.TerminationDate;
        personnel.IsActive = dto.IsActive; personnel.ModifiedBy = "system";

        _uow.Personnel.Update(personnel);
        await _uow.SaveChangesAsync(ct);
        return Ok(ApiResponse<PersonnelDto>.Success(MapDto(personnel)));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var personnel = await _uow.Personnel.GetByIdAsync(id, ct);
        if (personnel == null) return NotFound(ApiResponse<PersonnelDto>.Failure("Personnel not found"));
        _uow.Personnel.Delete(personnel);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }

    private static PersonnelDto MapDto(Personnel p) => new()
    {
        Id = p.Id, AgencyId = p.AgencyId, AgencyName = p.Agency?.AgencyName ?? "",
        FirstName = p.FirstName, LastName = p.LastName, MiddleName = p.MiddleName,
        Email = p.Email, Phone = p.Phone, PersonnelType = p.PersonnelType,
        Title = p.Title, HireDate = p.HireDate, TerminationDate = p.TerminationDate,
        IsActive = p.IsActive, CreatedAt = p.CreatedAt,
        Producer = p.Producer == null ? null : new ProducerDto
        {
            Id = p.Producer.Id, PersonnelId = p.Producer.PersonnelId, NPN = p.Producer.NPN,
            ResidentState = p.Producer.ResidentState, SSNLast4 = p.Producer.SSNLast4,
            DateOfBirth = p.Producer.DateOfBirth, EOExpirationDate = p.Producer.EOExpirationDate
        }
    };
}
