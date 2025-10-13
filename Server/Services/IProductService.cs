using InventoryHubApp.Shared.Models;

namespace InventoryHubApp.Server.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int productId);
        Task<Product> CreateProductAsync(Product product);
        Task<Product?> UpdateProductAsync(int productId, Product product);
        Task<bool> DeleteProductAsync(int productId);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetProductsBySupplierAsync(int supplierId);
        Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10);
        Task<PaginationResponse<Product>> GetProductsPaginatedAsync(int pageNumber = 1, int pageSize = 6);
    }
}
