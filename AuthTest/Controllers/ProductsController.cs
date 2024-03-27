
using DotNetEcommerceAPI.Data;
using DotNetEcommerceAPI.Entitities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetEcommerceAPI.Controllers;

[Authorize(Roles ="Admin")]
[Route("api/[controller]")]
[ApiController]
public class ProductsController(AppContext context) : ControllerBase
{
    private readonly AppContext _context = context;

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProducts(string categoryName = null, string searchQuery = null, decimal? minPrice = null,
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
        var productDTOs = products.Select(p => new ProductDTO(p.Id, p.Name, p.Description, p.Price, p.imageUrl, p.CategoryId)).ToList();

        return productDTOs;
        
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> GetProduct(Guid id)
    {
        var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(c => c.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        var productDTO = new ProductDTO(product.Id, product.Name, product.Description, product.Price, product.imageUrl, product.CategoryId);
        return productDTO;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(Guid id, UpdateProductDTO productDTO)
    {
        
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

       
        product.Name = productDTO.Name;
        product.Description = productDTO.Description;
        product.Price = productDTO.Price;
        product.imageUrl = productDTO.ImageUrl;
        product.CategoryId = productDTO.CategoryId;

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
    public async Task<ActionResult<ProductDTO>> PostProduct(CreateProductDTO productDTO)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = productDTO.Name,
            Description = productDTO.Description,
            Price = productDTO.Price,
            imageUrl = productDTO.ImageUrl,
            CategoryId = productDTO.CategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var createdProductDTO = new ProductDTO(product.Id, product.Name, product.Description, product.Price, product.imageUrl, product.CategoryId);
        return CreatedAtAction("GetProduct", new { id = product.Id }, createdProductDTO);
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
