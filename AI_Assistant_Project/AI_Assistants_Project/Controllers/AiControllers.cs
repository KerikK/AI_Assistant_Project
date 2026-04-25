using AI_Assistant_Project.Models;
using BLL.Interfaces;
using Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [HttpPost("AskGroq")]
        public async Task<IActionResult> AskGroq([FromBody] AiRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Prompt))
                return BadRequest("Prompt is empty");

            var requestEntity = new Request
            {
                Prompt = dto.Prompt,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous",
                CreatedAt = DateTime.UtcNow
            };

            var result = await _aiService.AskGroqAsync(requestEntity);
            return Ok(result);
        }

        [HttpPost("AskGemini")]
        public async Task<IActionResult> AskGemini([FromBody] AiRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Prompt))
                return BadRequest("Prompt is empty");

            var requestEntity = new Request
            {
                Prompt = dto.Prompt,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous",
                CreatedAt = DateTime.UtcNow
            };

            var result = await _aiService.AskGeminiAsync(requestEntity);
            return Ok(result);
        }
    }
}