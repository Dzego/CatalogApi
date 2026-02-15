using System;

namespace CatalogApi.Contracts.Dtos;
public record ProductUpdateDto(
    string Name,
    string Category,
    decimal Price,
    bool InStock
);

