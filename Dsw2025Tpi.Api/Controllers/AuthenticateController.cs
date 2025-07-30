using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticateController : ControllerBase
    {
        private readonly AuthenticateService _authService;

        public AuthenticateController(AuthenticateService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            var token = await _authService.LoginAsync(request);
            return Ok(new { token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var message = await _authService.RegisterAsync(model);
            return Ok(new { message });
        }
    }
}

