using HiveMinds.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDto model)
        {
            return Ok();
        }

        [HttpPost("signup")]
        public async Task<ActionResult> Signup([FromBody] SignupDto model)
        {
            return Ok();
        }
        
    }
}