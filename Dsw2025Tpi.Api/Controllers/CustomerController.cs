using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers
{
    [AllowAnonymous]
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    
    public class CustomerController : ControllerBase
    {
        private readonly CustomerService _clienteService;

        public CustomerController(CustomerService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetClientes()
        {
            var clientes = await _clienteService.GetAllAsync();
            return Ok(clientes);
        }
    }
}

