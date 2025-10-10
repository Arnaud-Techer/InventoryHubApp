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
            // Create a new product without navigation properties
            var newProduct = new Product
            {
                ProductName = product.ProductName,
                Price = product.Price,
                Stock = product.Stock
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            // Now add the relationships with existing categories and suppliers
            if (product.Categories?.Any() == true)
            {
                var categoryIds = product.Categories.Select(c => c.CategoryId).ToList();
                var existingCategories = await _context.Categories
                    .Where(c => categoryIds.Contains(c.CategoryId))
                    .ToListAsync();
                
                foreach (var category in existingCategories)
                {
                    newProduct.Categories.Add(category);
                }
            }

            if (product.Suppliers?.Any() == true)
            {
                var supplierIds = product.Suppliers.Select(s => s.SupplierId).ToList();
                var existingSuppliers = await _context.Suppliers
                    .Where(s => supplierIds.Contains(s.SupplierId))
                    .ToListAsync();
                
                foreach (var supplier in existingSuppliers)
                {
                    newProduct.Suppliers.Add(supplier);
                }
            }

            await _context.SaveChangesAsync();
            return newProduct;
        }

        public async Task<Product?> UpdateProductAsync(int productId, Product product)
        {
            var existingProduct = await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Suppliers)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (existingProduct == null)
                return null;

            // Update basic properties
            existingProduct.ProductName = product.ProductName;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;

            // Update categories if provided
            if (product.Categories != null && product.Categories.Any())
            {
                existingProduct.Categories.Clear();
                var categoryIds = product.Categories.Select(c => c.CategoryId).ToList();
                var existingCategories = await _context.Categories
                    .Where(c => categoryIds.Contains(c.CategoryId))
                    .ToListAsync();
                
                foreach (var category in existingCategories)
                {
                    existingProduct.Categories.Add(category);
                }
            }

            // Update suppliers if provided
            if (product.Suppliers != null && product.Suppliers.Any())
            {
                existingProduct.Suppliers.Clear();
                var supplierIds = product.Suppliers.Select(s => s.SupplierId).ToList();
                var existingSuppliers = await _context.Suppliers
                    .Where(s => supplierIds.Contains(s.SupplierId))
                    .ToListAsync();
                
                foreach (var supplier in existingSuppliers)
                {
                    existingProduct.Suppliers.Add(supplier);
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
