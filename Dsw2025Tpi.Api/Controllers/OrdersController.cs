using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Api.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", detail = ex.Message });
            }
        }

        // GET api/orders
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
            catch (ArgumentException ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", detail = ex.Message });
            }
        }

        //  GET api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            {
                var order = await _ordersService.GetOrderByIdAsync(id);

                if (order == null)
                    return NotFound(new { message = $"No se encontró una orden con el ID {id}." });

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", detail = ex.Message });
            }
        }

        // PUT: /api/orders/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderStatusUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedOrder = await _ordersService.UpdateOrderStatusAsync(id, dto.NewStatus);

                if (updatedOrder == null)
                    return NotFound(new { message = $"No se encontró la orden con ID {id}." });

                return Ok(updatedOrder); // 200 con detalles actualizados
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400 si el estado es inválido
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error inesperado.", detail = ex.Message });
            }
        }


    }
}

