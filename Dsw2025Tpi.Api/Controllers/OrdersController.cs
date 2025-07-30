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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

                var createdOrder = await _ordersService.CreateOrderAsync(dto);
                return Ok(createdOrder);
            
    
            
        }

        // GET api/orders
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync(
            [FromQuery] OrderStatus? status, // FromQuery es para cuando hago algo q se haga al reves, reibo u string q sea del body
            [FromQuery] Guid? customerId)
           // [FromQuery] int pageNumber = 1,
            //[FromQuery] int pageSize = 10)
        {
           
                var orders = await _ordersService.GetOrdersAsync(status, customerId);
                return Ok(orders);
 
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

