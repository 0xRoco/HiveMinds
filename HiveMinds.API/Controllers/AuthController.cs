using HiveMinds.Models;
using HiveMinds.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            return Ok();
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupViewModel model)
        {
            return Ok();
        }
        
    }
}