using Microsoft.AspNetCore.Mvc;
using InventoryHubApp.Server.Database;
using Microsoft.EntityFrameworkCore;
using InventoryHubApp.Shared.Models;

namespace InventoryHubApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public CategoryController(InventoryDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Get categories with product counts
        /// </summary>
        [HttpGet("with-counts")]
        public async Task<ActionResult<IEnumerable<CategorySummary>>> GetCategoriesWithCounts()
        {
            var summaries = await _context.Categories
                .Select(c => new CategorySummary
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    ProductCount = c.Products.Count()
                })
                .ToListAsync();

            return Ok(summaries);
        }

        /// <summary>
        /// Get a category by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(category);
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {
            if (category == null)
            {
                return BadRequest("Category data is required.");
            }

            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                return BadRequest("Category name is required.");
            }

            // Check if category already exists
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName == category.CategoryName);
            
            if (existingCategory != null)
            {
                return Conflict($"A category with the name '{category.CategoryName}' already exists.");
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.CategoryId }, category);
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
