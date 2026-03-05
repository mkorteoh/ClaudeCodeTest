using InsuranceDMS.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceDMS.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<ApiResponse<object>> Login([FromBody] LoginDto dto)
    {
        // Stub: returns a fake JWT
        var token = new
        {
            accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.stub.stub",
            expiresIn = 3600,
            tokenType = "Bearer"
        };
        return Ok(ApiResponse<object>.Success(token));
    }

    [HttpPost("refresh")]
    public ActionResult<ApiResponse<object>> Refresh()
    {
        var token = new
        {
            accessToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.stub.stub",
            expiresIn = 3600,
            tokenType = "Bearer"
        };
        return Ok(ApiResponse<object>.Success(token));
    }
}

public class LoginDto
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
