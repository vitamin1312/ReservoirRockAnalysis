using Microsoft.AspNetCore.Mvc;
using CsharpBackend.Models;
using CsharpBackend.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;


namespace CsharpBackend.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IImageRepository repository;

        public AccountController(IImageRepository context)
        {
            repository = context;
        }

        public struct LoginData
        {
            public string login { get; set; }
            public string password { get; set; }
        }

        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<object>> GetToken([FromBody] LoginData ld)
        {
            var user = await repository.GetUser(ld);
            if (user == null)
            {
                Response.StatusCode = 401;
                return new { message = "wrong login/password" };
            }
            return AuthOptions.GenerateToken(user.IsAdmin);
        }


        [HttpGet("users")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await repository.GetUsers();
        }
        
    }
}
