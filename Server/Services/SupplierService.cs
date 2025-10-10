using InventoryHubApp.Shared.Models;
using InventoryHubApp.Server.Database;
using Microsoft.EntityFrameworkCore;

namespace InventoryHubApp.Server.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly InventoryDbContext _context;

        public SupplierService(InventoryDbContext context)
        {
            _context = context;
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
            return existingSupplier;
        }

        public async Task<bool> DeleteSupplierAsync(int supplierId)
        {
            var supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier == null)
                return false;

            _context.Suppliers.Remove(supplier);
            await _context.SaveChangesAsync();
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
    }
}
