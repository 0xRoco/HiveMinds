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
    
    [HttpPost("{username}")]
    public async Task<ActionResult> CreateThought(string username, [FromBody] string body)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account == null) return NotFound();
        var thought = await _thoughtService.CreateThought(username, body);
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