using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// --- IMPORTACIONES CORREGIDAS ---
using Dsw2025Tpi.Domain.Entities;       // Para 'Customer'
using Dsw2025Tpi.Domain.Interfaces;     // Para 'IRepository'
using Dsw2025Tpi.Application.Exceptions; // Para 'AppException'
using Dsw2025Tpi.Application.Dtos;      // <--- AQUÍ ESTÁN LoginModel y RegisterModel
// --------------------------------

namespace Dsw2025Tpi.Application.Services
{
    public class AuthenticateService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        // Inyectamos el repositorio para poder guardar el Cliente
        private readonly IRepository<Customer> _customerRepo;

        public AuthenticateService(
            IConfiguration config,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthenticateService> logger,
            IRepository<Customer> customerRepo // Recibimos el repo
            )
        {
            _config = config;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _customerRepo = customerRepo; // Lo guardamos
        }

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

        public async Task<string> RegisterAsync(RegisterModel model)
        {
            // 1. Validaciones básicas
            var existingUser = await _userManager.FindByNameAsync(model.Username);
            if (existingUser != null)
            {
                throw new AppException("El nombre de usuario ya existe.");
            }

            if (model.Password != model.ConfirmPassword)
            {
                throw new AppException("Las contraseñas no coinciden.");
            }

            // 2. Crear el usuario en IDENTITY (Sistema de Login)
            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new AppException($"Error al crear el usuario: {errorMessages}");
            }

            // 3. Crear roles si no existen en la BD
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await _roleManager.RoleExistsAsync("Customer"))
                await _roleManager.CreateAsync(new IdentityRole("Customer"));

            // 4. Validar que el rol sea uno permitido
            var allowedRoles = new[] { "Admin", "Customer" };
            if (!allowedRoles.Contains(model.Role))
                throw new AppException("El rol especificado no es válido.");

            // 5. Asignar el rol al usuario
            await _userManager.AddToRoleAsync(user, model.Role);

            // --- 6. PARTE CRÍTICA: CREAR EL CLIENTE EN LA TABLA DE NEGOCIO ---
            // Si el usuario es un Cliente, debemos crear su ficha en la tabla 'Customers'
            // para que luego pueda hacer pedidos sin error de "EntityNotFound".
            if (model.Role == "Customer")
            {
                var newCustomer = new Customer
                {
                    Id = Guid.Parse(user.Id), // CLAVE: Usamos el mismo ID que Identity generó
                    Name = user.UserName,
                    Email = user.Email,
                    PhoneNumber = "" // Inicializamos vacío para evitar nulos
                };

                await _customerRepo.Add(newCustomer);
            }
            // ---------------------------------------------------------------

            return $"Usuario registrado correctamente como {model.Role}.";
        }

        private async Task<string> GenerateJwtToken(IdentityUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

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

