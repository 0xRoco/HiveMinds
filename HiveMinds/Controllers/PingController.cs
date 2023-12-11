using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.Controllers;

public class PingController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Home");
    }
}