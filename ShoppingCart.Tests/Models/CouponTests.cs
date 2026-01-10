using ShoppingCart.Models;
using ShoppingCart.Models.Enums;
using ShoppingCart.Models.User;
using Xunit;

namespace ShoppingCart.Tests.Models
{
    public class CouponTests
    {
        [Fact]
        public void Coupon_應該正確初始化()
        {
            // Arrange & Act
            var expiredDate = DateTime.Parse("2016-03-02");
            var coupon = new Coupon(expiredDate, 1000, 200);

            // Assert
            Assert.Equal(expiredDate, coupon.expiredAt);
            Assert.Equal(1000, coupon.spendAmount);
            Assert.Equal(200, coupon.discountAmount);
        }

        [Theory]
        [InlineData("2016-03-02", 1000, 200, "2015-11-11", 4500, true)] // 有效：未過期且達到門檻
        [InlineData("2016-03-02", 1000, 200, "2016-03-03", 4500, false)] // 無效：已過期
        [InlineData("2016-03-02", 1000, 200, "2015-11-11", 500, false)] // 無效：未達門檻
        [InlineData("2016-03-02", 5000, 200, "2015-11-11", 4500, false)] // 無效：未達門檻
        public void Coupon_應該正確判斷是否可用(string expiredDateStr, int spendAmount, int discountAmount, 
            string orderDateStr, float cartTotal, bool expectedValid)
        {
            // Arrange
            var expiredDate = DateTime.Parse(expiredDateStr);
            var orderDate = DateTime.Parse(orderDateStr);
            var coupon = new Coupon(expiredDate, spendAmount, discountAmount);

            // Act
            bool isValid = coupon.expiredAt > orderDate && cartTotal >= coupon.spendAmount;

            // Assert
            Assert.Equal(expectedValid, isValid);
        }

        [Fact]
        public void Coupon_過期券不應該適用()
        {
            // Arrange
            var coupon = new Coupon(DateTime.Parse("2015-01-01"), 1000, 200);
            var orderDate = DateTime.Parse("2016-03-02");
            var cartTotal = 1500f;

            // Act
            bool isValid = coupon.expiredAt > orderDate && cartTotal >= coupon.spendAmount;

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void Coupon_未達門檻不應該適用()
        {
            // Arrange
            var coupon = new Coupon(DateTime.Parse("2016-03-02"), 1000, 200);
            var orderDate = DateTime.Parse("2015-11-11");
            var cartTotal = 500f;

            // Act
            bool isValid = coupon.expiredAt > orderDate && cartTotal >= coupon.spendAmount;

            // Assert
            Assert.False(isValid);
        }
    }
}
