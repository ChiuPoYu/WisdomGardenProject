using ShoppingCart.Models.Enums;

namespace ShoppingCart.Models.User
{
    /// <summary>
    /// 折價券
    /// </summary>
    public class Coupon
    {
        public Coupon(DateTime dateTime, int spendAmount, int discountAmount)
        {
            this.expiredAt = dateTime;
            this.spendAmount = spendAmount;
            this.discountAmount = discountAmount;
        }

        /// <summary>
        /// 折扣期限
        /// </summary>
        public DateTime expiredAt { get; set; }

        /// <summary>
        /// 消費金額門檻
        /// </summary>
        public int spendAmount { get; set; }

        /// <summary>
        /// 折扣金額
        /// </summary>
        public int discountAmount { get; set; }
    }
}
