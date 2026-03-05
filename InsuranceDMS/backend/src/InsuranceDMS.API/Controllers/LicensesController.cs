using InsuranceDMS.Application.Common;
using InsuranceDMS.Application.DTOs;
using InsuranceDMS.Application.Interfaces;
using InsuranceDMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceDMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LicensesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public LicensesController(IUnitOfWork uow) => _uow = uow;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<LicenseDto>>>> GetList(
        [FromQuery] int? producerId, [FromQuery] string? stateCode, [FromQuery] int? licenseTypeId,
        [FromQuery] DateTime? expiringBefore, [FromQuery] bool? isActive,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default)
    {
        var filter = new LicenseFilter { ProducerId = producerId, StateCode = stateCode, LicenseTypeId = licenseTypeId, ExpiringBefore = expiringBefore, IsActive = isActive, Page = page, PageSize = pageSize };
        var (items, total) = await _uow.Licenses.GetListAsync(filter, ct);
        return Ok(ApiResponse<List<LicenseDto>>.Success(items.Select(MapDto).ToList(),
            new PaginationMeta { Page = page, PageSize = pageSize, TotalCount = total }));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<LicenseDto>>> GetById(int id, CancellationToken ct)
    {
        var license = await _uow.Licenses.GetByIdAsync(id, ct);
        if (license == null) return NotFound(ApiResponse<LicenseDto>.Failure("License not found"));
        return Ok(ApiResponse<LicenseDto>.Success(MapDto(license)));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<LicenseDto>>> Create([FromBody] CreateLicenseDto dto, CancellationToken ct)
    {
        var license = new License
        {
            ProducerId = dto.ProducerId, StateCode = dto.StateCode, LicenseTypeId = dto.LicenseTypeId,
            LicenseNumber = dto.LicenseNumber, IssueDate = dto.IssueDate, ExpirationDate = dto.ExpirationDate,
            RenewalDate = dto.RenewalDate, IsActive = true, CreatedBy = "system"
        };
        _uow.Licenses.Add(license);
        await _uow.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = license.Id }, ApiResponse<LicenseDto>.Success(MapDto(license)));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<LicenseDto>>> Update(int id, [FromBody] UpdateLicenseDto dto, CancellationToken ct)
    {
        var license = await _uow.Licenses.GetByIdAsync(id, ct);
        if (license == null) return NotFound(ApiResponse<LicenseDto>.Failure("License not found"));

        license.StateCode = dto.StateCode; license.LicenseTypeId = dto.LicenseTypeId;
        license.LicenseNumber = dto.LicenseNumber; license.IssueDate = dto.IssueDate;
        license.ExpirationDate = dto.ExpirationDate; license.RenewalDate = dto.RenewalDate;
        license.IsActive = dto.IsActive; license.ModifiedBy = "system";

        _uow.Licenses.Update(license);
        await _uow.SaveChangesAsync(ct);
        return Ok(ApiResponse<LicenseDto>.Success(MapDto(license)));
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult<ApiResponse<LicenseDto>>> PatchStatus(int id, [FromBody] bool isActive, CancellationToken ct)
    {
        var license = await _uow.Licenses.GetByIdAsync(id, ct);
        if (license == null) return NotFound(ApiResponse<LicenseDto>.Failure("License not found"));
        license.IsActive = isActive; license.ModifiedBy = "system";
        _uow.Licenses.Update(license);
        await _uow.SaveChangesAsync(ct);
        return Ok(ApiResponse<LicenseDto>.Success(MapDto(license)));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken ct)
    {
        var license = await _uow.Licenses.GetByIdAsync(id, ct);
        if (license == null) return NotFound(ApiResponse<LicenseDto>.Failure("License not found"));
        _uow.Licenses.Delete(license);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }

    private static LicenseDto MapDto(License l) => new()
    {
        Id = l.Id, ProducerId = l.ProducerId, StateCode = l.StateCode, LicenseTypeId = l.LicenseTypeId,
        LicenseTypeCode = l.LicenseType?.Code, LicenseNumber = l.LicenseNumber,
        IssueDate = l.IssueDate, ExpirationDate = l.ExpirationDate, RenewalDate = l.RenewalDate, IsActive = l.IsActive
    };
}
