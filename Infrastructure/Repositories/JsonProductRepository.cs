using System;
using System.Text.Json;
using CatalogApi.Domain.Models;

namespace CatalogApi.Infrastructure.Repositories;

public class JsonProductRepository
{
    private readonly IWebHostEnvironment _env;

    public JsonProductRepository(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct)
    {
        var path = Path.Combine(_env.ContentRootPath, "Data", "products.json");

        if (!File.Exists(path))
            return Array.Empty<Product>();

        await using var stream = File.OpenRead(path);

        var products = await JsonSerializer.DeserializeAsync<List<Product>>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
            ct
        );

        return products ?? (IReadOnlyList<Product>)Array.Empty<Product>();
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct)
    {
        var all = await GetAllAsync(ct);
        return all.FirstOrDefault(p => p.Id == id);
    }
}
