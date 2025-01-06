namespace FurnitureShop.Utils
{
    public class CreateNameUrl
    {
        public static string CreateProductUrl(string productName)
        {
            return productName
                .ToLower()
                .Replace(" ", "-")
                .Replace("đ", "d")
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("ó", "o")
                .Replace("ú", "u")
                .Replace("í", "i")
                .Replace("ç", "c")
                .Replace("ã", "a")
                .Replace("ô", "o")
                .Replace("ê", "e")
                .Replace("ơ", "o")
                .Replace("ư", "u");
        }
    }
}
