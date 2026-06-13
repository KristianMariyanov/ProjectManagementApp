using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.Api.Common;

namespace ProjectManagement.Api.Controllers;

[ApiController]
[Route("api/diagnostics")]
[Authorize]
public class DiagnosticsController : ControllerBase
{
    [HttpGet("version")]
    public IActionResult GetVersion([FromQuery] string? component)
    {
        if (string.IsNullOrWhiteSpace(component))
            return Result.BadRequest("Component is required").ToActionResult();

        return Result<object>.Ok(new { component, version = "1.0.0" }).ToActionResult();
    }
}
