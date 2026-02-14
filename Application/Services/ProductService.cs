using System;
using CatalogApi.Application.Abstractions;
using CatalogApi.Application.Validation;
using CatalogApi.Contracts.Dtos;

namespace CatalogApi.Application.Services;

public class ProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<PagedResult<ProductDto>> GetAllAsync(ProductQuery query, CancellationToken ct)
    {
        var (page, pageSize) = PagingGuard.Normalize(query.Page, query.PageSize);

        var products = await _repo.GetAllAsync(ct);

        // Filtering
        IEnumerable<Domain.Models.Product> filtered = products;

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            filtered = filtered.Where(p =>
                string.Equals(p.Category, query.Category, StringComparison.OrdinalIgnoreCase));
        }

        if (query.InStock is not null)
        {
            filtered = filtered.Where(p => p.InStock == query.InStock.Value);
        }

        // Count before pagination
        var totalCount = filtered.Count();

        // Pagination
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var pageItems = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto(p.Id, p.Name, p.Category, p.Price, p.InStock))
            .ToList();

        return new PagedResult<ProductDto>(
            Items: pageItems,
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount,
            TotalPages: totalPages
        );
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var p = await _repo.GetByIdAsync(id, ct);
        return p is null ? null : new ProductDto(p.Id, p.Name, p.Category, p.Price, p.InStock);
    }

}
