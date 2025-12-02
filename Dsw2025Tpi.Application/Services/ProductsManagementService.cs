using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Application.Dtos;
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

      // Modificamos la firma para aceptar page, pageSize y el filtro
public async Task<PagedResult<Product>> GetAll(int page, int pageSize, string? name, bool? isActive)
{
    // 1. Traemos los datos del repositorio
    // Nota: Dependiendo de tu repositorio, esto trae todo a memoria o prepara la consulta.
    var allProducts = await _productRepo.GetAll(); 

    // 2. Lógica de Búsqueda por Nombre o SKU (Nuevo)
    if (!string.IsNullOrEmpty(name))
    {
        // Convertimos a minúsculas para buscar sin importar mayúsculas
        var term = name.ToLower();
        allProducts = allProducts.Where(p => 
            (p.Name != null && p.Name.ToLower().Contains(term)) || 
            (p.Sku != null && p.Sku.ToLower().Contains(term))
        );
    }

    // 3. Lógica de Estado (Nuevo filtro Activo/Inactivo/Todos)
    // - Si isActive es TRUE: Muestra solo activos (Lo que verá el Customer o el Admin filtrando "Activo")
    // - Si isActive es FALSE: Muestra solo inactivos (Admin filtrando "Inactivo")
    // - Si isActive es NULL: No entra aquí y muestra TODO (Admin filtrando "Todos")
    if (isActive.HasValue)
    {
        allProducts = allProducts.Where(p => p.IsActive == isActive.Value);
    }

    // Nota: He quitado la validación de 'StockQuantity > 0' que tenías antes.
    // Razón: En el panel de Admin, a veces necesitas ver productos Activos 
    // pero sin stock para saber que tienes que reponerlos.

    // 4. Conteo total (Para la paginación)
    var totalCount = allProducts.Count();

    // 5. Aplicar Paginación
    var items = allProducts
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

    return new PagedResult<Product>
    {
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize,
        Items = items
    };
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
