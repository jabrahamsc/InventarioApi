namespace InventarioApi.DTOs;

public record SaleItemRequest(int ProductId, int Quantity);

public record CreateSaleRequest(List<SaleItemRequest> Items);

public record SaleItemResponse(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal);

public record SaleResponse(
    int Id,
    DateTime SaleDate,
    decimal TotalAmount,
    string SoldBy,
    List<SaleItemResponse> Items);

public record LowStockProductResponse(
    int Id,
    string Name,
    decimal Price,
    int CurrentStock,
    int MinimumStock,
    int StockDeficit);