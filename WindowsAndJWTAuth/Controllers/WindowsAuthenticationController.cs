using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WindowsAndJWTAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WindowsAuthenticationController : ControllerBase
    {
        [HttpGet("[action]")]

        public IActionResult GetToken()
        {
            return Ok("Siema");
        }

    }
}