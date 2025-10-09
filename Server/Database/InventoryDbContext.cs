using Microsoft.EntityFrameworkCore;
using InventoryHubApp.Shared.Models;
using InventoryHubApp.Server.Database.Configurations;

namespace InventoryHubApp.Server.Database
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new SupplierConfiguration());

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Electronics" },
                new Category { CategoryId = 2, CategoryName = "Furniture" },
                new Category { CategoryId = 3, CategoryName = "Accessories" }
            );

            // Seed Suppliers
            modelBuilder.Entity<Supplier>().HasData(
                new Supplier
                {
                    SupplierId = 1,
                    SupplierName = "TechCorp",
                    SupplierEmail = "contact@techcorp.com",
                    SupplierAddress = "123 Tech Street, Silicon Valley, CA 94000",
                    SupplierPhoneNumber = "+1-555-0123"
                },
                new Supplier
                {
                    SupplierId = 2,
                    SupplierName = "OfficeSupply Inc",
                    SupplierEmail = "orders@officesupply.com",
                    SupplierAddress = "456 Business Ave, New York, NY 10001",
                    SupplierPhoneNumber = "+1-555-0456"
                },
                new Supplier
                {
                    SupplierId = 3,
                    SupplierName = "Global Electronics",
                    SupplierEmail = "sales@globalelectronics.com",
                    SupplierAddress = "789 Innovation Blvd, Austin, TX 73301",
                    SupplierPhoneNumber = "+1-555-0789"
                }
            );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    ProductId = 1,
                    ProductName = "Laptop",
                    Price = 999.99m,
                    Stock = 15
                },
                new Product
                {
                    ProductId = 2,
                    ProductName = "Office Chair",
                    Price = 299.99m,
                    Stock = 8
                },
                new Product
                {
                    ProductId = 3,
                    ProductName = "Wireless Mouse",
                    Price = 29.99m,
                    Stock = 25
                }
            );

            // Note: Many-to-many relationships will be handled by the application logic
            // or through separate seeding methods after the database is created
        }
    }
}
