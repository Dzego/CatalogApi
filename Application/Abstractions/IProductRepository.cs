using System;
using CatalogApi.Domain.Models;

namespace CatalogApi.Application.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);

    Task<Product> AddAsync(Product product, CancellationToken ct);
    Task<bool> UpdateAsync(Product product, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}


