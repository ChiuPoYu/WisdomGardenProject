using ShoppingCart.Models;
using ShoppingCart.Models.Enums;
using Xunit;

namespace ShoppingCart.Tests.Models
{
    public class ProductTests
    {
        [Fact]
        public void Product_應該正確初始化()
        {
            // Arrange & Act
            var product = new Product("ipad", ProductType.電子)
            {
                price = 2399.00f,
                quantity = 1
            };

            // Assert
            Assert.Equal("ipad", product.name);
            Assert.Equal(ProductType.電子, product.productType);
            Assert.Equal(2399.00f, product.price);
            Assert.Equal(1, product.quantity);
        }

        [Theory]
        [InlineData("ipad", 2399.00f, 1, 2399.00f)]
        [InlineData("啤酒", 25.00f, 12, 300.00f)]
        [InlineData("麵包", 9.00f, 5, 45.00f)]
        [InlineData("螢幕", 1799.00f, 2, 3598.00f)]
        public void Product_應該正確計算小計(string name, float price, int quantity, float expectedTotal)
        {
            // Arrange
            var product = new Product(name, ProductType.電子)
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
        public void Product_應該正確設定產品類型()
        {
            // Arrange & Act
            var electronicProduct = new Product("ipad", ProductType.電子);
            var foodProduct = new Product("麵包", ProductType.食品);
            var householdProduct = new Product("雨傘", ProductType.日用品);
            var alcoholProduct = new Product("啤酒", ProductType.酒類);

            // Assert
            Assert.Equal(ProductType.電子, electronicProduct.productType);
            Assert.Equal(ProductType.食品, foodProduct.productType);
            Assert.Equal(ProductType.日用品, householdProduct.productType);
            Assert.Equal(ProductType.酒類, alcoholProduct.productType);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(100)]
        public void Product_應該允許設定任意數量(int quantity)
        {
            // Arrange
            var product = new Product("ipad", ProductType.電子)
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
        public void Product_應該允許設定任意價格(float price)
        {
            // Arrange
            var product = new Product("測試商品", ProductType.電子)
            {
                price = price
            };

            // Act & Assert
            Assert.Equal(price, product.price);
        }
    }
}
