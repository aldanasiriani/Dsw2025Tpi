using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _ordersService;

        public OrdersController(OrderService ordersService)
        {
            _ordersService = ordersService;
        }

        // POST api/orders
        [Authorize(Roles = "Customer")]
        [HttpPost]
public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
{
    // Validación de formato (Data Annotations)
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    // Llamada directa al servicio. 
    // Si explota por stock, el Middleware lo atrapa automáticamente.
    var createdOrder = await _ordersService.CreateOrderAsync(dto);
    
    return Ok(createdOrder);
}
   

   
     // GET api/orders
[AllowAnonymous]
[HttpGet]
public async Task<IActionResult> GetOrdersAsync(
    [FromQuery] int page = 1,        // Agregamos paginación
    [FromQuery] int limit = 10,      // Agregamos límite
    [FromQuery] OrderStatus? status = null, 
    [FromQuery] Guid? customerId = null)
{
    // Pasamos todos los datos al servicio
    var pagedResult = await _ordersService.GetOrdersAsync(page, limit, status, customerId);
    
    // Devolvemos el resultado paginado
    return Ok(pagedResult);
}


        // GET api/orders/{id}
        [Authorize(Roles = "Customer")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
           
                var order = await _ordersService.GetOrderByIdAsync(id);
                return Ok(order);
          
               
            
        }

        // PUT: /api/orders/{id}/status
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderStatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedOrder = await _ordersService.UpdateOrderStatusAsync(id, dto.NewStatus);
            return Ok(updatedOrder);
        }


    }
}

