namespace ShoppingCart.Models.User
{
    public class Order
    {
        /// <summary>
        /// 消費內容(購物車)
        /// </summary>
        public Cart cart { get; set; }

        /// <summary>
        /// 消費日期
        /// </summary>
        public DateTime createdAt { get; set; }

        /// <summary>
        /// 促銷活動
        /// </summary>
        public Promotion? promotion { get; set; }
    }
}
