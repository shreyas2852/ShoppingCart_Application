using ShoppingCart_6.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCart_6.ViewModel
{
    public class OrderViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ShippingAddress { get; set; }
        public int PaymentMethodType { get; set; }
        public string Status { get; set; }

        public int RegisterId { get; set; }

        public int ProductId { get; set; }
    }
}
