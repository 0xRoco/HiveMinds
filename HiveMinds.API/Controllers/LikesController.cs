using System.ComponentModel.DataAnnotations;
using HiveMinds.API.Services.Interfaces;
using HiveMinds.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LikesController : ControllerBase
{
    private readonly IThoughtService _thoughtService;
    private readonly IAccountRepository _accountRepository;


    public LikesController(IThoughtService thoughtService, IAccountRepository accountRepository)
    {
        _thoughtService = thoughtService;
        _accountRepository = accountRepository;
    }
    
    [HttpGet("{thoughtId:int}")]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetLikesByThoughtId(int thoughtId)
    {
        var likes = await _thoughtService.GetLikesByThoughtId(thoughtId);
        return Ok(likes);
    }
    
    [HttpGet("{username}")]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetLikesForUser(string username)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account == null) return NotFound();
        var likes = await _thoughtService.GetLikesForUser(account.Id);
        return Ok(likes);
    }
    
    [HttpGet("{likeId:int}")]
    public async Task<ActionResult<LikeDto>> GetLikeById(int likeId)
    {
        var like = await _thoughtService.GetLikeById(likeId);
        return Ok(like);
    }
    
    [HttpPost("{thoughtId:int}")]
    public async Task<ActionResult> LikeThought(int thoughtId, [Required] string username)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account == null) return NotFound();
        var like = await _thoughtService.LikeThought(thoughtId, username);
        if (like) return Ok();
        return BadRequest();
    }
    
    [HttpDelete("{thoughtId:int}")]
    public async Task<ActionResult> DeleteLike(int thoughtId, [Required] string username)
    {
        var like = await _thoughtService.UnlikeThought(thoughtId, username);
        if (like) return Ok();
        return BadRequest();
    }
    
    
}