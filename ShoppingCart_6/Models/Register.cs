
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart_6.Models
{
    public class Register 
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

    }
    public enum UserType
    {
        ShopKeeper,
        Customer
    }
}
