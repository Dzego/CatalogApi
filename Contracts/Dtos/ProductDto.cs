using System;

namespace CatalogApi.Contracts.Dtos;

public record ProductDto(int Id, string Name, string Category, decimal Price, bool InStock);

