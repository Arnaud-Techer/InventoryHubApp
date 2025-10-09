using InventoryHubApp.Shared.Models;

namespace InventoryHubApp.Server.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<Supplier?> GetSupplierByIdAsync(int supplierId);
        Task<Supplier> CreateSupplierAsync(Supplier supplier);
        Task<Supplier?> UpdateSupplierAsync(int supplierId, Supplier supplier);
        Task<bool> DeleteSupplierAsync(int supplierId);
        Task<IEnumerable<Supplier>> SearchSuppliersByNameAsync(string name);
        Task<IEnumerable<Supplier>> GetSuppliersByEmailAsync(string email);
    }
}
