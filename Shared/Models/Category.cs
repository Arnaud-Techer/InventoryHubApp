namespace InventoryHubApp.Shared.Models;
using System.Text.Json.Serialization;

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