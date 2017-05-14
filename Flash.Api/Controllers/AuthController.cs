using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Flash.Api.Controllers
{
    public class JwtPacket
    {
        public string Token { get; set; }
        public string FirstName { get; set; }
    }

    public class LoginData
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    [Produces("application/json")]
    [Route("auth")]
    public class AuthController : Controller
    {
        readonly ApiContext context;
        public AuthController(ApiContext context)
        {
            this.context = context;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginData loginData)
        {
            var user = context.users.SingleOrDefault(u => u.Email == loginData.Email && u.Password == loginData.Password);
            if (user==null)
            {
                return NotFound("Email or password incorrect");
            }
            return Ok(CreateJwtPacket(user));
           
        }


        [HttpPost("register")]
        public JwtPacket Register([FromBody]Models.User user)
        {
            context.users.Add(user);
            context.SaveChanges();
            return CreateJwtPacket(user);


}
        JwtPacket CreateJwtPacket(Models.User user)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("this is the secret phrase"));
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id)
            };
            var signingCredentials = new SigningCredentials(signingKey,SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(claims:claims,signingCredentials:signingCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
                return new JwtPacket()
                {
                    Token = encodedJwt,
                    FirstName = user.FirstName
                };
            }
        }
    }
