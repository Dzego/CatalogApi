using System;

namespace CatalogApi.Contracts.Dtos;

public record ProductCreateDto(
    string Name,
    string Category,
    decimal Price,
    bool InStock
);

