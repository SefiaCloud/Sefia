using Microsoft.AspNetCore.Mvc;
using Sefia.Common;
using Sefia.Dtos;
using Sefia.Services;
using System.Reflection;

namespace Sefia.Controllers
{
    [Route("api")]
    [ApiController]
    public class ApiController(AppSettingsService _appSettingsService, UserService _userService) : ControllerBase
    {
        [HttpGet("server-status")]
        public ActionResult<ServerStatusDto> GetServerStatus()
        {
            var version = Assembly.GetExecutingAssembly()
                      .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                      .InformationalVersion;
            var isInitialized = _appSettingsService.Get<bool>(nameof(AppSettings.IsInitialized));
            var status = new ServerStatusDto("OK", version!, isInitialized);
            return Ok(status);
        }

        [HttpPost("init-server")]
        public async Task<ActionResult> InitServer([FromBody] InitServerDto dto)
        {
            var isInitialized = _appSettingsService.Get<bool>(nameof(AppSettings.IsInitialized));

            if (isInitialized)
            {
                return Conflict(ApiResponse.Error("The server is already initialized"));
            }

            await _userService.AddUserAsync(dto.Admin.Email, dto.Admin.Password, dto.Admin.Name, UserRoles.Admin);

            _appSettingsService.Set(nameof(AppSettings.IsInitialized), true);
            _appSettingsService.Set(nameof(AppSettings.ApplicationDomain), dto.ApplicationDomain);
            _appSettingsService.Set(nameof(AppSettings.ServingDomain), dto.ServingDomain);
            _appSettingsService.Set(nameof(AppSettings.WebRoot), dto.WebRoot);

            _appSettingsService.SaveToFile();

            return Ok(ApiResponse.Success());
        }

    }
}
