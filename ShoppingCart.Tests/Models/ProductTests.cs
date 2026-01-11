using ShoppingCart.Models;
using ShoppingCart.Models.Enums;
using Xunit;

namespace ShoppingCart.Tests.Models
{
    public class ProductTests
    {
        [Fact]
        public void Product_ShouldBeAbleToInitializeCorrectly()
        {
            // Arrange & Act
            var product = new Product("ipad", ProductType.Electric)
            {
                price = 2399.00f,
                quantity = 1
            };

            // Assert
            Assert.Equal("ipad", product.name);
            Assert.Equal(ProductType.Electric, product.productType);
            Assert.Equal(2399.00f, product.price);
            Assert.Equal(1, product.quantity);
        }

        [Theory]
        [InlineData("ipad", 2399.00f, 1, 2399.00f)]
        [InlineData("啤酒", 25.00f, 12, 300.00f)]
        [InlineData("麵包", 9.00f, 5, 45.00f)]
        [InlineData("螢幕", 1799.00f, 2, 3598.00f)]
        public void Product_ShouldBeAbleToCalculateTotalCorrectly(string name, float price, int quantity, float expectedTotal)
        {
            // Arrange
            var product = new Product(name, ProductType.Electric)
            {
                price = price,
                quantity = quantity
            };

            // Act
            var total = product.price * product.quantity;

            // Assert
            Assert.Equal(expectedTotal, total, 2);
        }

        [Fact]
        public void Product_ShouldBeAbleToSetProductType()
        {
            // Arrange & Act
            var electronicProduct = new Product("ipad", ProductType.Electric);
            var foodProduct = new Product("麵包", ProductType.Food);
            var householdProduct = new Product("肥皂", ProductType.LifeUsed);
            var alcoholProduct = new Product("啤酒", ProductType.Alcohol);

            // Assert
            Assert.Equal(ProductType.Electric, electronicProduct.productType);
            Assert.Equal(ProductType.Food, foodProduct.productType);
            Assert.Equal(ProductType.LifeUsed, householdProduct.productType);
            Assert.Equal(ProductType.Alcohol, alcoholProduct.productType);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void Product_ShouldBeAbleToSetDifferentQuantity(int quantity)
        {
            // Arrange
            var product = new Product("ipad", ProductType.Electric)
            {
                quantity = quantity
            };

            // Act & Assert
            Assert.Equal(quantity, product.quantity);
        }

        [Theory]
        [InlineData(0.00f)]
        [InlineData(9.99f)]
        [InlineData(2399.00f)]
        [InlineData(99999.99f)]
        public void Product_ShouldBeAbleToSetDifferentPrice(float price)
        {
            // Arrange
            var product = new Product("測試商品", ProductType.Electric)
            {
                price = price
            };

            // Act & Assert
            Assert.Equal(price, product.price);
        }
    }
}
