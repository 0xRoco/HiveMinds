using HiveMinds.Models;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.API.Controllers
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        public AccountsController()
        {
        }

        // GET: api/User
        [HttpGet]
        public async Task<IEnumerable<string>> Get()
        {
            return new List<string>();
        }

        // GET: api/User/5
        [HttpGet("{id:int}", Name = "GetById")]
        public async Task<string> Get(int id)
        {
            return "value";
        }
        
        [HttpGet("{username}", Name = "GetByUsername")]
        public async Task<string> Get(string username)
        {
            return "value";
        }

        // POST: api/User
        [HttpPost]
        public void Post([FromBody] Account model)
        {
        }

        // PUT: api/User/5
        [HttpPut("{id:int}")]
        public void Put(int id, [FromBody] Account model)
        {
        }

        // DELETE: api/User/5
        [HttpDelete("{id:int}")]
        public void Delete(int id)
        {
        }
    }
}
