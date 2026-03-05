using InsuranceDMS.Application.Common;
using InsuranceDMS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InsuranceDMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LookupController : ControllerBase
{
    private readonly AppDbContext _db;
    public LookupController(AppDbContext db) => _db = db;

    [HttpGet("states")]
    public async Task<ActionResult<ApiResponse<object>>> GetStates(CancellationToken ct)
    {
        var states = await _db.States.OrderBy(s => s.StateName).Select(s => new { s.StateCode, s.StateName }).ToListAsync(ct);
        return Ok(ApiResponse<object>.Success(states));
    }

    [HttpGet("license-types")]
    public async Task<ActionResult<ApiResponse<object>>> GetLicenseTypes(CancellationToken ct)
    {
        var types = await _db.LicenseTypes.OrderBy(t => t.Code).Select(t => new { t.LicenseTypeId, t.Code, t.Description }).ToListAsync(ct);
        return Ok(ApiResponse<object>.Success(types));
    }
}
