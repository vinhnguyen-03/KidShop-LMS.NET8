using KidShop.Services;
using Microsoft.AspNetCore.Mvc;

namespace KidShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : Controller
    {
        private readonly GeminiService _gemini;
        public ChatbotController(GeminiService gemini)
        {
            _gemini = gemini;
        }
        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Câu hỏi không được để trống.");

            string context = request.Context ?? "";
            string reply = await _gemini.AskExpertAsync(context, request.Prompt);

            return Ok(new { reply });
        }

        public class ChatRequest
        {
            public string Prompt { get; set; }
            public string Context { get; set; }
        }
    }
}
