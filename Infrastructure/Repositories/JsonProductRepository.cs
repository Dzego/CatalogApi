using System;
using System.Text.Json;
using CatalogApi.Application.Abstractions;
using CatalogApi.Domain.Models;
using Microsoft.AspNetCore.SignalR;

namespace CatalogApi.Infrastructure.Repositories;

public class JsonProductRepository : IProductRepository
{
    private readonly IWebHostEnvironment _env;
    private static readonly SemaphoreSlim _fileLock = new(1, 1);
    
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
     
    public async Task<Product> AddAsync(Product product, CancellationToken ct)
    {
        var path = Path.Combine(_env.ContentRootPath, "Data", "products.json");
        await _fileLock.WaitAsync(ct);
        try
        {
            var list = (await ReadListAsync(path, ct)) ?? new List<Product>();

            var nextId = list.Count == 0 ? 1 : list.Max(p => p.Id) + 1;

            var created = new Product
            {
                Id = nextId,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                InStock = product.InStock
            };

            list.Add(created);
            await WriteListAsync(path, list, ct);
            return created;
        }
        finally
        {
            _fileLock.Release();
        }
    }

      private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };


    private async Task<List<Product>?> ReadListAsync(string path, CancellationToken ct)
    {
        if (!File.Exists(path)) return new List<Product>();

        await using var stream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<List<Product>>(stream, _jsonOptions, ct);
    }
    
    public async Task<bool> UpdateAsync(Product product, CancellationToken ct)
    {
        var path = Path.Combine(_env.ContentRootPath, "Data", "products.json");
        await _fileLock.WaitAsync(ct);
        try
        {
            var list = (await ReadListAsync(path, ct)) ?? new List<Product>();

            var idx = list.FindIndex(p => p.Id == product.Id);
            if (idx < 0) return false;

            list[idx] = product;
            await WriteListAsync(path, list, ct);
            return true;
        }
        finally
        {
            _fileLock.Release();
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var path = Path.Combine(_env.ContentRootPath, "Data", "products.json");
        await _fileLock.WaitAsync(ct);
        try
        {
            var list = (await ReadListAsync(path, ct)) ?? new List<Product>();

            var removed = list.RemoveAll(p => p.Id == id) > 0;
            if (!removed) return false;

            await WriteListAsync(path, list, ct);
            return true;
        }
        finally
        {
            _fileLock.Release();
        }
    }
    
    private async Task WriteListAsync(string path, List<Product> list, CancellationToken ct)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        await using var stream = File.Create(path);
        await JsonSerializer.SerializeAsync(stream, list, _jsonOptions, ct);

    }

}
