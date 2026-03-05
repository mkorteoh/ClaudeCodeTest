using InsuranceDMS.Application.Common;
using InsuranceDMS.Application.DTOs;
using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceDMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public AppointmentsController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<AppointmentDto>>>> GetList(
        [FromQuery] int? producerId, [FromQuery] int? carrierId, [FromQuery] string? stateCode,
        [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default)
    {
        var filter = new AppointmentFilter { ProducerId = producerId, CarrierId = carrierId, StateCode = stateCode, Status = status, Page = page, PageSize = pageSize };
        var (items, total) = await _uow.Appointments.GetListAsync(filter, ct);
        return Ok(ApiResponse<List<AppointmentDto>>.Success(items.Select(MapDto).ToList(),
            new PaginationMeta { Page = page, PageSize = pageSize, TotalCount = total }));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> GetById(int id, CancellationToken ct)
    {
        var appt = await _uow.Appointments.GetByIdAsync(id, ct);
        if (appt == null) return NotFound(ApiResponse<AppointmentDto>.Failure("Appointment not found"));
        return Ok(ApiResponse<AppointmentDto>.Success(MapDto(appt)));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Create([FromBody] CreateAppointmentDto dto, CancellationToken ct)
    {
        var appt = new CarrierAppointment
        {
            ProducerId = dto.ProducerId, CarrierId = dto.CarrierId, StateCode = dto.StateCode,
            AppointmentDate = dto.AppointmentDate, AppointmentStatus = dto.AppointmentStatus,
            CreatedBy = "system"
        };
        _uow.Appointments.Add(appt);
        await _uow.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = appt.Id }, ApiResponse<AppointmentDto>.Success(MapDto(appt)));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Update(int id, [FromBody] UpdateAppointmentDto dto, CancellationToken ct)
    {
        var appt = await _uow.Appointments.GetByIdAsync(id, ct);
        if (appt == null) return NotFound(ApiResponse<AppointmentDto>.Failure("Appointment not found"));

        appt.CarrierId = dto.CarrierId; appt.StateCode = dto.StateCode;
        appt.AppointmentDate = dto.AppointmentDate; appt.AppointmentStatus = dto.AppointmentStatus;
        appt.TerminationDate = dto.TerminationDate; appt.ModifiedBy = "system";

        _uow.Appointments.Update(appt);
        await _uow.SaveChangesAsync(ct);
        return Ok(ApiResponse<AppointmentDto>.Success(MapDto(appt)));
    }

    [HttpPatch("{id:int}/terminate")]
    public async Task<ActionResult<ApiResponse<AppointmentDto>>> Terminate(int id, CancellationToken ct)
    {
        var appt = await _uow.Appointments.GetByIdAsync(id, ct);
        if (appt == null) return NotFound(ApiResponse<AppointmentDto>.Failure("Appointment not found"));
        appt.AppointmentStatus = "Terminated"; appt.TerminationDate = DateTime.UtcNow; appt.ModifiedBy = "system";
        _uow.Appointments.Update(appt);
        await _uow.SaveChangesAsync(ct);
        return Ok(ApiResponse<AppointmentDto>.Success(MapDto(appt)));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var appt = await _uow.Appointments.GetByIdAsync(id, ct);
        if (appt == null) return NotFound(ApiResponse<AppointmentDto>.Failure("Appointment not found"));
        _uow.Appointments.Delete(appt);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }

    private static AppointmentDto MapDto(CarrierAppointment a) => new()
    {
        Id = a.Id, ProducerId = a.ProducerId, CarrierId = a.CarrierId,
        CarrierName = a.Carrier?.CarrierName, StateCode = a.StateCode,
        AppointmentDate = a.AppointmentDate, TerminationDate = a.TerminationDate,
        AppointmentStatus = a.AppointmentStatus
    };
}
