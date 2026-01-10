namespace ShoppingCart.Models
{
    public static class MyDB
    {
        public static List<Dictionary<string, string>> DefaultFakeData = new List<Dictionary<string, string>>()
        {
            new Dictionary<string, string>()
            {
                { "Id", "1" },
                { "Name", "Apple" },
                { "Price", "0.50" }
            },
            new Dictionary<string, string>()
            {
                { "Id", "2" },
                { "Name", "Banana" },
                { "Price", "0.30" }
            },
            new Dictionary<string, string>()
            {
                { "Id", "3" },
                { "Name", "Orange" },
                { "Price", "0.80" }
            }
        };  
    }
}
