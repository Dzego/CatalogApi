using System;

namespace CatalogApi.Domain.Models;

public class Product
{
    public int Id { get; init; }
    public string Name { get; init; } = "";
    public string Category { get; init; } = "";
    public decimal Price { get; init; }
    public bool InStock { get; init; }
}
