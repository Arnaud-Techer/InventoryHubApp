using System.Net.Http.Json;
using InventoryHubApp.Shared.Models;

namespace InventoryHubApp.Client.Services;

public class ApiOptions
{
    public const string Api = "Api";
    public required string BaseAddress { get; set; }
}
public interface IApiService
{
    public Task<Product[]?> GetProductsAsync();
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Product[]?> GetProductsAsync()
    {
        return await _httpClient.GetFromJsonAsync<Product[]>("/api/products");
    }
}