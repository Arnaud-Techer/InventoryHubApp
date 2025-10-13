using InventoryHubApp.Shared.Models;
using InventoryHubApp.Server.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InventoryHubApp.Server.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly InventoryDbContext _context;
        private readonly IMemoryCache _cache;

        // Cache configuration
        private const string CACHE_KEY_FIRST_PAGE_SUPPLIERS = "first_page_suppliers";
        private const int CACHE_EXPIRATION_MINUTES = 30;

        public SupplierService(InventoryDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        private void InvalidateFirstPageCache()
        {
            _cache.Remove(CACHE_KEY_FIRST_PAGE_SUPPLIERS);
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .ToListAsync();
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int supplierId)
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId);
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            // Create a new supplier without navigation properties
            var newSupplier = new Supplier
            {
                SupplierName = supplier.SupplierName,
                SupplierEmail = supplier.SupplierEmail,
                SupplierPhoneNumber = supplier.SupplierPhoneNumber,
                SupplierAddress = supplier.SupplierAddress
            };

            _context.Suppliers.Add(newSupplier);
            await _context.SaveChangesAsync();

            // Now add the relationships with existing products
            if (supplier.Products?.Any() == true)
            {
                var productIds = supplier.Products.Select(p => p.ProductId).ToList();
                var existingProducts = await _context.Products
                    .Where(p => productIds.Contains(p.ProductId))
                    .ToListAsync();
                
                foreach (var product in existingProducts)
                {
                    newSupplier.Products.Add(product);
                }
            }

            await _context.SaveChangesAsync();
            
            // Invalidate cache after creating a new supplier
            InvalidateFirstPageCache();
            
            return newSupplier;
        }

        public async Task<Supplier?> UpdateSupplierAsync(int supplierId, Supplier supplier)
        {
            var existingSupplier = await _context.Suppliers
                .Include(s => s.Products)
                .FirstOrDefaultAsync(s => s.SupplierId == supplierId);

            if (existingSupplier == null)
                return null;

            // Update basic properties
            existingSupplier.SupplierName = supplier.SupplierName;
            existingSupplier.SupplierEmail = supplier.SupplierEmail;
            existingSupplier.SupplierAddress = supplier.SupplierAddress;
            existingSupplier.SupplierPhoneNumber = supplier.SupplierPhoneNumber;

            // Update products if provided
            if (supplier.Products != null)
            {
                existingSupplier.Products.Clear();
                var productIds = supplier.Products.Select(p => p.ProductId).ToList();
                var existingProducts = await _context.Products
                    .Where(p => productIds.Contains(p.ProductId))
                    .ToListAsync();
                
                foreach (var product in existingProducts)
                {
                    existingSupplier.Products.Add(product);
                }
            }

            await _context.SaveChangesAsync();
            
            // Invalidate cache after updating a supplier
            InvalidateFirstPageCache();
            
            return existingSupplier;
        }

        public async Task<bool> DeleteSupplierAsync(int supplierId)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier == null)
                return false;

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
            
            // Invalidate cache after deleting a supplier
            InvalidateFirstPageCache();
            
            return true;
        }

        public async Task<IEnumerable<Supplier>> SearchSuppliersByNameAsync(string name)
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .Where(s => s.SupplierName != null && 
                           s.SupplierName.Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Supplier>> GetSuppliersByEmailAsync(string email)
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .Where(s => s.SupplierEmail != null && 
                           s.SupplierEmail.Equals(email))
                .ToListAsync();
        }

        public async Task<PaginationResponse<Supplier>> GetSuppliersPaginatedAsync(int pageNumber = 1, int pageSize = 6)
        {
            // Check if this is page 1 with default page size (cache scenario)
            if (pageNumber == 1 && pageSize == 6)
            {
                // Try to get from cache first
                if (_cache.TryGetValue(CACHE_KEY_FIRST_PAGE_SUPPLIERS, out PaginationResponse<Supplier>? cachedResponse))
                {
                    return cachedResponse;
                }
            }

            // Fetch from database
            var totalCount = await _context.Suppliers.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            
            var suppliers = await _context.Suppliers
                .Include(s => s.Products)
                .OrderBy(s => s.SupplierId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            var response = new PaginationResponse<Supplier>
            {
                Items = suppliers,
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

                _cache.Set(CACHE_KEY_FIRST_PAGE_SUPPLIERS, response, cacheOptions);
            }

            return response;
        }
    }
}
