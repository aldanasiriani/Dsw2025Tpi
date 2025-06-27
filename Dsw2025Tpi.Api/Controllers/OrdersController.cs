using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Dsw2025Tpi.Application.Services;



namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersService _ordersService;

        public OrdersController(OrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderItems = new List<OrderItem>();
            decimal total = 0;

            foreach (var item in dto.OrderItems)
            {
                var product = await _ordersService.GetById<Product>(item.ProductId);
                if (product is null)
                    return BadRequest($"Producto {item.ProductId} no encontrado.");

                if (!product.IsActive || product.StockQuantity < item.Quantity)
                    return BadRequest($"No hay stock suficiente para {product.Name}.");

                // Descontar stock
                product.StockQuantity -= item.Quantity;
                await _ordersService.Update(product);

                var orderItem = new OrderItem(item.Quantity, product.CurrentUnitPrice);

                orderItems.Add(orderItem);
                total += orderItem.Subtotal;
            }

            var order = new Order(
                 DateTime.UtcNow,
                 dto.ShippingAddress,
                dto.BillingAddress,
                dto.Notes
   );


            var created = await _ordersService.Add(order);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _ordersService.GetById<Order>(id, "OrderItems");
            if (order is null) return NotFound();
            return Ok(order);
        }
    }
}
