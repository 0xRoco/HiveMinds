using System.Net;
using HiveMinds.API.Core;
using HiveMinds.API.Interfaces;
using HiveMinds.Common;
using HiveMinds.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HiveMinds.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        
        private readonly IAccountRepository _accountRepository;
        private readonly ISecurityService _securityService;
        private readonly IAccountFactory _accountFactory;
        private readonly IEmailService _emailService;

        private readonly HiveMindsConfig _hiveMindsConfig;

        public AuthController(IAccountRepository accountRepository, ISecurityService securityService,
            IAccountFactory accountFactory, IEmailService emailService, IOptions<HiveMindsConfig> hiveMindsConfig,
            ILogger<AuthController> logger)
        {
            _logger = logger;
            _accountRepository = accountRepository;
            _securityService = securityService;
            _accountFactory = accountFactory;
            _emailService = emailService;
            _hiveMindsConfig = hiveMindsConfig.Value;
        }

        [HttpPost("login")]
        public async Task<ApiResponse<LoginResponseDto>> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return ApiResponse<LoginResponseDto>.FailureResponse(HttpStatusCode.BadRequest, "Invalid request");
            try
            {
                var account = await _accountRepository.GetByUsername(model.Username);
                if (account is null)
                    return ApiResponse<LoginResponseDto>.FailureResponse(HttpStatusCode.BadRequest,
                        "Account not found with the specified username");

                var passwordVerified = _securityService.VerifyPasswordHash(model.Password, account.PasswordHash);

                if (!passwordVerified)
                    return ApiResponse<LoginResponseDto>.FailureResponse(HttpStatusCode.Unauthorized,
                        "Incorrect Password");

                if (account.Status != AccountStatus.Active)
                    return ApiResponse<LoginResponseDto>.FailureResponse(HttpStatusCode.Unauthorized,
                        "Account is deactivated or suspended");

                var token = _securityService.GenerateToken(account);

                var loginResponse = new LoginResponseDto
                {
                    UserId = account.Id,
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(_hiveMindsConfig.TokenExpirationHours)
                };

                await _accountRepository.UpdateLastLogin(account);

                return ApiResponse<LoginResponseDto>.SuccessResponse("Login successful", loginResponse);
            }
            catch (Exception ex)
            {
                model.Password = "[REDACTED]";
                _logger.LogError(ex, "Error occurred while signing up user: {model}", model);
                return ApiResponse<LoginResponseDto>.FailureResponse(HttpStatusCode.InternalServerError,
                    "An error occurred while logging in");
            }
        }

        [HttpPost("signup")]
        public async Task<ActionResult<ApiResponse<object>>> Signup([FromBody] SignupDto model)
        {
            if (!ModelState.IsValid)
                return ApiResponse<object>.FailureResponse(HttpStatusCode.BadRequest, "Invalid request");

            try
            {
                if (await _accountRepository.Exists(model.Username))
                    return ApiResponse<object>.FailureResponse(HttpStatusCode.BadRequest,
                        "An account with the specified username already exists");

                var account = _accountFactory.CreateAccountModel(model);
                var databaseResult = await _accountRepository.CreateUser(account);
                if (!databaseResult)
                    return ApiResponse<object>.FailureResponse(HttpStatusCode.InternalServerError,
                        "An error occurred while signing up");

                var emailBody = await _emailService.PrepareEmailBody("Data/emailTemplate.html", account);

                await _emailService.SendEmailAsync(model.Email, "Verify Your HiveMinds Account", emailBody);

                return ApiResponse<object>.SuccessResponse("Account created successfully", account.Id);
            }
            catch (Exception ex)
            {
                model.Password = "[REDACTED]";
                _logger.LogError(ex, "Error occurred while signing up user: {model}", model);
                return ApiResponse<object>.FailureResponse(HttpStatusCode.InternalServerError,
                    "An error occurred while signing up");
            }
        }

        [HttpGet("Ping")]
        public ActionResult Ping()
        {
            return Ok("Pong");
        }
    }
}