using ShoppingCart_6.Models;

namespace ShoppingCart_6.Services
{
    public class CartServices : ICartService
    {
        private readonly List<Product> _cartItems = new List<Product>(); // Assuming Product is your product model
       // private readonly List<CartItem> _cartItems = new List<CartItem>();

        public void AddToCart(Product product)
        {
            _cartItems.Add(product);
        }

        public IEnumerable<Product> GetCartItems()
        {
            return _cartItems;
        }
        public void RemoveFromCart(int productId)
        {
            var itemToRemove = _cartItems.FirstOrDefault(item => item.Id == productId);

            if (itemToRemove != null)
            {
                _cartItems.Remove(itemToRemove);
            }
        }

    }

   
}
