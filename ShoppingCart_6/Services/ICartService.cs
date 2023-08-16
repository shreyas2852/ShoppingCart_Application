using ShoppingCart_6.Models;

namespace ShoppingCart_6.Services
{
    public interface ICartService
    {
        void AddToCart(Product product);
        IEnumerable<Product> GetCartItems();
        void RemoveFromCart(int productId);
    }
}
