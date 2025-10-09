using Microsoft.AspNetCore.Mvc;
using InventoryHubApp.Server.Services;
using InventoryHubApp.Shared.Models;

namespace InventoryHubApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        /// <summary>
        /// Get all suppliers
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            var suppliers = await _supplierService.GetAllSuppliersAsync();
            return Ok(suppliers);
        }

        /// <summary>
        /// Get a supplier by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Supplier>> GetSupplier(int id)
        {
            var supplier = await _supplierService.GetSupplierByIdAsync(id);
            if (supplier == null)
            {
                return NotFound($"Supplier with ID {id} not found.");
            }
            return Ok(supplier);
        }

        /// <summary>
        /// Create a new supplier
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Supplier>> CreateSupplier([FromBody] Supplier supplier)
        {
            if (supplier == null)
            {
                return BadRequest("Supplier data is required.");
            }

            if (string.IsNullOrWhiteSpace(supplier.SupplierName))
            {
                return BadRequest("Supplier name is required.");
            }

            if (string.IsNullOrWhiteSpace(supplier.SupplierEmail))
            {
                return BadRequest("Supplier email is required.");
            }

            // Basic email validation
            if (!IsValidEmail(supplier.SupplierEmail))
            {
                return BadRequest("Invalid email format.");
            }

            var createdSupplier = await _supplierService.CreateSupplierAsync(supplier);
            return CreatedAtAction(nameof(GetSupplier), new { id = createdSupplier.SupplierId }, createdSupplier);
        }

        /// <summary>
        /// Update an existing supplier
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Supplier>> UpdateSupplier(int id, [FromBody] Supplier supplier)
        {
            if (supplier == null)
            {
                return BadRequest("Supplier data is required.");
            }

            if (id != supplier.SupplierId)
            {
                return BadRequest("Supplier ID mismatch.");
            }

            if (string.IsNullOrWhiteSpace(supplier.SupplierName))
            {
                return BadRequest("Supplier name is required.");
            }

            if (string.IsNullOrWhiteSpace(supplier.SupplierEmail))
            {
                return BadRequest("Supplier email is required.");
            }

            // Basic email validation
            if (!IsValidEmail(supplier.SupplierEmail))
            {
                return BadRequest("Invalid email format.");
            }

            var updatedSupplier = await _supplierService.UpdateSupplierAsync(id, supplier);
            if (updatedSupplier == null)
            {
                return NotFound($"Supplier with ID {id} not found.");
            }

            return Ok(updatedSupplier);
        }

        /// <summary>
        /// Delete a supplier
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSupplier(int id)
        {
            var result = await _supplierService.DeleteSupplierAsync(id);
            if (!result)
            {
                return NotFound($"Supplier with ID {id} not found.");
            }

            return NoContent();
        }

        /// <summary>
        /// Search suppliers by name
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Supplier>>> SearchSuppliers([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search name is required.");
            }

            var suppliers = await _supplierService.SearchSuppliersByNameAsync(name);
            return Ok(suppliers);
        }

        /// <summary>
        /// Get suppliers by email
        /// </summary>
        [HttpGet("by-email")]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliersByEmail([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email is required.");
            }

            if (!IsValidEmail(email))
            {
                return BadRequest("Invalid email format.");
            }

            var suppliers = await _supplierService.GetSuppliersByEmailAsync(email);
            return Ok(suppliers);
        }

        /// <summary>
        /// Basic email validation helper method
        /// </summary>
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
