using System.Net;
using AutoMapper;
using HiveMinds.API.Interfaces;
using HiveMinds.Common;
using HiveMinds.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.API.Controllers;

[Route("api/[controller]"), Authorize]
[ApiController]
public class ProfilesController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IThoughtService _thoughtService;
    private readonly IMapper _mapper;

    public ProfilesController(IAccountRepository accountRepository, IThoughtService thoughtService, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _thoughtService = thoughtService;
        _mapper = mapper;
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

    /*
     * TODO: Add endpoint for updating profile
     * email, password, bio, profile picture
     */

    [HttpPut("{username}")]
    public async Task<ApiResponse<ProfileDto>> UpdateProfile(string username, [FromBody] EditProfileDto dto)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account is null)
            return ApiResponse<ProfileDto>.FailureResponse(HttpStatusCode.BadRequest,
                "No account found with that username");

        account.ProfilePictureUrl = dto.ProfilePicture;
        account.Bio = dto.Bio;
        account.LoyaltyStatement = dto.LoyaltyStatement;

        var result = await _accountRepository.UpdateUser(account);
        if (!result)
            return ApiResponse<ProfileDto>.FailureResponse(HttpStatusCode.InternalServerError,
                "Failed to update profile");

        account = await _accountRepository.GetByUsername(username);

        return ApiResponse<ProfileDto>.SuccessResponse("Profile updated", new ProfileDto
        {
            User = _mapper.Map<UserDto>(account)
        });
    }
}