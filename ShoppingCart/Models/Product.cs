using ShoppingCart.Models.Enums;

namespace ShoppingCart.Models
{
    /// <summary>
    /// 產品
    /// </summary>
    public class Product
    {
        public Product(string name, ProductType electric)
        {
            this.name = name;
            this.productType = electric;
        }

        /// <summary>
        /// 名稱
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 產品種類
        /// </summary>
        public ProductType productType { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        public float price { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        public int quantity { get; set; }
    }
}
