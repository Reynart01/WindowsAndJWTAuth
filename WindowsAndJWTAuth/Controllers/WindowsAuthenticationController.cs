using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WindowsAndJWTAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WindowsAndJWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Windows")]
    public class WindowsAuthenticationController : ControllerBase
    {
        [HttpGet("[action]")]

        public IActionResult GetToken()
        {
            var service = new JwtAuthenticationService();

            //Return signed JWT token
            return Ok(new
            {
                token = service.GetToken("email", DateTimeOffset.Now.AddDays(1).ToUnixTimeSeconds())
            });
        }

    }
}