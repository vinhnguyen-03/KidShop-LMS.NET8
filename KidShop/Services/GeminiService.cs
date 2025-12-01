using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace KidShop.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiApi:ApiKey"]
                      ?? throw new ArgumentNullException("GeminiApi:ApiKey", "Gemini API key is missing in configuration.");
        }

        private async Task<string> CallGeminiAsync(string prompt)
        {
            //  Sử dụng endpoint đúng cho Gemini 2.0 Flash
            string endpoint = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return $"❌ Lỗi HTTP {response.StatusCode}: {responseString}";
            }

            try
            {
                var jsonResponse = JObject.Parse(responseString);
                var text = jsonResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                return string.IsNullOrEmpty(text)
                    ? "❌ Không có phản hồi từ Gemini."
                    : text.Trim();
            }
            catch (Exception ex)
            {
                return $"❌ Lỗi xử lý phản hồi: {ex.Message}\n{responseString}";
            }
        }

        public Task<string> SummarizeAsync(string inputText)
        {
            string prompt = $"Tóm tắt đoạn văn dưới đây thành 3-5 câu, bằng tiếng Việt, rõ ràng và tự nhiên:\n\n{inputText}";
            return CallGeminiAsync(prompt);
        }

        public Task<string> AskQuestionAsync(string contextText, string question)
        {
            string prompt = $"Dựa trên nội dung sau:\n{contextText}\n\nHãy trả lời ngắn gọn và dễ hiểu cho câu hỏi: {question}";
            return CallGeminiAsync(prompt);
        }
        public Task<string> AskExpertAsync(string contextText, string userQuestion)
        {
            string prompt = $@"
                Bạn là một chuyên gia tư vấn về trẻ bị chậm nói và sản phẩm hỗ trợ. Hãy trả lời người dùng một cách chuyên nghiệp, ngắn gọn và dễ hiểu.
                Ngữ cảnh hiện tại: {contextText}
                Câu hỏi của khách hàng: {userQuestion}
                Trả lời dưới dạng câu trả lời dành cho khách hàng, rõ ràng, thân thiện, chuyên nghiệp.";

            return CallGeminiAsync(prompt);
        }
    }
}
