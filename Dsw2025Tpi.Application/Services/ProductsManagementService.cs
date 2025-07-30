using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Dsw2025Tpi.Application.Services
{
    public class ProductsManagementService
    {
        private readonly IRepository<Product> _productRepo;

        public ProductsManagementService(IRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        // Obtener todos los productos activos
        public async Task<IEnumerable<Product>> GetAll()
        {
            var productos = await _productRepo.GetFiltered(p => p.IsActive);
            return productos ?? Enumerable.Empty<Product>();
        }

        // Obtener un producto activo por ID
        public async Task<Product> GetById(Guid id)
        {
            var product = await _productRepo.GetById(id);
            if (product is null || !product.IsActive)
                throw new EntityNotFoundException($"No se encontro el producto con ID: {id}");
            return product;
        }

        // Agregar un nuevo producto (validacion de SKU unico)
        public async Task<Product> Add(Product entity)
        {
            var existente = await _productRepo.First(p => p.Sku == entity.Sku);
            if (existente is not null)
                throw new DuplicatedEntityException($"Ya existe un producto con el SKU: {entity.Sku}");

            entity.IsActive = true;
            return await _productRepo.Add(entity);
        }

        // Actualizar un producto existente
        public async Task<Product> Update(Product entity)
        {
            var original = await _productRepo.GetById(entity.Id);
            if (original is null)
                throw new EntityNotFoundException($"No se encontro el producto con ID: {entity.Id}");

            if (!original.IsActive)
                throw new BusinessRuleViolationException("No se puede actualizar un producto inactivo.");

            // Validacion de SKU duplicado en otro producto
            var otroConMismoSku = await _productRepo.First(p => p.Sku == entity.Sku && p.Id != entity.Id);
            if (otroConMismoSku is not null)
                throw new DuplicatedEntityException($"Ya existe otro producto con el SKU: {entity.Sku}");

            return await _productRepo.Update(entity);
        }

        // Inhabilitar un producto (soft delete)
        public async Task Disable(Guid id)
        {
            var product = await _productRepo.GetById(id);
            if (product is null)
                throw new EntityNotFoundException($"No se encontro el producto con ID: {id}");

            product.IsActive = false;
            await _productRepo.Update(product);
        }

        // Borrar fisicamente un producto (solo si esta inactivo)
        public async Task<Product> Delete(Guid id)
        {
            var product = await _productRepo.GetById(id);
            if (product is null)
                throw new EntityNotFoundException($"No se encontro el producto con ID: {id}");

            if (product.IsActive)
                throw new BusinessRuleViolationException("Solo se pueden eliminar productos que ya estan inactivos.");

            return await _productRepo.Delete(product);
        }


    }
}
