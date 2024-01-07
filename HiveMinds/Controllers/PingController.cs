using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.Controllers;

// Used by uptime monitors
public class PingController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("Pong!");
    }
}