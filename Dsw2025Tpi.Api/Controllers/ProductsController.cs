using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Dsw2025Tpi.Application.Services;
using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Application.Exceptions;
using Microsoft.AspNetCore.Authorization;




namespace Dsw2025Tpi.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsManagementService _service;

        public ProductsController(ProductsManagementService service)
        {
            _service = service;
        }

        /*
        // GET: api/products 
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var productos = await _service.GetAll(); // ya trae todos: activos e inactivos

            if (productos == null || !productos.Any())
                return NoContent(); // 204

            return Ok(productos.Select(ToResponse)); // 200
        }
        */

        // GET: api/products/paginacion
        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<ProductResponseDto>>> GetProducts([FromQuery] ProductFilterDto request)
        {
            var response = await _service.GetProductsAsync(request);
            return Ok(response);
        }

        // GET: api/products/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
        {
            var product = await _service.GetById(id);
            if (product is null) return NotFound(); // error 404
            return Ok(ToResponse(product)); // 200 con el objeto del producto solicitado
        }

       
        
        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);// esto valida el 400

            var product = new Product(dto.Sku, dto.InternalCode, dto.Name,
                                      dto.Description, dto.CurrentUnitPrice,
                                      dto.StockQuantity, true);

            var created = await _service.Add(product);
            return CreatedAtAction(nameof(GetById),
                                   new { id = created.Id },
                                   ToResponse(created)); // esto devuelve 201 Created con el objeto creado
        }

        // PUT: api/products/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id,
                                               [FromBody] ProductUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // 400 los datos no son validos

            var existing = await _service.GetById(id);

            if (existing is null) return NotFound(); // error 404


            // actualizar campos
            
            existing.Sku = dto.Sku;
            existing.InternalCode = dto.InternalCode;
            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.CurrentUnitPrice = dto.CurrentUnitPrice;
            existing.StockQuantity = dto.StockQuantity;
            existing.IsActive = dto.IsActive;

            var updated = await _service.Update(existing);
            return Ok(ToResponse(updated)); // 200 producto actualizado
        }



        // PATCH: api/products/{id}/disable
        [HttpPatch("{id:guid}/disable")]
        public async Task<IActionResult> Disable(Guid id)
        {
            await _service.Disable(id);
            return NoContent(); // 204 si se desactiva correctamente
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _service.Delete(id); // lanza error si no está inactivo
            return NoContent(); // 204 eliminado
        }



        // mapeo manual DTO <-> entidad
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

