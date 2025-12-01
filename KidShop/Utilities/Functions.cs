using System.Security.Cryptography;
using System.Text;

namespace KidShop.Utilities
{
    public class Functions
    {
        public static string TitleSlugGeneration(string type, string title, long id)
        {
            // Nếu title null thì gán thành chuỗi rỗng hoặc "no-title"
            title = title ?? "";

            return type + "-" + SlugGenerator.SlugGenerator.GenerateSlug(title) + "-" + id.ToString() + ".html";
        }
        public static string getCurrentDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string MD5Hash(string text)
        {
            using (var md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(text);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder strBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    strBuilder.Append(hashBytes[i].ToString("x2"));
                }
                return strBuilder.ToString();
            }
        }
        public static string MD5Password(string? text)
        {
            string str = MD5Hash(text);
            //Lặp thêm 5 lần mã hóa xâu đảm bảo tính bảo mật //Mỗi lần lặp nhân đôi xâu mã hóa, ở giữa thêm " "
            //Có thể làm các cách khác để tăng tính bảo mật ở đây
            for (int i = 0; i <= 5; i++)
                str = MD5Hash(str + "_" + str);
            return str;
        }
        public static string ToSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "no-title";

            string normalized = text.ToLower().Trim();
            normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
            normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"\s+", "-");
            return normalized;
        }
    }
}
