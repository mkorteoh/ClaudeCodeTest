using InsuranceDMS.Application.Common;
using InsuranceDMS.Application.DTOs;
using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceDMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProducersController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public ProducersController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProducerDto>>>> GetList(
        [FromQuery] int? agencyId, [FromQuery] string? state, [FromQuery] string? search,
        [FromQuery] string? npn, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default)
    {
        var filter = new ProducerFilter { AgencyId = agencyId, State = state, Search = search, Npn = npn, Page = page, PageSize = pageSize };
        var (items, total) = await _uow.Producers.GetListAsync(filter, ct);
        return Ok(ApiResponse<List<ProducerDto>>.Success(items.Select(MapDto).ToList(),
            new PaginationMeta { Page = page, PageSize = pageSize, TotalCount = total }));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProducerDto>>> GetById(int id, CancellationToken ct)
    {
        var producer = await _uow.Producers.GetByIdAsync(id, ct);
        if (producer == null) return NotFound(ApiResponse<ProducerDto>.Failure("Producer not found"));
        return Ok(ApiResponse<ProducerDto>.Success(MapDetailDto(producer)));
    }

    [HttpGet("{id:int}/licenses")]
    public async Task<ActionResult<ApiResponse<List<LicenseDto>>>> GetLicenses(int id, CancellationToken ct)
    {
        var licenses = await _uow.Producers.GetLicensesAsync(id, ct);
        return Ok(ApiResponse<List<LicenseDto>>.Success(licenses.Select(MapLicense).ToList()));
    }

    [HttpGet("{id:int}/appointments")]
    public async Task<ActionResult<ApiResponse<List<AppointmentDto>>>> GetAppointments(int id, CancellationToken ct)
    {
        var appointments = await _uow.Producers.GetAppointmentsAsync(id, ct);
        return Ok(ApiResponse<List<AppointmentDto>>.Success(appointments.Select(MapAppointment).ToList()));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProducerDto>>> Update(int id, [FromBody] UpdateProducerDto dto, CancellationToken ct)
    {
        var producer = await _uow.Producers.GetByIdAsync(id, ct);
        if (producer == null) return NotFound(ApiResponse<ProducerDto>.Failure("Producer not found"));

        producer.NPN = dto.NPN; producer.ResidentState = dto.ResidentState;
        producer.SSNLast4 = dto.SSNLast4; producer.DateOfBirth = dto.DateOfBirth;
        producer.EOExpirationDate = dto.EOExpirationDate; producer.ModifiedBy = "system";

        _uow.Producers.Update(producer);
        await _uow.SaveChangesAsync(ct);
        return Ok(ApiResponse<ProducerDto>.Success(MapDto(producer)));
    }

    private static ProducerDto MapDto(Producer p) => new()
    {
        Id = p.Id, PersonnelId = p.PersonnelId, NPN = p.NPN,
        ResidentState = p.ResidentState, SSNLast4 = p.SSNLast4,
        DateOfBirth = p.DateOfBirth, EOExpirationDate = p.EOExpirationDate
    };

    private static ProducerDto MapDetailDto(Producer p) => new()
    {
        Id = p.Id, PersonnelId = p.PersonnelId, NPN = p.NPN,
        ResidentState = p.ResidentState, SSNLast4 = p.SSNLast4,
        DateOfBirth = p.DateOfBirth, EOExpirationDate = p.EOExpirationDate,
        Licenses = p.Licenses.Select(MapLicense).ToList(),
        Appointments = p.Appointments.Select(MapAppointment).ToList()
    };

    private static LicenseDto MapLicense(License l) => new()
    {
        Id = l.Id, ProducerId = l.ProducerId, StateCode = l.StateCode,
        LicenseTypeId = l.LicenseTypeId, LicenseTypeCode = l.LicenseType?.Code,
        LicenseNumber = l.LicenseNumber, IssueDate = l.IssueDate,
        ExpirationDate = l.ExpirationDate, RenewalDate = l.RenewalDate, IsActive = l.IsActive
    };

    private static AppointmentDto MapAppointment(CarrierAppointment a) => new()
    {
        Id = a.Id, ProducerId = a.ProducerId, CarrierId = a.CarrierId,
        CarrierName = a.Carrier?.CarrierName, StateCode = a.StateCode,
        AppointmentDate = a.AppointmentDate, TerminationDate = a.TerminationDate,
        AppointmentStatus = a.AppointmentStatus
    };
}
