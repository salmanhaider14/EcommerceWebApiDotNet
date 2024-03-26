
using DotNetEcommerceAPI.Entitities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNetEcommerceAPI.Controllers;

[Authorize(Roles ="Admin")]
[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly AppContext _context;

    public ProductsController(AppContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string categoryName = null, string searchQuery = null, decimal? minPrice = null,
        decimal? maxPrice = null,
        string sortBy = null,
        int? pageNumber = null,
        int? pageSize = null)
    {
        IQueryable<Product> productsQuery = _context.Products.Include(p => p.Category);

        if (!string.IsNullOrWhiteSpace(categoryName))
            productsQuery = productsQuery.Where(p => p.Category.Name.ToLower() == categoryName.ToLower());

        if (!string.IsNullOrEmpty(searchQuery))
        {
            productsQuery = productsQuery.Where(p =>
                p.Name.ToLower().Contains(searchQuery.ToLower()) ||
                p.Description.ToLower().Contains(searchQuery.ToLower())
            );
        }

        if (minPrice.HasValue)
            productsQuery = productsQuery.Where(p => p.Price >= minPrice);

        if (maxPrice.HasValue)
            productsQuery = productsQuery.Where(p => p.Price <= maxPrice);

        if (!string.IsNullOrWhiteSpace(sortBy))
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
            productsQuery = productsQuery.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);

        var products = await productsQuery.ToListAsync();
        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
        };

        var jsonResult = JsonSerializer.Serialize(products, jsonOptions);
        return Content(jsonResult, "application/json");

    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(Guid id)
    {
        var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(c => c.Id == id);

        if (product == null)
        {
            return NotFound();
        }
        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
        };

        var jsonResult = JsonSerializer.Serialize(product, jsonOptions);
        return Content(jsonResult, "application/json");

    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(Guid id, Product product)
    {
        if (id != product.Id)
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

    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
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
    private bool ProductExists(Guid id)
    {
        return _context.Products.Any(e => e.Id == id);
    }
}
