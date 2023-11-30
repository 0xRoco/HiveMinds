using AutoMapper;
using HiveMinds.API.Services.Interfaces;
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
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _accountRepository.GetAll();
        return Ok(_mapper.Map<List<UserDto>>(users));
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _accountRepository.GetById(id);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<UserDto>(user));
    }

    [HttpGet("{username}"), AllowAnonymous]
    public async Task<ActionResult<UserDto>> GetUser(string username)
    {
        var user = await _accountRepository.GetByUsername(username);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<UserDto>(user));
    }
}