using InventoryHubApp.Shared.Models;
using InventoryHubApp.Server.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryHubApp.Server.Services
{
    public class ProductService : IProductService
    {
        private readonly InventoryDbContext _context;

        public ProductService(InventoryDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Suppliers)
                .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            return await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Suppliers)
                .FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateProductAsync(int productId, Product product)
        {
            var existingProduct = await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Suppliers)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (existingProduct == null)
                return null;

            existingProduct.ProductName = product.ProductName;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;

            // Update categories if provided
            if (product.Categories != null && product.Categories.Any())
            {
                existingProduct.Categories.Clear();
                foreach (var category in product.Categories)
                {
                    var existingCategory = await _context.Categories.FindAsync(category.CategoryId);
                    if (existingCategory != null)
                    {
                        existingProduct.Categories.Add(existingCategory);
                    }
                }
            }

            // Update suppliers if provided
            if (product.Suppliers != null && product.Suppliers.Any())
            {
                existingProduct.Suppliers.Clear();
                foreach (var supplier in product.Suppliers)
                {
                    var existingSupplier = await _context.Suppliers.FindAsync(supplier.SupplierId);
                    if (existingSupplier != null)
                    {
                        existingProduct.Suppliers.Add(existingSupplier);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Suppliers)
                .Where(p => p.Categories.Any(c => c.CategoryId == categoryId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsBySupplierAsync(int supplierId)
        {
            return await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Suppliers)
                .Where(p => p.Suppliers.Any(s => s.SupplierId == supplierId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10)
        {
            return await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Suppliers)
                .Where(p => p.Stock <= threshold)
                .ToListAsync();
        }
    }
}
