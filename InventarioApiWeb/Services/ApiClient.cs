using System;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using InventarioApi.Web.Models;

namespace InventarioApi.Web.Services;

public interface IApiClient
{
    Task<(string? Token, string? Role)> LoginAsync(string username, string password);
    Task<List<ProductViewModel>> GetProductsAsync(string token);
    Task<ProductViewModel?> GetProductByIdAsync(int id, string token);
    Task<bool> CreateProductAsync(CreateProductViewModel model, string token);
    Task<bool> UpdateProductAsync(int id, CreateProductViewModel model, string token);
    Task<bool> DeleteProductAsync(int id, string token);
    Task<List<LowStockViewModel>> GetLowStockAsync(string token);
    Task<SaleViewModel?> CreateSaleAsync(RegisterSaleViewModel model, string token);
    Task<List<SaleViewModel>> GetSalesAsync(string token);
}

public class ApiClient : IApiClient
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOpts =
        new() { PropertyNameCaseInsensitive = true };

    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiClient(HttpClient http, IConfiguration config, IHttpContextAccessor httpContextAccessor)
    {
        _http = http;
        _http.BaseAddress = new Uri(config["ApiBaseUrl"]
            ?? "https://localhost:7001/");
        _httpContextAccessor = httpContextAccessor;
    }
    private bool IsUnauthorized(HttpResponseMessage response)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
            return false;

        _httpContextAccessor.HttpContext?.Session.Clear();
        return true;
    }

    private void SetAuth(string token) =>
        _http.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

    private StringContent Json<T>(T obj) =>
        new(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    public async Task<(string? Token, string? Role)> LoginAsync(string username, string password)
    {
        var response = await _http.PostAsync("api/auth/login",
            Json(new { username, password }));

        if (!response.IsSuccessStatusCode) return (null, null);

        var content = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(content);
        var data = doc.RootElement.GetProperty("data");

        return (
            data.GetProperty("token").GetString(),
            data.GetProperty("role").GetString()
        );
    }

    public async Task<List<ProductViewModel>> GetProductsAsync(string token)
    {
        SetAuth(token);
        var response = await _http.GetAsync("api/products");

        if (IsUnauthorized(response)) return new();

        if (!response.IsSuccessStatusCode) return new();

        var content = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(content);
        return JsonSerializer.Deserialize<List<ProductViewModel>>(
            doc.RootElement.GetProperty("data").GetRawText(), _jsonOpts) ?? new();
    }

    public async Task<ProductViewModel?> GetProductByIdAsync(int id, string token)
    {
        SetAuth(token);
        var response = await _http.GetAsync($"api/products/{id}");
        if (!response.IsSuccessStatusCode) return null;

        if (IsUnauthorized(response)) return null;

        var content = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(content);
        return JsonSerializer.Deserialize<ProductViewModel>(
            doc.RootElement.GetProperty("data").GetRawText(), _jsonOpts);
    }

    public async Task<bool> CreateProductAsync(CreateProductViewModel model, string token)
    {
        SetAuth(token);
        var response = await _http.PostAsync("api/products", Json(new
        {
            name = model.Name,
            price = model.Price,
            currentStock = model.CurrentStock,
            minimumStock = model.MinimumStock
        }));

        if (IsUnauthorized(response)) return false;

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateProductAsync(int id, CreateProductViewModel model, string token)
    {
        SetAuth(token);
        var response = await _http.PutAsync($"api/products/{id}", Json(new
        {
            name = model.Name,
            price = model.Price,
            currentStock = model.CurrentStock,
            minimumStock = model.MinimumStock
        }));

        if (IsUnauthorized(response)) return false;
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteProductAsync(int id, string token)
    {
        SetAuth(token);
        var response = await _http.DeleteAsync($"api/products/{id}");

        if (IsUnauthorized(response)) return false;
        return response.IsSuccessStatusCode;
    }

    public async Task<List<LowStockViewModel>> GetLowStockAsync(string token)
    {
        SetAuth(token);
        var response = await _http.GetAsync("api/reports/low-stock");
        if (IsUnauthorized(response)) return new();

        if (!response.IsSuccessStatusCode) return new();

        var content = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(content);
        return JsonSerializer.Deserialize<List<LowStockViewModel>>(
            doc.RootElement.GetProperty("data").GetRawText(), _jsonOpts) ?? new();
    }

    public async Task<SaleViewModel?> CreateSaleAsync(RegisterSaleViewModel model, string token)
    {
        SetAuth(token);
        var items = model.Lines
            .Select(l => new { productId = l.ProductId, quantity = l.Quantity });

        var response = await _http.PostAsync("api/sales", Json(new { items }));

        if (IsUnauthorized(response)) return null;
        if (!response.IsSuccessStatusCode) return null;

        var content = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(content);
        return JsonSerializer.Deserialize<SaleViewModel>(
            doc.RootElement.GetProperty("data").GetRawText(), _jsonOpts);
    }

    public async Task<List<SaleViewModel>> GetSalesAsync(string token)
    {
        SetAuth(token);
        var response = await _http.GetAsync("api/sales");

        if (IsUnauthorized(response)) return new();
        if (!response.IsSuccessStatusCode) return new();

        var content = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(content);
        return JsonSerializer.Deserialize<List<SaleViewModel>>(
            doc.RootElement.GetProperty("data").GetRawText(), _jsonOpts) ?? new();
    }
}
