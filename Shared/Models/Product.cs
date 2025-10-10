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

}