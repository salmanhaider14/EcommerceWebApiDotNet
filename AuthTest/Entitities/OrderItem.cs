using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthTest.Entitities
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
       
        public int OrderId { get; set; }
       
        public Order? Order { get; set; } 

        [Required]
      
        public int ProductId { get; set; }

   
        public Product? Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }
}
