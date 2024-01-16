using HiveMinds.API.Core;
using HiveMinds.API.Interfaces;
using HiveMinds.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.API.Controllers;


[Route("api/[controller]")]
[ApiController, Authorize]
public class ThoughtsController : ControllerBase
{
    private readonly IAccountRepository _accountRepository;
    private readonly IThoughtService _thoughtService;


    public ThoughtsController(IAccountRepository accountRepository, IThoughtService thoughtService)
    {
        _accountRepository = accountRepository;
        _thoughtService = thoughtService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ThoughtDto>>> GetThoughts()
    {
        var thoughts = await _thoughtService.GetThoughts();
        return Ok(thoughts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ThoughtDto>> GetThought(int id)
    {
        var thoughtDto =  await _thoughtService.GetThoughtById(id);
        
        if (thoughtDto is null) return NotFound();
        
        return Ok(thoughtDto);
    }
    
    [HttpGet("{username}")]
    public async Task<ActionResult<IEnumerable<ThoughtDto>>> GetThoughtsByUsername(string username)
    {
        var thoughts = await _thoughtService.GetThoughtsByUsername(username);
        return Ok(thoughts);
    }

    [HttpPost]
    public async Task<ActionResult> CreateThought([FromBody] string body)
    {
        var accountId = Utility.GetAccountIdFromClaims(User);

        var account = await _accountRepository.GetById(accountId);
        if (account == null) return NotFound();
        var thought = await _thoughtService.CreateThought(account.Username, body);
        if (thought) return Ok();
        return BadRequest("Could not create thought");
    }
    
    [HttpDelete("{thoughtId:int}")]
    public async Task<ActionResult> DeleteThought(int thoughtId)
    {
        var deleted = await _thoughtService.DeleteThought(thoughtId);
        if (deleted) return Ok();
        return BadRequest("Could not delete thought");
    }
    
    
}