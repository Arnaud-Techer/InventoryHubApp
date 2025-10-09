using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryHubApp.Shared.Models;

namespace InventoryHubApp.Server.Database.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.ProductId);

            builder.Property(p => p.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.Stock)
                .IsRequired();

            // Configure many-to-many relationship with Category
            builder.HasMany(p => p.Categories)
                .WithMany(c => c.Products)
                .UsingEntity(j => j.ToTable("ProductCategories"));

            // Configure many-to-many relationship with Supplier
            builder.HasMany(p => p.Suppliers)
                .WithMany(s => s.Products)
                .UsingEntity(j => j.ToTable("ProductSuppliers"));

            // Add indexes for better performance
            builder.HasIndex(p => p.ProductName);
            builder.HasIndex(p => p.Price);
            builder.HasIndex(p => p.Stock);
        }
    }
}
