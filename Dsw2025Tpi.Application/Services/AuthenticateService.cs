using Dsw2025Tpi.Application.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Services
{
    public class AuthenticateService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthenticateService(
            IConfiguration config,
            SignInManager<IdentityUser> signInManager,
            ILogger<AuthenticateService> logger,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // ============================================================
        // LOGIN
        // ============================================================
        public async Task<string> LoginAsync(LoginModel request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Usuario o contraseña incorrectos.");

            return await GenerateJwtToken(user);
        }

        // ============================================================
        // REGISTER
        // ============================================================
        public async Task<string> RegisterAsync(RegisterModel model)
        {
            // Crear roles si no existen (Admin / Customer)
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await _roleManager.RoleExistsAsync("Customer"))
                await _roleManager.CreateAsync(new IdentityRole("Customer"));

            // Validar rol enviado
            if (model.Role != "Admin" && model.Role != "Customer")
                throw new ArgumentException("El rol debe ser Admin o Customer.");

            var user = new IdentityUser
            {
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var uppercaseError = result.Errors.FirstOrDefault(e => e.Code == "PasswordRequiresUpper");
                if (uppercaseError != null)
                    throw new ArgumentException("La contraseña debe tener al menos una letra mayúscula.");

                var message = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ArgumentException(message);
            }

            // Asignar rol al usuario
            await _userManager.AddToRoleAsync(user, model.Role);

            return "Usuario registrado correctamente.";
        }

        // ============================================================
        // JWT → AHORA INCLUYE LOS ROLES
        // ============================================================
        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id)
        };

            // Agregar los roles al token
            claims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
