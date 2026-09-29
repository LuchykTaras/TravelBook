using Microsoft.AspNetCore.Mvc;

namespace TravelBook.Api.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                ok = true,
                service = "TravelBook.Api",
                utc = DateTime.UtcNow
            });
        }
    }
}