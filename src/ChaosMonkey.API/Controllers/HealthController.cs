using Microsoft.AspNetCore.Mvc;

namespace ChaosMonkey.API.Controllers
{
    [ApiController]
    public class HealthController : Controller
    {
        [HttpGet("api/v1/health")]
        public OkResult Get()
        {
            return Ok();
        }
    }
}