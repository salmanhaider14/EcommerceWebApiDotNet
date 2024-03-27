using DotNetEcommerceAPI.Entitities;
using System.ComponentModel.DataAnnotations;

namespace DotNetEcommerceAPI.Data;
public record ProductDTO(Guid Id, string Name, string Description, decimal Price, string? ImageUrl, Guid? CategoryId);

public record CreateProductDTO([Required] string Name, [Required] string Description, [Range(0, double.MaxValue)] decimal Price, string? ImageUrl, Guid? CategoryId);

public record UpdateProductDTO([Required] string Name, [Required] string Description, [Range(0, double.MaxValue)] decimal Price, string? ImageUrl, Guid? CategoryId);

public record CategoryDTO(Guid Id, string Name);

public record CreateCategoryDTO([Required] string Name);

public record UpdateCategoryDTO([Required] string Name);

public record OrderDTO(Guid Id, Guid UserId, DateTime OrderDate, OrderStatus OrderStatus, List<OrderItem> OrderItems);

public record CreateOrderDTO([Required] Guid UserId, [Required] DateTime OrderDate, [Required] OrderStatus OrderStatus, List<OrderItem> OrderItems);
public record UpdateOrderDTO([Required] OrderStatus OrderStatus);

/*public record OrderItemDTO(Guid Id, Guid OrderId, Guid ProductId, int Quantity, decimal UnitPrice);

public record CreateOrderItemDTO([Required] Guid OrderId, [Required] Guid ProductId, [Required] int Quantity, [Range(0, double.MaxValue)] decimal UnitPrice);

public record UpdateOrderItemDTO([Required] int Quantity, [Range(0, double.MaxValue)] decimal UnitPrice);*/


