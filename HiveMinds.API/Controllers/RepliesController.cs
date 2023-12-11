using System.ComponentModel.DataAnnotations;
using HiveMinds.API.Interfaces;
using HiveMinds.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RepliesController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IThoughtService _thoughtService;

    public RepliesController(IThoughtService thoughtService, IAccountRepository accountRepository)
    {
        _thoughtService = thoughtService;
        _accountRepository = accountRepository;
    }

    [HttpGet("{replyId:int}")]
    public async Task<ActionResult<ReplyDto>> GetReplyById(int replyId)
    {
        var reply = await _thoughtService.GetReplyById(replyId);
        if (reply is null) return NotFound();
        return Ok(reply);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReplyDto>>> GetRepliesByThoughtId([Required] int thoughtId)
    {
        var replies = await _thoughtService.GetRepliesByThoughtId(thoughtId);
        return Ok(replies);
    }
    
    [HttpGet("{username}")]
    public async Task<ActionResult<IEnumerable<ReplyDto>>> GetRepliesForUser(string username)
    {
        var replies = await _thoughtService.GetRepliesForUser(username);
        return Ok(replies);
    }
    
    [HttpPost("{thoughtId:int}")]
    public async Task<ActionResult> ReplyToThought(int thoughtId, [Required] string username, [FromBody] string body)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account == null) return NotFound();
        var reply = await _thoughtService.ReplyToThought(thoughtId, username, body);
        if (reply) return Ok();
        return BadRequest();
    }
    
    [HttpDelete("{replyId:int}")]
    public async Task<ActionResult> DeleteReply(int replyId)
    {
        var reply = await _thoughtService.DeleteReply(replyId);
        if (reply) return Ok();
        return BadRequest();
    }
    
}