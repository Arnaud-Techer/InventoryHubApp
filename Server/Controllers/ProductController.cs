using Microsoft.AspNetCore.Mvc;
using InventoryHubApp.Server.Services;
using InventoryHubApp.Shared.Models;

namespace InventoryHubApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get all products
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        /// <summary>
        /// Get a product by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }
            return Ok(product);
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product data is required.");
            }

            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                return BadRequest("Product name is required.");
            }

            if (product.Price < 0)
            {
                return BadRequest("Price cannot be negative.");
            }

            if (product.Stock < 0)
            {
                return BadRequest("Stock cannot be negative.");
            }

            var createdProduct = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.ProductId }, createdProduct);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, [FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest("Product data is required.");
            }

            if (id != product.ProductId)
            {
                return BadRequest("Product ID mismatch.");
            }

            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                return BadRequest("Product name is required.");
            }

            if (product.Price < 0)
            {
                return BadRequest("Price cannot be negative.");
            }

            if (product.Stock < 0)
            {
                return BadRequest("Stock cannot be negative.");
            }

            var updatedProduct = await _productService.UpdateProductAsync(id, product);
            if (updatedProduct == null)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return Ok(updatedProduct);
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound($"Product with ID {id} not found.");
            }

            return NoContent();
        }

        /// <summary>
        /// Get products by category ID
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategoryAsync(categoryId);
            return Ok(products);
        }

        /// <summary>
        /// Get products by supplier ID
        /// </summary>
        [HttpGet("supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsBySupplier(int supplierId)
        {
            var products = await _productService.GetProductsBySupplierAsync(supplierId);
            return Ok(products);
        }

        /// <summary>
        /// Get products with low stock
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<Product>>> GetLowStockProducts([FromQuery] int threshold = 10)
        {
            var products = await _productService.GetLowStockProductsAsync(threshold);
            return Ok(products);
        }

        /// <summary>
        /// Get products with pagination
        /// </summary>
        [HttpGet("paginated")]
        public async Task<ActionResult<PaginationResponse<Product>>> GetProductsPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 6)
        {
            if (pageNumber < 1)
            {
                return BadRequest("Page number must be greater than 0.");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Page size must be between 1 and 100.");
            }

            var result = await _productService.GetProductsPaginatedAsync(pageNumber, pageSize);
            return Ok(result);
        }
    }
}
