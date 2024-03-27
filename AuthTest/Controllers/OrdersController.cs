
using DotNetEcommerceAPI.Data;
using DotNetEcommerceAPI.Entitities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetEcommerceAPI.Controllers;

[Authorize(Roles = "Admin,Customer")]
[Route("api/[controller]")]
[ApiController]
public class OrdersController(AppContext context) : ControllerBase
{
    private readonly AppContext _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders(Guid? userId,
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

        if (orders == null)
        {
            return NotFound();
        }


        var orderDTOs = orders.Select(o => new OrderDTO(o.Id, o.UserId, o.OrderDate, o.OrderStatus,
            o.OrderItems.Select(oi => new OrderItem {
                Id = oi.Id,
                ProductId = oi.ProductId,
                OrderId = oi.OrderId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()
            )
        ).ToList();

        return orderDTOs;

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDTO>> GetOrder(Guid id)
    {
        var order = await _context.Orders.Include(o=>o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }
        
        return new OrderDTO(order.Id, order.UserId, order.OrderDate, order.OrderStatus,
            order.OrderItems.Select(oi => new OrderItem
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                OrderId = oi.OrderId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList());




    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutOrder(Guid id, UpdateOrderDTO orderDTO)
    {

        var order = await _context.Orders.FindAsync(id);
        if (order == null) return NotFound();

        order.OrderStatus = orderDTO.OrderStatus;

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
    public async Task<ActionResult<OrderDTO>> PostOrder(CreateOrderDTO orderDTO)
    {
        var order = new Order {
            UserId = orderDTO.UserId,
            OrderStatus = orderDTO.OrderStatus,
            OrderDate = orderDTO.OrderDate,
            OrderItems = orderDTO.OrderItems.Select(oi => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList()

        };
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        var createdOrderDTO = new OrderDTO(order.Id, order.UserId, order.OrderDate, order.OrderStatus,
            order.OrderItems.Select(oi => new OrderItem
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                OrderId = oi.OrderId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList());

        return CreatedAtAction("GetOrder", new { id = order.Id }, createdOrderDTO);
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
