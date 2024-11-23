using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sefia.Common;
using Sefia.Dtos;

namespace Sefia.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
