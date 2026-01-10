namespace ShoppingCart.Models.User
{
    public class Cart
    {
        public Coupon? coupon { get; set; }

        public List<Product> products { get; set; } = new List<Product>();

    }
}
