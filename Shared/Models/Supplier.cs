namespace InventoryHubApp.Shared.Models;
using System.Text.Json.Serialization;

public class Supplier
    {
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierEmail { get; set; }
        public string? SupplierAddress { get; set; }
        public string? SupplierPhoneNumber { get; set; }
        
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        // Parameterless constructor for EF Core
        public Supplier() { }

        // Constructor for convenience
        public Supplier(string supplierName, string supplierEmail)
        {
            SupplierName = supplierName;
            SupplierEmail = supplierEmail;
        }
    }