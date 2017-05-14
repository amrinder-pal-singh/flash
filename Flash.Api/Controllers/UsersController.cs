using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Flash.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : Controller
    {
        readonly ApiContext context;
        public UsersController(ApiContext context)
        {
            this.context = context;
        }
        [HttpGet("{id}")]
        public ActionResult Get(string id)
        {
            var user = context.users.SingleOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }
        [Authorize]
        [HttpGet("me")]
        public ActionResult Get()
        {
 
           return Ok(GetSecureUser());
        }
        [Authorize]
        [HttpPost("me")]
        public ActionResult Post()
        {
            var user = GetSecureUser();
            return null;
        }
        Models.User GetSecureUser()
        {
            var id = HttpContext.User.Claims.First().Value;
            return context.users.SingleOrDefault(u => u.Id == id);
        }
    }
}