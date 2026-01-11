using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using ShoppingCart.Models.Enums;
using ShoppingCart.Models.User;
using System.Diagnostics;

namespace ShoppingCart.Controllers
{
    public class HomeController : Controller
    {
        // 建立DB假資料
        private readonly List<Product> _products = new List<Product>
        {
            // 電子產品
            new Product("ipad", ProductType.Electric),
            new Product("iphone", ProductType.Electric),
            new Product("螢幕", ProductType.Electric),
            new Product("筆記型電腦", ProductType.Electric),
            new Product("鍵盤", ProductType.Electric),
            
            // 食品
            new Product("麵包", ProductType.Food),
            new Product("餅乾", ProductType.Food),
            new Product("蛋糕", ProductType.Food),
            new Product("牛肉", ProductType.Food),
            new Product("魚", ProductType.Food),
            new Product("蔬菜", ProductType.Food),
            
            // 日用品
            new Product("餐巾紙", ProductType.LifeUsed),
            new Product("收納箱", ProductType.LifeUsed),
            new Product("咖啡杯", ProductType.LifeUsed),
            new Product("雨傘", ProductType.LifeUsed),
            
            // 酒類
            new Product("啤酒", ProductType.Alcohol),
            new Product("白酒", ProductType.Alcohol),
            new Product("伏特加", ProductType.Alcohol)
        };

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Checkout(string promotionInput, string productsInput, string orderDate, string? couponInput)
        {
            try
            {
                // 建立訂單
                Order order = new Order();

                #region 解析購物車商品
                var cart = new Cart();
                var productLines = productsInput.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                
                foreach (var line in productLines)
                {
                    var trimmedLine = line.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedLine))
                        continue;

                    // 格式：數量*商品:單價
                    var parts = trimmedLine.Split('*');
                    if (parts.Length != 2)
                        continue;

                    int quantity = int.Parse(parts[0].Trim());
                    var productInfo = parts[1].Split(':');
                    if (productInfo.Length != 2)
                        continue;

                    string productName = productInfo[0].Trim();
                    float price = float.Parse(productInfo[1].Trim());

                    // 根據商品名稱判斷類型（簡化處理）
                    var product = _products.FirstOrDefault(p => p.name == productName);
                    if (product != null)
                    {
                        product.price = price;
                        product.quantity = quantity;
                        cart.products.Add(product);
                    }
                }
                #endregion

                #region 解析折價券
                if (!string.IsNullOrWhiteSpace(couponInput))
                {
                    var couponParts = couponInput.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (couponParts.Length == 3)
                    {
                        DateTime expiredAt = DateTime.Parse(couponParts[0].Replace('.', '/'));
                        int spendAmount = int.Parse(couponParts[1]);
                        int discountAmount = int.Parse(couponParts[2]);
                        cart.coupon = new Coupon(expiredAt, spendAmount, discountAmount);
                    }
                }
                #endregion

                #region 解析結算日期
                DateTime createdAt = DateTime.Parse(orderDate.Replace('.', '/'));
                #endregion

                #region 解析促銷活動
                if (!string.IsNullOrWhiteSpace(promotionInput))
                {
                    var promotionParts = promotionInput.Trim().Split('|', StringSplitOptions.RemoveEmptyEntries);
                    if (promotionParts.Length == 3)
                    {
                        DateTime expiredAt = DateTime.Parse(promotionParts[0].Replace('.', '/'));
                        float discount = float.Parse(promotionParts[1]);
                        order.promotion = new Promotion
                        {
                            expiredAt = expiredAt,
                            discountPercent = discount,
                            productType = promotionParts[2] switch
                            {
                                "電子" => ProductType.Electric,
                                "食品" => ProductType.Food,
                                "日用品" => ProductType.LifeUsed,
                                "酒類" => ProductType.Alcohol,
                                _ => throw new ArgumentException($"未知的商品類型: {promotionParts[2]}")
                            }
                        };
                    }
                }
                #endregion

                order.cart = cart;
                order.createdAt = createdAt;

                return View("OrderConfirmation", order);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"輸入格式錯誤：{ex.Message}";
                return View("Index");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
