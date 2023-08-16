using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ShoppingCart_6.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "numeric(18,2)")] // Adjust precision and scale as needed
        public decimal Price { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [MaxLength(200)]
        [DisplayName("Image")]
        public string ImageUrl { get; set; }

        [NotMapped]
        [DisplayName("Product Image")]
        public IFormFile ImageFile { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
