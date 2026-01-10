using ShoppingCart.Models.Enums;

namespace ShoppingCart.Models.User
{
    /// <summary>
    /// 促銷活動
    /// </summary>
    public class Promotion
    {
        /// <summary>
        /// 到期日期
        /// </summary>
        public DateTime expiredAt { get; set; }

        /// <summary>
        /// 折扣比例
        /// </summary>
        public float discountPercent { get; set; }

        /// <summary>
        /// 產品類型
        /// </summary>
        public ProductType productType { get; set; }

    }
}
