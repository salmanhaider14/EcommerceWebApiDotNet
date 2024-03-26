using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    [Key]
    public int OrderId { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }

    [Required]

    public OrderStatus OrderStatus { get; set; }

    public IdentityUser? User { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}
