using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetEcommerceAPI.Entitities;
public class Product
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    public string Name { get; set; }
    public string Description { get; set; }

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    public string? imageUrl { get; set; }
    [ForeignKey("CategoryId")]
    public int? CategoryId { get; set; }
    public virtual Category? Category { get; set; }
}
