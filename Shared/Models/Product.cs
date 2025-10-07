namespace InventoryHubApp.Shared.Models
{
    public sealed class Product
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public List<Category> Categories { get; set; }
        public List<Supplier> Suppliers { get; set; }

        public Product(int productId, string productName, decimal price, int stock, List<Category> categories, List<Supplier> suppliers)
        {
            ProductId = productId;
            ProductName = productName;
            Price = price;
            Stock = stock;
            Categories = categories;
            Suppliers = suppliers;
        }
    }

    public sealed class Category
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public Category(int categoryId, string categoryName)
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
        }
    }

    public sealed class Supplier
    {
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierEmail { get; set; }
        public string? SupplierAddress { get; set; }
        public string? SupplierPhoneNumber { get; set; }
    }
}