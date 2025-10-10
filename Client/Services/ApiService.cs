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
    public Task<Product?> GetProductByIdAsync(int id);
    public Task<Product?> CreateProductAsync(Product product);
    public Task<Product?> UpdateProductAsync(int id, Product product);
    public Task<bool> DeleteProductAsync(int id);
    public Task<Category[]?> GetCategoriesAsync();
    public Task<Supplier[]?> GetSuppliersAsync();
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
        try
        {
            return await _httpClient.GetFromJsonAsync<Product[]>("/api/product");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to connect to the API. Please ensure the server is running. Error: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            throw new Exception($"Request timed out. Please check your connection and try again. Error: {ex.Message}");
        }
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Product>($"/api/product/{id}");
    }

    public async Task<Product?> CreateProductAsync(Product product)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/product", product);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Product>();
        }
        return null;
    }

    public async Task<Product?> UpdateProductAsync(int id, Product product)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/product/{id}", product);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<Product>();
        }
        return null;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"/api/product/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<Category[]?> GetCategoriesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Category[]>("/api/category");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to fetch categories. Error: {ex.Message}");
        }
    }

    public async Task<Supplier[]?> GetSuppliersAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Supplier[]>("/api/supplier");
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Failed to fetch suppliers. Error: {ex.Message}");
        }
    }
}