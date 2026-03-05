using InsuranceDMS.Application.Common;
using InsuranceDMS.Application.DTOs;
using InsuranceDMS.Domain.Entities;
using InsuranceDMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceDMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CarriersController : ControllerBase
{
    private readonly AppDbContext _db;
    public CarriersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<CarrierDto>>>> GetList(CancellationToken ct)
    {
        var carriers = await _db.Carriers.Where(c => !c.IsDeleted).OrderBy(c => c.CarrierName).ToListAsync(ct);
        return Ok(ApiResponse<List<CarrierDto>>.Success(carriers.Select(MapDto).ToList()));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CarrierDto>>> Create([FromBody] CreateCarrierDto dto, CancellationToken ct)
    {
        var carrier = new Carrier { CarrierName = dto.CarrierName, NAIC = dto.NAIC, IsActive = true, CreatedBy = "system" };
        _db.Carriers.Add(carrier);
        await _db.SaveChangesAsync(ct);
        return Created($"/api/v1/carriers/{carrier.Id}", ApiResponse<CarrierDto>.Success(MapDto(carrier)));
    }

    private static CarrierDto MapDto(Carrier c) => new() { Id = c.Id, CarrierName = c.CarrierName, NAIC = c.NAIC, IsActive = c.IsActive };
}
