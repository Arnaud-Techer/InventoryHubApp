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
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task<Supplier?> UpdateSupplierAsync(int supplierId, Supplier supplier)
        {
            var existingSupplier = await _context.Suppliers.FindAsync(supplierId);
            if (existingSupplier == null)
                return null;

            existingSupplier.SupplierName = supplier.SupplierName;
            existingSupplier.SupplierEmail = supplier.SupplierEmail;
            existingSupplier.SupplierAddress = supplier.SupplierAddress;
            existingSupplier.SupplierPhoneNumber = supplier.SupplierPhoneNumber;

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
