using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CsharpBackend.Data;
using CsharpBackend.Models;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CsharpBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly CsharpBackendContext _context;

        public AccountController(CsharpBackendContext context)
        {
            _context = context;
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
            var user = await _context.User.FirstOrDefaultAsync(u => u.Login == ld.login && u.Password == ld.password);
            if (user == null)
            {
                Response.StatusCode = 401;
                return new { message = "wrong login/password" };
            }
            return AuthOptions.GenerateToken(user.IsAdmin);
        }


        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.User.ToListAsync();
        }
        
    }
}
