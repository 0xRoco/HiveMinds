using HiveMinds.API.Interfaces;
using HiveMinds.Models;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.API.Controllers.admin
{
    // Secure later or disable in prod (as if its a thing)
    [Route("api/admin/[controller]")]
    [ApiController]
    internal class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IAccountRepository accountRepository, ILogger<AccountsController> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Account>>> GetAll()
        {
            var accounts = await _accountRepository.GetAll();
            return Ok(accounts);
        }
        
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Account>> GetById(int id)
        {
            var account = await _accountRepository.GetById(id);
            if (account is null) return NotFound();
            return Ok(account);
        }
        
        [HttpGet("[action]")]
        public async Task<ActionResult<Account>> GetByEmail(string email)
        {
            var account = await _accountRepository.GetByEmail(email);
            if (account is null) return NotFound();
            return Ok(account);
        }
        
        [HttpGet("[action]")]
        public async Task<ActionResult<Account>> GetByUsername(string username)
        {
            var account = await _accountRepository.GetByUsername(username);
            if (account is null) return NotFound();
            return Ok(account);
        }
        
        [HttpGet("[action]")]
        public async Task<ActionResult<bool>> Exists(string username)
        {
            var account = await _accountRepository.GetByUsername(username);
            if (account is { Id: >= 1 }) return Ok();
            return NotFound();
        }
        
        [HttpPost]
        public async Task<ActionResult<bool>> CreateUser(Account account)
        {
            var created = await _accountRepository.CreateAccount(account);
            if (created) return Ok();
            return BadRequest();
        }
        
        [HttpPut]
        public async Task<ActionResult<bool>> UpdateUser(Account account)
        {
            var updated = await _accountRepository.UpdateAccount(account);
            if (updated) return Ok();
            return BadRequest();
        }
        
        // TODO: Delete account by id?
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteUser(Account account)
        {
            var deleted = await _accountRepository.DeleteAccount(account);
            if (deleted) return Ok();
            return BadRequest();
        }
    }
}
