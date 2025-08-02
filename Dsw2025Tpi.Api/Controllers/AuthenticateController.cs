using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Dsw2025Tpi.Application.Exceptions; 

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
            try
            {
                var message = await _authService.RegisterAsync(model);
                return Ok(new { message });
            }
            catch (AppException ex) // Excepción personalizada
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                // Cualquier otro error no previsto
                return StatusCode(500, new { message = "Ocurrió un error inesperado al registrar el usuario." });
            }
        }
    }
}

