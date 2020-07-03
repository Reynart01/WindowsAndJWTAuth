using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WindowsAndJWTAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using WindowsAndJWTAuth.Dtos;
using Newtonsoft.Json;

namespace WindowsAndJWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "BearerCF")]
    public class CfAuthenticationController : ControllerBase
    {
        [HttpGet("[action]")]

        public IActionResult GetToken()
        {
            var service = new JwtAuthenticationService();
            string email = GetUserEmail();
            long expiration = GetTokenExpirationTime();

            var token = service.GetToken(email, expiration);

            return Ok(token);
        }

        private string GetUserEmail()
        {
            string json = HttpContext.User
                .Claims.FirstOrDefault(x => x.Type == "custom")
                ?.Value;
            if (json is null)
            {
                return null;
            }

            dynamic result = JsonConvert.DeserializeObject(json);

            return result.email;
        }

        private long GetTokenExpirationTime()
        {
            string expStr = HttpContext.User
                .Claims.FirstOrDefault(x => x.Type == "exp")
                ?.Value;
            if (expStr is null)
            {
                return -1;
            }

            return long.Parse(expStr);
        }
    }
}