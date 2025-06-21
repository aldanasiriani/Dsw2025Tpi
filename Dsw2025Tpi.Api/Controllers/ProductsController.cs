using Dsw2025Tpi.Data.Repositories;      // IRepository
using Dsw2025Tpi.Domain.Common;          // EntityBase
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IRepository _repo;

        public ProductsController(IRepository repo)
        {
            _repo = repo;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll()
        {
            var productos = await _repo.GetAll<Product>();
            var response = productos?.Select(ToResponse);
            return Ok(response);
        }

        // GET: api/products/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
        {
            var product = await _repo.GetById<Product>(id);
            if (product is null) return NotFound();
            return Ok(ToResponse(product));
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = new Product(dto.Sku, dto.InternalCode, dto.Name,
                                      dto.Description, dto.CurrentUnitPrice,
                                      dto.StockQuantity, true);

            var created = await _repo.Add(product);
            return CreatedAtAction(nameof(GetById),
                                   new { id = created.Id },
                                   ToResponse(created));
        }

        // PUT: api/products/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,
                                               [FromBody] ProductUpdateDto dto)
        {
            var existing = await _repo.GetById<Product>(id);
            if (existing is null) return NotFound();

            // actualizar campos
            existing.Sku = dto.Sku;
            existing.InternalCode = dto.InternalCode;
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.CurrentUnitPrice = dto.CurrentUnitPrice;
            existing.StockQuantity = dto.StockQuantity;
            existing.IsActive = dto.IsActive;

            var updated = await _repo.Update(existing);
            return Ok(ToResponse(updated));
        }

        // PATCH: api/products/{id}/disable
        [HttpPatch("{id:guid}/disable")]
        public async Task<IActionResult> Disable(Guid id)
        {
            var product = await _repo.GetById<Product>(id);
            if (product is null) return NotFound();

            product.IsActive = false;
            await _repo.Update(product);
            return NoContent();
        }

        // DELETE físico (opcional)
        //[HttpDelete("{id:guid}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var product = await _repo.GetById<Product>(id);
        //    if (product is null) return NotFound();
        //    await _repo.Delete(product);
        //    return NoContent();
        //}

        /* --------------------- mapeo manual DTO <-> entidad --------------------*/
        private static ProductResponseDto ToResponse(Product p) => new()
        {
            Id = p.Id,
            Sku = p.Sku,
            InternalCode = p.InternalCode,
            Name = p.Name,
            Description = p.Description,
            CurrentUnitPrice = p.CurrentUnitPrice,
            StockQuantity = p.StockQuantity,
            IsActive = p.IsActive
        };
    }
}

