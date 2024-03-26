using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetEcommerceAPI.Entitities;
public class Product
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    [Range(0, double.MaxValue)]
    public required decimal Price { get; set; }
    public string? imageUrl { get; set; }

    [ForeignKey("CategoryId")]
    public Guid? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
}
