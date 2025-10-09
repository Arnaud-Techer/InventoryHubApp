using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryHubApp.Shared.Models;

namespace InventoryHubApp.Server.Database.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.CategoryId);

            builder.Property(c => c.CategoryName)
                .IsRequired()
                .HasMaxLength(100);

            // Add unique constraint on CategoryName
            builder.HasIndex(c => c.CategoryName)
                .IsUnique();

            // Configure many-to-many relationship with Product
            builder.HasMany(c => c.Products)
                .WithMany(p => p.Categories)
                .UsingEntity(j => j.ToTable("ProductCategories"));
        }
    }
}
