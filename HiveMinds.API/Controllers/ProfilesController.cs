using System.Net;
using AutoMapper;
using HiveMinds.API.Core;
using HiveMinds.API.Interfaces;
using HiveMinds.Common;
using HiveMinds.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace HiveMinds.API.Controllers;

[Route("api/[controller]"), Authorize]
[ApiController]
public class ProfilesController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IThoughtService _thoughtService;
    private readonly IMapper _mapper;
    private readonly IMinioClientFactory _minioClientFactory;
    private readonly ILogger<ProfilesController> _logger;

    public ProfilesController(IAccountRepository accountRepository, IThoughtService thoughtService, IMapper mapper,
        IMinioClientFactory minioClientFactory, ILogger<ProfilesController> logger)
    {
        _accountRepository = accountRepository;
        _thoughtService = thoughtService;
        _mapper = mapper;
        _minioClientFactory = minioClientFactory;
        _logger = logger;
    }

    [HttpGet("{username}"), AllowAnonymous]
    public async Task<ActionResult<ProfileDto>> GetProfile(string username)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account is null)
        {
            return NotFound($"No account found with username {username}");
        }

        var thoughts = await _thoughtService.GetThoughtsByUsername(account.Username);
        var likes = await _thoughtService.GetLikesForUser(account.Id);

        var profile = new ProfileDto
        {
            User = _mapper.Map<UserDto>(account),
            Thoughts = _mapper.Map<List<ThoughtDto>>(thoughts),
            Likes = _mapper.Map<List<LikeDto>>(likes)
        };

        return Ok(profile);
    }

    [HttpPut("{username}")]
    public async Task<ApiResponse<ProfileDto>> UpdateProfile([FromBody] EditProfileDto dto)
    {
        if (!ModelState.IsValid)
            return ApiResponse<ProfileDto>.FailureResponse(HttpStatusCode.BadRequest, "Invalid request");

        var accountId = Utility.GetAccountIdFromClaims(User);

        var account = await _accountRepository.GetById(accountId);
        if (account is null)
            return ApiResponse<ProfileDto>.FailureResponse(HttpStatusCode.BadRequest,
                "No account found with that username");

        if (accountId != account.Id)
            return ApiResponse<ProfileDto>.FailureResponse(HttpStatusCode.BadRequest,
                "Invalid account id");

        account.Bio = dto.Bio;

        var result = await _accountRepository.UpdateAccount(account);
        if (!result)
            return ApiResponse<ProfileDto>.FailureResponse(HttpStatusCode.InternalServerError,
                "Failed to update profile");

        return ApiResponse<ProfileDto>.SuccessResponse("Profile updated", new ProfileDto
        {
            User = _mapper.Map<UserDto>(account)
        });
    }

    //TODO: Move this garbage to a service

    [HttpPut("{username}/profile-picture")]
    public async Task<ApiResponse<ProfileDto>> UpdateProfilePicture([FromForm] IFormFile profilePicture)
    {
        var accountId = Utility.GetAccountIdFromClaims(User);
        var account = await _accountRepository.GetById(accountId);
        if (account is null)
            return ApiResponse<ProfileDto>.FailureResponse(HttpStatusCode.BadRequest,
                "No account found with that username");

        if (accountId != account.Id)
            return ApiResponse<ProfileDto>.FailureResponse(HttpStatusCode.BadRequest,
                "Invalid account id");

        if (profilePicture.Length <= 0)
            return ApiResponse<ProfileDto>.FailureResponse(HttpStatusCode.BadRequest,
                "Invalid profile picture");

        var minioClient = _minioClientFactory.CreateClient();

        var targetBucket = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? "hiveminds-local"
            : "hiveminds";

        var imageName =
            $"{Guid.NewGuid()}.{profilePicture.FileName[(profilePicture.FileName.LastIndexOf('.') + 1)..]}";

        await minioClient.PutObjectAsync(new PutObjectArgs().WithBucket(targetBucket)
            .WithObject($"profile_images/{account.Id}/{imageName}")
            .WithContentType(profilePicture.ContentType).WithStreamData(profilePicture.OpenReadStream())
            .WithObjectSize(profilePicture.Length));

        account.ProfilePictureUrl =
            $"https://cdn.mdnite-vps.xyz/{targetBucket}/profile_images/{account.Id}/{imageName}";

        var result = await _accountRepository.UpdateAccount(account);
        if (!result)
            return ApiResponse<ProfileDto>.FailureResponse(HttpStatusCode.InternalServerError,
                "Failed to update profile");

        return ApiResponse<ProfileDto>.SuccessResponse("Profile updated", new ProfileDto
        {
            User = _mapper.Map<UserDto>(account)
        });
    }
}