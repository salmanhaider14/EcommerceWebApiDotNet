
using DotNetEcommerceAPI.Entitities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetEcommerceAPI.Controllers;

[Authorize(Roles = "Customer,Admin")]
[Route("api/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly AppContext _context;

    public OrdersController(AppContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders(Guid? userId,
         OrderStatus? status = null,

        int? pageNumber = null,
        int? pageSize = null,
        DateTime? startDate = null,
        DateTime? endDate = null
        )
    {
        IQueryable<Order> query = _context.Orders.Include(o => o.User).Include(o => o.OrderItems);

        if (userId != null)
            query = query.Where(o => o.UserId == userId);

        if (status != null)
            query = query.Where(o => o.OrderStatus == status);

        if (startDate != null)
            query = query.Where(o => o.OrderDate >= startDate.Value.Date);

        if (endDate != null)
            query = query.Where(o => o.OrderDate <= endDate.Value.Date);

        if (pageNumber.HasValue && pageSize.HasValue)
            query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);

        var orders = await query.ToListAsync();

        if (orders == null || orders.Count == 0)
        {
            return NotFound();
        }

        return orders;

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            return NotFound();
        }

        return order;
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutOrder(Guid id, Order order)
    {
        if (id != order.Id)
        {
            return BadRequest();
        }

        _context.Entry(order).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!OrderExists(id))
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
    public async Task<ActionResult<Order>> PostOrder(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetOrder", new { id = order.Id }, order);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOrder(Guid id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool OrderExists(Guid id)
    {
        return _context.Orders.Any(e => e.Id == id);
    }
}
