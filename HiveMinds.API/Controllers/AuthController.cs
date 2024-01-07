using System.Net;
using HiveMinds.API.Interfaces;
using HiveMinds.API.Services;
using HiveMinds.Common;
using HiveMinds.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;


        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ApiResponse<LoginResponseDto>> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid)
                return ApiResponse<LoginResponseDto>.FailureResponse(HttpStatusCode.BadRequest, "Invalid request");
            try
            {
                var loginResult = await _authService.Login(model);

                return !loginResult.IsSuccess
                    ? ApiResponse<LoginResponseDto>.FailureResponse(HttpStatusCode.Unauthorized, loginResult.Message)
                    : ApiResponse<LoginResponseDto>.SuccessResponse("Login successful", loginResult.Value);
            }
            catch (Exception ex)
            {
                model.Password = string.Empty;
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
                var signupResult = await _authService.Signup(model);

                if (!signupResult.IsSuccess)
                {
                    return ApiResponse<object>.FailureResponse(
                        signupResult.Status == AuthService.AuthResults.UsernameAlreadyExists
                            ? HttpStatusCode.BadRequest
                            : HttpStatusCode.InternalServerError, signupResult.Message);
                }

                return ApiResponse<object>.SuccessResponse("Account created successfully", signupResult.Value);
            }
            catch (Exception ex)
            {
                model.Password = string.Empty;
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