using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryHubApp.Shared.Models;

namespace InventoryHubApp.Server.Database.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.HasKey(s => s.SupplierId);

            builder.Property(s => s.SupplierName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.SupplierEmail)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(s => s.SupplierAddress)
                .HasMaxLength(500);

            builder.Property(s => s.SupplierPhoneNumber)
                .HasMaxLength(20);

            // Add unique constraint on SupplierEmail
            builder.HasIndex(s => s.SupplierEmail)
                .IsUnique();

            // Add index on SupplierName for search functionality
            builder.HasIndex(s => s.SupplierName);

            // Configure many-to-many relationship with Product
            builder.HasMany(s => s.Products)
                .WithMany(p => p.Suppliers)
                .UsingEntity(j => j.ToTable("ProductSuppliers"));
        }
    }
}
