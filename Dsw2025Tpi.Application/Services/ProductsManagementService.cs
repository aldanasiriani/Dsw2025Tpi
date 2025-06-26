using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Dsw2025Tpi.Data.Repositories;

namespace Dsw2025Tpi.Application.Services;

public class ProductsManagementService
{
    private readonly IRepository _repository;

    public ProductsManagementService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<T>?> GetAll<T>() where T : EntityBase
        => await _repository.GetAll<T>();

    public async Task<T?> GetById<T>(Guid id) where T : EntityBase
        => await _repository.GetById<T>(id);

    public async Task<T> Add<T>(T entity) where T : EntityBase
        => await _repository.Add(entity);

    public async Task<T> Update<T>(T entity) where T : EntityBase
        => await _repository.Update(entity);

    public async Task Delete<T>(T entity) where T : EntityBase
        => await _repository.Delete(entity);
}
