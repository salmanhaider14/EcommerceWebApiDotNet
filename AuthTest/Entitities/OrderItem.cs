using System.ComponentModel.DataAnnotations;
namespace DotNetEcommerceAPI.Entitities;

public class OrderItem
{
    public Guid Id { get; set; }

    public Guid? OrderId { get; set; }      
    public Order? Order { get; set; } 
    public required Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public required int Quantity { get; set; }
    [Range(0, double.MaxValue)]
    public required decimal UnitPrice { get; set; }
}
