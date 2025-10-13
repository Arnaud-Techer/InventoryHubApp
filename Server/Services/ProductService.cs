using InventoryHubApp.Shared.Models;
using InventoryHubApp.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InventoryHubApp.Server.Services
{
    public class ProductService : IProductService
    {
        private readonly InventoryDbContext _context;
        private readonly IMemoryCache _cache;
        private const string CACHE_KEY_FIRST_PAGE_PRODUCTS = "first_page_products";
        private const int CACHE_EXPIRATION_MINUTES = 30;

        public ProductService(InventoryDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        private void InvalidateFirstPageCache()
        {
            _cache.Remove(CACHE_KEY_FIRST_PAGE_PRODUCTS);
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
            
            // Invalidate cache since we added a new product
            InvalidateFirstPageCache();
            
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
            
            // Invalidate cache since we updated a product
            InvalidateFirstPageCache();
            
            return existingProduct;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            
            // Invalidate cache since we deleted a product
            InvalidateFirstPageCache();
            
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

        public async Task<PaginationResponse<Product>> GetProductsPaginatedAsync(int pageNumber = 1, int pageSize = 6)
        {
            // Check if this is page 1 with default page size (cache scenario)
            if (pageNumber == 1 && pageSize == 6)
            {
                // Try to get from cache first
                if (_cache.TryGetValue(CACHE_KEY_FIRST_PAGE_PRODUCTS, out PaginationResponse<Product>? cachedResponse))
                {
                    return cachedResponse;
                }
            }

            // Fetch from database
            var totalCount = await _context.Products.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var products = await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Suppliers)
                .OrderBy(p => p.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            var response = new PaginationResponse<Product>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasNextPage = pageNumber < totalPages,
                HasPreviousPage = pageNumber > 1
            };

            // Cache the first page if it's page 1 with default page size
            if (pageNumber == 1 && pageSize == 6)
            {
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(CACHE_EXPIRATION_MINUTES),
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(CACHE_KEY_FIRST_PAGE_PRODUCTS, response, cacheOptions);
            }

            return response;
        }
    }
}
