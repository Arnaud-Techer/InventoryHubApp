using InventoryHubApp.Server.Services;
using InventoryHubApp.Server.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Add Entity Framework
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    "Data Source=inventory.db"));

// Register custom services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy => policy.WithOrigins("http://localhost:5014").AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    context.Database.EnsureCreated();
    
    // Seed many-to-many relationships if they don't exist
    await SeedManyToManyRelationships(context);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");

app.MapControllers();
app.Run();

// Helper method to seed many-to-many relationships
static async Task SeedManyToManyRelationships(InventoryDbContext context)
{
    // Check if relationships already exist
    var laptop = await context.Products.FindAsync(1);
    var chair = await context.Products.FindAsync(2);
    var mouse = await context.Products.FindAsync(3);
    
    var electronics = await context.Categories.FindAsync(1);
    var furniture = await context.Categories.FindAsync(2);
    var accessories = await context.Categories.FindAsync(3);
    
    var techCorp = await context.Suppliers.FindAsync(1);
    var officeSupply = await context.Suppliers.FindAsync(2);
    var globalElectronics = await context.Suppliers.FindAsync(3);

    if (laptop != null && electronics != null && !laptop.Categories.Contains(electronics))
    {
        laptop.Categories.Add(electronics);
    }
    
    if (chair != null && furniture != null && !chair.Categories.Contains(furniture))
    {
        chair.Categories.Add(furniture);
    }
    
    if (mouse != null && electronics != null && !mouse.Categories.Contains(electronics))
    {
        mouse.Categories.Add(electronics);
    }
    
    if (mouse != null && accessories != null && !mouse.Categories.Contains(accessories))
    {
        mouse.Categories.Add(accessories);
    }

    if (laptop != null && techCorp != null && !laptop.Suppliers.Contains(techCorp))
    {
        laptop.Suppliers.Add(techCorp);
    }
    
    if (chair != null && officeSupply != null && !chair.Suppliers.Contains(officeSupply))
    {
        chair.Suppliers.Add(officeSupply);
    }
    
    if (mouse != null && techCorp != null && !mouse.Suppliers.Contains(techCorp))
    {
        mouse.Suppliers.Add(techCorp);
    }
    
    if (mouse != null && globalElectronics != null && !mouse.Suppliers.Contains(globalElectronics))
    {
        mouse.Suppliers.Add(globalElectronics);
    }

    await context.SaveChangesAsync();
}
