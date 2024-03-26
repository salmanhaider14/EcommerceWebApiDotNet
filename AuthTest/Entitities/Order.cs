using Microsoft.AspNetCore.Identity;
namespace DotNetEcommerceAPI.Entitities;
public enum OrderStatus
{
    Pending,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}
public class Order
{
    public Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required DateTime OrderDate { get; set; }
    public required OrderStatus OrderStatus { get; set; }
    public IdentityUser? User { get; set; }
    public required List<OrderItem> OrderItems { get; set; }
}
