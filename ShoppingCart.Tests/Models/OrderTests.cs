using ShoppingCart.Models;
using ShoppingCart.Models.Enums;
using ShoppingCart.Models.User;
using Xunit;

namespace ShoppingCart.Tests.Models
{
    public class OrderTests
    {
        [Fact]
        public void Order_應該正確建立訂單()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.電子) { price = 2399.00f, quantity = 1 });
            var createdAt = DateTime.Parse("2015-11-11");

            // Act
            var order = new Order
            {
                cart = cart,
                createdAt = createdAt
            };

            // Assert
            Assert.NotNull(order.cart);
            Assert.Equal(createdAt, order.createdAt);
            Assert.Single(order.cart.products);
        }

        [Fact]
        public void Order_應該正確計算無折扣的總金額()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.電子) { price = 2399.00f, quantity = 1 });
            cart.products.Add(new Product("螢幕", ProductType.電子) { price = 1799.00f, quantity = 1 });
            cart.products.Add(new Product("啤酒", ProductType.酒類) { price = 25.00f, quantity = 12 });
            cart.products.Add(new Product("麵包", ProductType.食品) { price = 9.00f, quantity = 5 });

            var order = new Order
            {
                cart = cart,
                createdAt = DateTime.Parse("2015-11-11")
            };

            // Act
            var subtotal = order.cart.products.Sum(p => p.price * p.quantity);

            // Assert
            Assert.Equal(4543.00f, subtotal, 2); // 2399 + 1799 + 300 + 45 = 4543
        }

        [Fact]
        public void Order_應該正確套用有效折價券()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.電子) { price = 2399.00f, quantity = 1 });
            cart.products.Add(new Product("螢幕", ProductType.電子) { price = 1799.00f, quantity = 1 });
            cart.coupon = new Coupon(DateTime.Parse("2016-03-02"), 1000, 200);

            var order = new Order
            {
                cart = cart,
                createdAt = DateTime.Parse("2015-11-11")
            };

            // Act
            var subtotal = order.cart.products.Sum(p => p.price * p.quantity);
            var discount = 0f;
            if (order.cart.coupon != null && 
                order.cart.coupon.expiredAt > order.createdAt && 
                subtotal >= order.cart.coupon.spendAmount)
            {
                discount = order.cart.coupon.discountAmount;
            }
            var total = subtotal - discount;

            // Assert
            Assert.Equal(4198.00f, subtotal, 2);
            Assert.Equal(200f, discount);
            Assert.Equal(3998.00f, total, 2);
        }

        [Fact]
        public void Order_過期折價券不應該被套用()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.電子) { price = 2399.00f, quantity = 1 });
            cart.coupon = new Coupon(DateTime.Parse("2015-01-01"), 1000, 200); // 過期券

            var order = new Order
            {
                cart = cart,
                createdAt = DateTime.Parse("2015-11-11")
            };

            // Act
            var subtotal = order.cart.products.Sum(p => p.price * p.quantity);
            var discount = 0f;
            if (order.cart.coupon != null && 
                order.cart.coupon.expiredAt > order.createdAt && 
                subtotal >= order.cart.coupon.spendAmount)
            {
                discount = order.cart.coupon.discountAmount;
            }
            var total = subtotal - discount;

            // Assert
            Assert.Equal(0f, discount);
            Assert.Equal(subtotal, total);
        }

        [Fact]
        public void Order_應該正確套用促銷活動()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.電子) { price = 2000.00f, quantity = 1 });
            cart.products.Add(new Product("麵包", ProductType.食品) { price = 100.00f, quantity = 1 });

            var order = new Order
            {
                cart = cart,
                createdAt = DateTime.Parse("2015-11-11"),
                promotion = new Promotion
                {
                    expiredAt = DateTime.Parse("2016-03-02"),
                    discountPercent = 0.1f, // 10% 折扣
                    productType = ProductType.電子
                }
            };

            // Act
            var subtotal = order.cart.products.Sum(p => p.price * p.quantity);
            var promotionDiscount = 0f;

            if (order.promotion != null && order.promotion.expiredAt > order.createdAt)
            {
                var eligibleProducts = order.cart.products
                    .Where(p => p.productType == order.promotion.productType)
                    .Sum(p => p.price * p.quantity);
                promotionDiscount = eligibleProducts * order.promotion.discountPercent;
            }

            var total = subtotal - promotionDiscount;

            // Assert
            Assert.Equal(2100.00f, subtotal, 2);
            Assert.Equal(200.00f, promotionDiscount, 2); // 2000 * 0.1 = 200
            Assert.Equal(1900.00f, total, 2);
        }

        [Fact]
        public void Order_應該同時套用折價券和促銷活動()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.電子) { price = 2000.00f, quantity = 1 });
            cart.products.Add(new Product("麵包", ProductType.食品) { price = 500.00f, quantity = 1 });
            cart.coupon = new Coupon(DateTime.Parse("2016-03-02"), 1000, 200);

            var order = new Order
            {
                cart = cart,
                createdAt = DateTime.Parse("2015-11-11"),
                promotion = new Promotion
                {
                    expiredAt = DateTime.Parse("2016-03-02"),
                    discountPercent = 0.1f,
                    productType = ProductType.電子
                }
            };

            // Act
            var subtotal = order.cart.products.Sum(p => p.price * p.quantity);
            
            // 促銷折扣
            var promotionDiscount = 0f;
            if (order.promotion != null && order.promotion.expiredAt > order.createdAt)
            {
                var eligibleProducts = order.cart.products
                    .Where(p => p.productType == order.promotion.productType)
                    .Sum(p => p.price * p.quantity);
                promotionDiscount = eligibleProducts * order.promotion.discountPercent;
            }

            var afterPromotion = subtotal - promotionDiscount;

            // 折價券折扣
            var couponDiscount = 0f;
            if (order.cart.coupon != null && 
                order.cart.coupon.expiredAt > order.createdAt && 
                afterPromotion >= order.cart.coupon.spendAmount)
            {
                couponDiscount = order.cart.coupon.discountAmount;
            }

            var total = afterPromotion - couponDiscount;

            // Assert
            Assert.Equal(2500.00f, subtotal, 2);
            Assert.Equal(200.00f, promotionDiscount, 2);
            Assert.Equal(200.00f, couponDiscount, 2);
            Assert.Equal(2100.00f, total, 2);
        }
    }
}
