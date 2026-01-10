using System.ComponentModel;

namespace ShoppingCart.Models.Enums
{
    /// <summary>
    /// 產品類別
    /// </summary>
    public enum ProductType
    {
        /// <summary>
        /// 食品 = 1
        /// </summary>
        [Description("食品")]
        食品 = 1,

        /// <summary>
        /// 電子 = 2
        /// </summary>
        [Description("電子")]
        電子 = 2,

        /// <summary>
        /// 日用品 = 2
        /// </summary>
        [Description("日用品")]
        日用品 = 3,

        /// <summary>
        /// 酒類 = 2
        /// </summary>
        [Description("酒類")]
        酒類 = 4 
    }
}
