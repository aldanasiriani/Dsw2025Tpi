using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Application.Services;

public class ProductsManagementService
{
    private readonly IRepository<Product> _productRepo;

    public ProductsManagementService(IRepository<Product> productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<IEnumerable<Product>?> GetAll()
        => await _productRepo.GetAll();

    public async Task<Product?> GetById(Guid id)
        => await _productRepo.GetById(id);

    public async Task<Product> Add(Product entity)
        => await _productRepo.Add(entity);

    public async Task<Product> Update(Product entity)
        => await _productRepo.Update(entity);

    public async Task Delete(Product entity)
        => await _productRepo.Delete(entity);
}
