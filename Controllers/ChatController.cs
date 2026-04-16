using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace YourProjectName.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly string _apiKey = "INSERT_KEY_HERE";

        [HttpPost("ask")]
        public async Task<IActionResult> AskChatbot([FromBody] ChatRequest request)
        {
            if (string.IsNullOrEmpty(request.Message)) return BadRequest();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            var payload = new
            {
                model = "llama3-8b-8192",
                messages = new[] {
                    new { role = "system", content = "You are a helpful assistant for SER Driving School. Answer questions about manual/automatic lessons and booking." },
                    new { role = "user", content = request.Message }
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://api.groq.com/openai/v1/chat/completions", content);
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "The AI Assistant is currently offline.");
            }
        }
    }

    public class ChatRequest { public string Message { get; set; } }
}