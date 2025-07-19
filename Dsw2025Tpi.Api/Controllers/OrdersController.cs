using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Dsw2025Tpi.Application.Services;
using System;



namespace Dsw2025Tpi.Api.Controllers
{
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
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdOrder = await _ordersService.CreateOrderAsync(dto);
                return Ok(createdOrder);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Loggear si es necesario
                return StatusCode(500, new { message = "Ocurrio un error inesperado.", detail = ex.Message });
            }

        }
        // GET Orders
        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync(
        
        [FromQuery] OrderStatus? status,
        [FromQuery] Guid? customerId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            try
            {
                var orders = await _ordersService.GetOrdersAsync(status, customerId, pageNumber, pageSize);
                return Ok(orders);
            }
            catch (ArgumentException ex) {
                return StatusCode(500, new { message = "Ocurrio un error inesperado.", detail = ex.Message });

            }
        }





    }
}
