using System.Net;
using AutoMapper;
using HiveMinds.API.Interfaces;
using HiveMinds.Common;
using HiveMinds.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.API.Controllers;


[Route("api/[controller]")]
[ApiController, Authorize]
public class UsersController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UsersController> _logger;


    public UsersController(IAccountRepository accountRepository, IMapper mapper, ILogger<UsersController> logger)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<ApiResponse<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _accountRepository.GetAll();
        var usersDtos = _mapper.Map<IEnumerable<UserDto>>(users);
        return ApiResponse<IEnumerable<UserDto>>.SuccessResponse("Users found",
            usersDtos);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ApiResponse<UserDto>> GetUser(int id)
    {
        var user = await _accountRepository.GetById(id);
        return user == null
            ? ApiResponse<UserDto>.FailureResponse(HttpStatusCode.NotFound, "User not found")
            : ApiResponse<UserDto>.SuccessResponse("User found", _mapper.Map<UserDto>(user));
    }

    [HttpGet("{username}"), AllowAnonymous]
    public async Task<ApiResponse<UserDto>> GetUser(string username)
    {
        var user = await _accountRepository.GetByUsername(username);
        return user == null
            ? ApiResponse<UserDto>.FailureResponse(HttpStatusCode.NotFound, "User not found")
            : ApiResponse<UserDto>.SuccessResponse("User found", _mapper.Map<UserDto>(user));
    }

    [HttpPost("verify-email")]
    public async Task<ApiResponse<object>> VerifyEmail([FromBody] VerifyEmailDto model)
    {
        if (!ModelState.IsValid)
            return ApiResponse<object>.FailureResponse(HttpStatusCode.BadRequest, "Invalid request");
        try
        {
            var user = await _accountRepository.GetByUsername(model.Username);
            if (user == null) return ApiResponse<object>.FailureResponse(HttpStatusCode.BadRequest, "Invalid username");
            if (user.IsEmailVerified)
                return ApiResponse<object>.FailureResponse(HttpStatusCode.BadRequest, "Email already verified");
            if (user.EmailCode != model.VerificationCode)
                return ApiResponse<object>.FailureResponse(HttpStatusCode.BadRequest, "Invalid verification code");
            user.IsEmailVerified = true;
            await _accountRepository.UpdateUser(user);
            return ApiResponse<object>.SuccessResponse("Email verified");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying email for user: {model}", model);
            return ApiResponse<object>.FailureResponse(HttpStatusCode.InternalServerError, "Error verifying email");
        }
    }
}