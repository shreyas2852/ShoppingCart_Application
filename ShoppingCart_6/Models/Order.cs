using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShoppingCart_6.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        [DisplayName("Shipping Address")]
        public string ShippingAddress { get; set; }
       
        [DisplayName("Payment Type")]
        public int PaymentMethodType { get; set; }
        public string Status { get; set; }

        public int ProductId { get; set; }

        [DisplayName("Product Name")]
        public virtual Product Product { get; set; }

        public int RegisterId { get; set; }

        [DisplayName("User Name")]
        public virtual Register Register { get;  set; }
    }

    public enum PaymentMethodType
    {
        Online,
        Offline
    }
}
