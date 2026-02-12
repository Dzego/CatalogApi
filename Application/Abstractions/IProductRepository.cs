using System;
using CatalogApi.Domain.Models;

namespace CatalogApi.Application.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);
}
