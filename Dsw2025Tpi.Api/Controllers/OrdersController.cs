using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;



namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository _repo;

        public OrdersController(IRepository repo)
        {
            _repo = repo;
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
                var product = await _repo.GetById<Product>(item.ProductId);
                if (product is null)
                    return BadRequest($"Producto {item.ProductId} no encontrado.");

                if (!product.IsActive || product.StockQuantity < item.Quantity)
                    return BadRequest($"No hay stock suficiente para {product.Name}.");

                // Descontar stock
                product.StockQuantity -= item.Quantity;
                await _repo.Update(product);

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


            var created = await _repo.Add(order);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var order = await _repo.GetById<Order>(id, "OrderItems");
            if (order is null) return NotFound();
            return Ok(order);
        }
    }
}
