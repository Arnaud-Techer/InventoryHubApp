using System.Text.Json.Serialization;

namespace InventoryHubApp.Shared.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<Supplier> Suppliers { get; set; } = new List<Supplier>();

        // Parameterless constructor for EF Core
        public Product() { }

        // Constructor for convenience
        public Product(string productName, decimal price, int stock)
        {
            ProductName = productName;
            Price = price;
            Stock = stock;
        }
    }

    public class Category
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        // Parameterless constructor for EF Core
        public Category() { }

        // Constructor for convenience
        public Category(string categoryName)
        {
            CategoryName = categoryName;
        }
    }

    public class Supplier
    {
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierEmail { get; set; }
        public string? SupplierAddress { get; set; }
        public string? SupplierPhoneNumber { get; set; }
        
        [JsonIgnore]
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
}