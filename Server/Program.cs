using InventoryHubApp.Shared.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy => policy.WithOrigins("http://localhost:5014").AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
Category category1 = new Category(101, "Electronics");
Category category2 = new Category(102, "Accessories");

Supplier supplier1 = new Supplier
{
    SupplierId = 201,
    SupplierName = "Tech Supplies Inc.",
    SupplierEmail = "contact@techsupplies.com",
    SupplierAddress = "123 Tech Street, Silicon Valley",
    SupplierPhoneNumber = "123-456-7890"
};

Supplier supplier2 = new Supplier
{
    SupplierId = 202,
    SupplierName = "Audio World",
    SupplierEmail = "support@audioworld.com",
    SupplierAddress = "456 Audio Lane, Music City",
    SupplierPhoneNumber = "987-654-3210"
};

Product product1 = new Product(1,
    "Laptop",
    1200.50m,
    25,
    new List<Category> { category1 },
    new List<Supplier> { supplier1 });

Product product2 = new Product(2, "Headphones",
    50.00m,
    100,
    new List<Category> { category2 },
    new List<Supplier> { supplier2 });

Product[] products = {product1, product2 };

app.MapGet("/api/products", () =>
{
    return products;
});

app.MapControllers();
app.UseCors("AllowBlazorClient");
app.Run();
