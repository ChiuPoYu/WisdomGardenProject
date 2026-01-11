using ShoppingCart.Models;
using ShoppingCart.Models.Enums;
using ShoppingCart.Models.User;
using Xunit;

namespace ShoppingCart.Tests.Models
{
    public class PromotionTests
    {
        [Fact]
        public void Promotion_ShouldBeAbleToInitializeCorrectly()
        {
            // Arrange & Act
            var promotion = new Promotion
            {
                expiredAt = DateTime.Parse("2016-03-02"),
                discountPercent = 0.1f,
                productType = ProductType.Electric
            };

            // Assert
            Assert.Equal(DateTime.Parse("2016-03-02"), promotion.expiredAt);
            Assert.Equal(0.1f, promotion.discountPercent);
            Assert.Equal(ProductType.Electric, promotion.productType);
        }

        [Theory]
        [InlineData("2016-03-02", "2015-11-11", true)]  // 有效：未過期
        [InlineData("2016-03-02", "2016-03-03", false)] // 無效：已過期
        [InlineData("2015-01-01", "2015-11-11", false)] // 無效：已過期
        public void Promotion_ShouldBeAbleToCheckIfValid(string expiredDateStr, string orderDateStr, bool expectedValid)
        {
            // Arrange
            var promotion = new Promotion
            {
                expiredAt = DateTime.Parse(expiredDateStr),
                discountPercent = 0.1f,
                productType = ProductType.Electric
            };
            var orderDate = DateTime.Parse(orderDateStr);

            // Act
            bool isValid = promotion.expiredAt > orderDate;

            // Assert
            Assert.Equal(expectedValid, isValid);
        }

        [Theory]
        [InlineData(ProductType.Electric, 2000.00f, 0.1f, 200.00f)]
        [InlineData(ProductType.Electric, 1000.00f, 0.2f, 200.00f)]
        [InlineData(ProductType.Food, 500.00f, 0.15f, 75.00f)]
        [InlineData(ProductType.Alcohol, 300.00f, 0.5f, 150.00f)]
        public void Promotion_ShouldBeAbleToCalculateDiscountAmountCorrectly(ProductType type, float amount, float percent, float expectedDiscount)
        {
            // Arrange
            var promotion = new Promotion
            {
                expiredAt = DateTime.Parse("2016-03-02"),
                discountPercent = percent,
                productType = type
            };

            // Act
            var discount = amount * promotion.discountPercent;

            // Assert
            Assert.Equal(expectedDiscount, discount, 2);
        }

        [Fact]
        public void Promotion_ShouldOnlyApplyToSpecifiedProductType()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.Electric) { price = 2000.00f, quantity = 1 });
            cart.products.Add(new Product("麵包", ProductType.Food) { price = 100.00f, quantity = 1 });
            cart.products.Add(new Product("啤酒", ProductType.Alcohol) { price = 50.00f, quantity = 1 });

            var promotion = new Promotion
            {
                expiredAt = DateTime.Parse("2016-03-02"),
                discountPercent = 0.1f,
                productType = ProductType.Electric
            };

            // Act
            var eligibleAmount = cart.products
                .Where(p => p.productType == promotion.productType)
                .Sum(p => p.price * p.quantity);
            var discount = eligibleAmount * promotion.discountPercent;

            // Assert
            Assert.Equal(2000.00f, eligibleAmount, 2); // 只有Electric商品
            Assert.Equal(200.00f, discount, 2);
        }

        [Fact]
        public void Promotion_MultipleSameTypeProducts_ShouldApplyPromotionCorrectly()
        {
            // Arrange
            var cart = new Cart();
            cart.products.Add(new Product("ipad", ProductType.Electric) { price = 2000.00f, quantity = 1 });
            cart.products.Add(new Product("iphone", ProductType.Electric) { price = 1000.00f, quantity = 2 });
            cart.products.Add(new Product("螢幕", ProductType.Electric) { price = 1500.00f, quantity = 1 });

            var promotion = new Promotion
            {
                expiredAt = DateTime.Parse("2016-03-02"),
                discountPercent = 0.1f,
                productType = ProductType.Electric
            };

            // Act
            var eligibleAmount = cart.products
                .Where(p => p.productType == promotion.productType)
                .Sum(p => p.price * p.quantity);
            var discount = eligibleAmount * promotion.discountPercent;

            // Assert
            Assert.Equal(5500.00f, eligibleAmount, 2); // 2000 + 2000 + 1500
            Assert.Equal(550.00f, discount, 2);
        }
    }
}
