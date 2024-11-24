using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sefia.Common;
using Sefia.Dtos;
using Sefia.Services;
using System.Reflection;

namespace Sefia.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController(AppSettingsService _appSettingsService) : ControllerBase
    {
        [HttpGet("server-status")]
        public async Task<ActionResult<ServerStatusDto>> GetServerStatus()
        {
            var version = Assembly.GetExecutingAssembly()
                      .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                      .InformationalVersion;
            var isInitialized = _appSettingsService.Get(nameof(AppSettings.IsInitialized)) as bool?;

            if (isInitialized == null)
            {
                throw new Exception("Setting: IsInitialized Must Bool");
            }

            var status = new ServerStatusDto("OK", version!, isInitialized.Value);
            return Ok(status);
        }
    }
}
