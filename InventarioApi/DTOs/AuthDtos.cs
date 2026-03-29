namespace InventarioApi.DTOs;

public record LoginRequest(string Username, string Password);

public record LoginResponse(
    string Token,
    string Username,
    string Role,
    DateTime ExpiresAt);

public record RegisterRequest(
    string Username,
    string Email,
    string Password,
    string Role);