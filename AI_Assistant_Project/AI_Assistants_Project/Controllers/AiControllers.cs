using BLL.Interfaces;
using Domain.DTO;
using AI_Assistant_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        public async Task<IActionResult> AskGroq([FromBody] AiRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Prompt))
                return BadRequest("Prompt cannot be empty.");

            var requestEntity = new AiRequest
            {
                Prompt = dto.Prompt,
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var result = await _aiService.AskGroqAsync(requestEntity);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during the Groq request: {ex.Message}");
            }
        }

      
        [HttpPost("AskGemini")]
        [Authorize]
        public async Task<IActionResult> AskGemini([FromBody] AiRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Prompt))
                return BadRequest("Prompt cannot be empty.");

            var requestEntity = new AiRequest
            {
                Prompt = dto.Prompt,
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var result = await _aiService.AskGeminiAsync(requestEntity);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred during the Gemini request: {ex.Message}");
            }
        }
    }
}