namespace InventarioApi.DTOs;

public record ProductResponse(
    int Id,
    string Name,
    decimal Price,
    int CurrentStock,
    int MinimumStock,
    bool RequiresRestocking,
    DateTime CreatedAt);

public record CreateProductRequest(
    string Name,
    decimal Price,
    int CurrentStock,
    int MinimumStock);

public record UpdateProductRequest(
    string Name,
    decimal Price,
    int CurrentStock,
    int MinimumStock);