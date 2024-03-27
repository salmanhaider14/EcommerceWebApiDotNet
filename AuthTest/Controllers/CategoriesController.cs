
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNetEcommerceAPI.Entitities;
using Microsoft.AspNetCore.Authorization;
using DotNetEcommerceAPI.Data;

namespace DotNetEcommerceAPI.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class CategoriesController(AppContext context) : ControllerBase
{
    private readonly AppContext _context = context;

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
    {
        var categories = await _context.Categories.ToListAsync();
        return categories.Select(c => new CategoryDTO(c.Id ,c.Name)).ToList();
    }
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDTO>> GetCategory(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return new CategoryDTO(category.Id, category.Name);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutCategory(Guid id, UpdateCategoryDTO categoryDTO)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        category.Name = categoryDTO.Name;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CategoryExists(id))
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
    public async Task<ActionResult<CategoryDTO>> PostCategory(CreateCategoryDTO categoryDTO)
    {
        var category = new Category
        {
            Name = categoryDTO.Name,
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var createdCategoryDto = new CategoryDTO(category.Id, category.Name);
        return CreatedAtAction("GetCategory", new { id = category.Id }, createdCategoryDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool CategoryExists(Guid id)
    {
        return _context.Categories.Any(e => e.Id == id);
    }
}
