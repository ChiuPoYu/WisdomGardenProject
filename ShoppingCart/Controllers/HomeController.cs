using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Models;
using ShoppingCart.Models.Enums;
using ShoppingCart.Models.User;
using System.Diagnostics;

namespace ShoppingCart.Controllers
{
    public class HomeController : Controller
    {
        private readonly List<Product> _products = new List<Product>
        {
            new Product("ipad", ProductType.Electric),
            new Product("iphone", ProductType.Electric),
            new Product("螢幕", ProductType.Electric),
            new Product("筆記型電腦", ProductType.Electric),
            new Product("鍵盤", ProductType.Electric),
            new Product("麵包", ProductType.Food),
            new Product("餅乾", ProductType.Food),
            new Product("蛋糕", ProductType.Food),
            new Product("牛肉", ProductType.Food),
            new Product("魚", ProductType.Food),
            new Product("蔬菜", ProductType.Food),
            new Product("餐巾紙", ProductType.LifeUsed),
            new Product("收納箱", ProductType.LifeUsed),
            new Product("咖啡杯", ProductType.LifeUsed),
            new Product("雨傘", ProductType.LifeUsed),
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
                var order = new Order();
                var cart = new Cart();
                
                var separators = new string[] { Environment.NewLine, "\r\n", "\r", "\n" };
                var productLines = productsInput.Split(separators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                
                foreach (var line in productLines)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split('*', StringSplitOptions.TrimEntries);
                    if (parts.Length != 2)
                        continue;

                    if (!int.TryParse(parts[0], out int quantity))
                        continue;
                        
                    var productInfo = parts[1].Split(':', StringSplitOptions.TrimEntries);
                    if (productInfo.Length != 2)
                        continue;

                    string productName = productInfo[0];
                    if (!float.TryParse(productInfo[1], out float price))
                        continue;

                    var product = _products.FirstOrDefault(p => p.name == productName);
                    if (product != null)
                    {
                        var productCopy = new Product(product.name, product.productType)
                        {
                            price = price,
                            quantity = quantity
                        };
                        cart.products.Add(productCopy);
                    }
                }

                if (!string.IsNullOrWhiteSpace(couponInput))
                {
                    var couponParts = couponInput.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (couponParts.Length == 3 &&
                        DateTime.TryParse(couponParts[0].Replace('.', '/'), out DateTime couponExpired) &&
                        int.TryParse(couponParts[1], out int spendAmount) &&
                        int.TryParse(couponParts[2], out int discountAmount))
                    {
                        cart.coupon = new Coupon(couponExpired, spendAmount, discountAmount);
                    }
                }

                if (DateTime.TryParse(orderDate.Replace('.', '/'), out DateTime createdAt))
                {
                    order.createdAt = createdAt;
                }
                else
                {
                    order.createdAt = DateTime.Now;
                }

                if (!string.IsNullOrWhiteSpace(promotionInput))
                {
                    var promotionParts = promotionInput.Trim().Split('|', StringSplitOptions.RemoveEmptyEntries);
                    if (promotionParts.Length == 3 &&
                        DateTime.TryParse(promotionParts[0].Replace('.', '/'), out DateTime promoExpired) &&
                        float.TryParse(promotionParts[1], out float discount))
                    {
                        var productType = promotionParts[2] switch
                        {
                            "電子" => ProductType.Electric,
                            "食品" => ProductType.Food,
                            "日用品" => ProductType.LifeUsed,
                            "酒類" => ProductType.Alcohol,
                            _ => (ProductType?)null
                        };
                        
                        if (productType.HasValue)
                        {
                            order.promotion = new Promotion
                            {
                                expiredAt = promoExpired,
                                discountPercent = discount,
                                productType = productType.Value
                            };
                        }
                    }
                }

                order.cart = cart;

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
