using ShoppingCart.Models;
using ShoppingCart.Models.Enums;
using ShoppingCart.Models.User;
using Xunit;

namespace ShoppingCart.Tests.Models
{
    public class CartTests
    {
        [Fact]
        public void Cart_應該能夠新增商品()
        {
            // Arrange
            var cart = new Cart();
            var product = new Product("ipad", ProductType.電子)
            {
                price = 2399.00f,
                quantity = 1
            };

            // Act
            cart.products.Add(product);

            // Assert
            Assert.Single(cart.products);
            Assert.Equal("ipad", cart.products[0].name);
            Assert.Equal(2399.00f, cart.products[0].price);
        }

        [Fact]
        public void Cart_應該能夠計算商品總額()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.電子) { price = 2399.00f, quantity = 1 });
            cart.products.Add(new Product("螢幕", ProductType.電子) { price = 1799.00f, quantity = 1 });
            cart.products.Add(new Product("啤酒", ProductType.酒類) { price = 25.00f, quantity = 12 });

            // Act
            var total = cart.products.Sum(p => p.price * p.quantity);

            // Assert
            Assert.Equal(4498.00f, total, 2); // 2399 + 1799 + 300 = 4498
        }

        [Fact]
        public void Cart_應該能夠加入折價券()
        {
            // Arrange
            var cart = new Cart();
            var coupon = new Coupon(DateTime.Parse("2016-03-02"), 1000, 200);

            // Act
            cart.coupon = coupon;

            // Assert
            Assert.NotNull(cart.coupon);
            Assert.Equal(1000, cart.coupon.spendAmount);
            Assert.Equal(200, cart.coupon.discountAmount);
        }

        [Fact]
        public void Cart_空購物車應該回傳零總額()
        {
            // Arrange
            var cart = new Cart();

            // Act
            var total = cart.products.Sum(p => p.price * p.quantity);

            // Assert
            Assert.Equal(0f, total);
        }
    }
}
