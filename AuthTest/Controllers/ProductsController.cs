
using DotNetEcommerceAPI.Entitities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNetEcommerceAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppContext _context;

        public ProductsController(AppContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string categoryName = null, string searchQuery = null, decimal? minPrice = null,
            decimal? maxPrice = null,
            string sortBy = null,
            int? pageNumber = null,
            int? pageSize = null)
        {
            IQueryable<Product> productsQuery = _context.Products.Include(p => p.Category);

            // Filter by category name if provided
            if (!string.IsNullOrEmpty(categoryName))
            {
                productsQuery = productsQuery.Where(p => p.Category.Name.ToLower() == categoryName.ToLower());
            }

            // Search products by name or description if search query is provided
            if (!string.IsNullOrEmpty(searchQuery))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Name.ToLower().Contains(searchQuery.ToLower()) ||
                    p.Description.ToLower().Contains(searchQuery.ToLower())
                );
            }
            // Filter by price range if provided
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice);
            }
            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice);
            }
            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLower())
                {
                    case "price":
                        productsQuery = productsQuery.OrderBy(p => p.Price);
                        break;
                    case "popularity":
                        // Implement popularity-based sorting logic if applicable
                        break;
                    default:
                        // Handle invalid sorting criteria
                        break;
                }
            }
            if (pageNumber.HasValue && pageSize.HasValue)
            {
                productsQuery = productsQuery.Skip((pageNumber.Value - 1) * pageSize.Value)
                                             .Take(pageSize.Value);
            }

            // Execute the query and return the results
            var products = await productsQuery.ToListAsync();
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true // Optionally, you can enable indentation for better readability
            };

            // Serialize the products using JsonSerializerOptions
            var jsonResult = JsonSerializer.Serialize(products, jsonOptions);

            // Return the serialized JSON result
            return Content(jsonResult, "application/json");

        }


        // GET: api/Products/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(c => c.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true // Optionally, you can enable indentation for better readability
            };

            // Serialize the products using JsonSerializerOptions
            var jsonResult = JsonSerializer.Serialize(product, jsonOptions);

            // Return the serialized JSON result
            return Content(jsonResult, "application/json");

        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.ProductId }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
