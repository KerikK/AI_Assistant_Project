using BLL.Interfaces;
using Domain.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AI_Assistant_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiControllers : ControllerBase
    {
        private readonly IAIService _aiService;

        public AiControllers(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("ask-Groq")]
        public async Task<IActionResult> AskOpenAi([FromBody] AiRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt is empty");
            var result = await _aiService.AskGroqAsync(request, "user1");
            return Ok(result);
        }
        [HttpPost("ask-gemini")]
        public async Task<IActionResult> AskGemini([FromBody] AiRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt is empty");
            var result = await _aiService.AskGeminiAsync(request, "user1");
            return Ok(result);
        }
    }
}
