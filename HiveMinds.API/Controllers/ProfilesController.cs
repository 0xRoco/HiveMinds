using AutoMapper;
using HiveMinds.API.Services.Interfaces;
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
}