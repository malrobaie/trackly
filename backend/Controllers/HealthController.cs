using Microsoft.AspNetCore.Mvc;

namespace Trackly.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(new
        {
            status = "ok",
            service = "Trackly.Api",
            utc = DateTime.UtcNow
        });
}
