using HiveMinds.API.Services.Interfaces;
using HiveMinds.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ISecurityService _securityService;
        private readonly IAccountFactory _accountFactory;

        public AuthController(IAccountRepository accountRepository, ISecurityService securityService,
            IAccountFactory accountFactory)
        {
            _accountRepository = accountRepository;
            _securityService = securityService;
            _accountFactory = accountFactory;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto model)
        {
            var account = await _accountRepository.GetByUsername(model.Username);
            if (account is null) return NotFound("Account not found with that username or password");

            var salt = Convert.FromBase64String(account.PasswordSalt);
            var hash = Convert.FromBase64String(account.PasswordHash);

            var passwordVerified = await _securityService.VerifyPassword(model.Password, salt, hash);

            if (!passwordVerified)
                return Unauthorized("Account not found with that username or password");

            await _accountRepository.UpdateLastLogin(account);

            var token = _securityService.GenerateToken(account);

            return Ok(new LoginResponseDto
            {
                UserId = account.Id,
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1)
            });
        }

        [HttpPost("signup")]
        public async Task<ActionResult> Signup([FromBody] SignupDto model)
        {
            if (await _accountRepository.Exists(model.Username))
                return BadRequest("Account already exists with that username");

            var account = await _accountFactory.CreateAccountModel(model);
            var databaseResult = await _accountRepository.CreateUser(account);
            if (!databaseResult) return BadRequest("Could not create account");

            return Ok("Account created successfully");
        }

        [HttpGet("Ping")]
        public ActionResult Ping()
        {
            return Ok("Pong");
        }
    }
}