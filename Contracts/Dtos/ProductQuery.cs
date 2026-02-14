using System;

namespace CatalogApi.Contracts.Dtos;

public sealed record ProductQuery(
    string? Category,
    bool? InStock,
    int Page = 1,
    int PageSize = 10
);