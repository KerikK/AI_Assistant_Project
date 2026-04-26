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
        private readonly ILogger<AiControllers> _logger;

        public AiControllers(IAIService aiService, ILogger<AiControllers> logger)
        {
            _aiService = aiService;
            _logger = logger;
        }

      
        [HttpPost("AskGrok")]
        [Authorize]
        public async Task<IActionResult> AskGrok([FromBody] AiRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Prompt))
            {
                _logger.LogError("Prompt is empty");
                return BadRequest("Prompt cannot be empty.");
            }

            var requestEntity = new AiRequest
            {
                Prompt = dto.Prompt,
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var result = await _aiService.AskGrokAsync(requestEntity);
                _logger.LogInformation($"Grok was asked: {dto.Prompt}");
                return Ok(result.Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.InnerException?.Message ?? "An error occurred while asking Grok");
                return StatusCode(500, $"An error occurred during the Grok request: {ex.Message}");
            }
        }

      
        [HttpPost("AskGemini")]
        [Authorize]
        public async Task<IActionResult> AskGemini([FromBody] AiRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Prompt))
            {
                _logger.LogError("Prompt is empty");
                return BadRequest("Prompt cannot be empty.");
            }

            var requestEntity = new AiRequest
            {
                Prompt = dto.Prompt,
                UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var result = await _aiService.AskGeminiAsync(requestEntity);
                _logger.LogInformation($"Gemini was asked: {dto.Prompt}");
                return Ok(result.Response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.InnerException?.Message ?? "An error occurred while asking Gemini");
                return StatusCode(500, $"An error occurred during the Gemini request: {ex.Message}");
            }
        }
    }
}