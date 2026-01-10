using ShoppingCart.Models;
using ShoppingCart.Models.Enums;
using ShoppingCart.Models.User;
using Xunit;

namespace ShoppingCart.Tests.Integration
{
    /// <summary>
    /// 整合測試：測試完整的購物流程
    /// </summary>
    public class ShoppingFlowTests
    {
        [Fact]
        public void 完整購物流程_無折扣_應該正確計算總金額()
        {
            // Arrange - 建立購物車並加入商品
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.電子) { price = 2399.00f, quantity = 1 });
            cart.products.Add(new Product("螢幕", ProductType.電子) { price = 1799.00f, quantity = 1 });
            cart.products.Add(new Product("啤酒", ProductType.酒類) { price = 25.00f, quantity = 12 });
            cart.products.Add(new Product("麵包", ProductType.食品) { price = 9.00f, quantity = 5 });

            // Act - 建立訂單並計算
            var order = new Order
            {
                cart = cart,
                createdAt = DateTime.Parse("2015-11-11")
            };

            var total = order.cart.products.Sum(p => p.price * p.quantity);

            // Assert
            Assert.Equal(4543.00f, total, 2); // 2399 + 1799 + 300 + 45 = 4543
        }

        [Fact]
        public void 完整購物流程_有效折價券_應該正確折扣()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.電子) { price = 2399.00f, quantity = 1 });
            cart.products.Add(new Product("螢幕", ProductType.電子) { price = 1799.00f, quantity = 1 });
            cart.products.Add(new Product("啤酒", ProductType.酒類) { price = 25.00f, quantity = 12 });
            cart.products.Add(new Product("麵包", ProductType.食品) { price = 9.00f, quantity = 5 });
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
            Assert.Equal(4543.00f, subtotal, 2);
            Assert.Equal(200f, discount);
            Assert.Equal(4343.00f, total, 2);
        }

        [Fact]
        public void 完整購物流程_過期折價券_不應該折扣()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.電子) { price = 2399.00f, quantity = 1 });
            cart.products.Add(new Product("螢幕", ProductType.電子) { price = 1799.00f, quantity = 1 });
            cart.coupon = new Coupon(DateTime.Parse("2014-03-02"), 1000, 200); // 過期券

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
            Assert.Equal(0f, discount);
            Assert.Equal(4198.00f, total, 2);
        }

        [Fact]
        public void 完整購物流程_有促銷活動_應該正確計算()
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
                createdAt = DateTime.Parse("2015-11-11"),
                promotion = new Promotion
                {
                    expiredAt = DateTime.Parse("2016-03-02"),
                    discountPercent = 0.1f, // 電子產品打9折
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
            Assert.Equal(4543.00f, subtotal, 2);
            Assert.Equal(419.80f, promotionDiscount, 2); // (2399 + 1799) * 0.1 = 419.8
            Assert.Equal(4123.20f, total, 2);
        }

        [Fact]
        public void 完整購物流程_同時有折價券和促銷_應該正確計算()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.電子) { price = 2399.00f, quantity = 1 });
            cart.products.Add(new Product("螢幕", ProductType.電子) { price = 1799.00f, quantity = 1 });
            cart.products.Add(new Product("啤酒", ProductType.酒類) { price = 25.00f, quantity = 12 });
            cart.products.Add(new Product("麵包", ProductType.食品) { price = 9.00f, quantity = 5 });
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
            
            // 先套用促銷折扣
            var promotionDiscount = 0f;
            if (order.promotion != null && order.promotion.expiredAt > order.createdAt)
            {
                var eligibleProducts = order.cart.products
                    .Where(p => p.productType == order.promotion.productType)
                    .Sum(p => p.price * p.quantity);
                promotionDiscount = eligibleProducts * order.promotion.discountPercent;
            }

            var afterPromotion = subtotal - promotionDiscount;

            // 再套用折價券
            var couponDiscount = 0f;
            if (order.cart.coupon != null && 
                order.cart.coupon.expiredAt > order.createdAt && 
                afterPromotion >= order.cart.coupon.spendAmount)
            {
                couponDiscount = order.cart.coupon.discountAmount;
            }

            var total = afterPromotion - couponDiscount;

            // Assert
            Assert.Equal(4543.00f, subtotal, 2);
            Assert.Equal(419.80f, promotionDiscount, 2);
            Assert.Equal(200.00f, couponDiscount, 2);
            Assert.Equal(3923.20f, total, 2);
        }

        [Fact]
        public void 完整購物流程_未達折價券門檻_不應該折扣()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("麵包", ProductType.食品) { price = 9.00f, quantity = 5 });
            cart.coupon = new Coupon(DateTime.Parse("2016-03-02"), 1000, 200); // 門檻1000

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
            Assert.Equal(45.00f, subtotal, 2);
            Assert.Equal(0f, discount);
            Assert.Equal(45.00f, total, 2);
        }

        [Fact]
        public void 完整購物流程_空購物車_應該回傳零()
        {
            // Arrange
            var cart = new Cart();
            var order = new Order
            {
                cart = cart,
                createdAt = DateTime.Parse("2015-11-11")
            };

            // Act
            var total = order.cart.products.Sum(p => p.price * p.quantity);

            // Assert
            Assert.Equal(0f, total);
        }
    }
}
