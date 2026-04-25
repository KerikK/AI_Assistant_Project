using AI_Assistant_Project.Models;
using BLL.Interfaces;
using DAL.Interfaces;
using Domain.DTO;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("AskGrok")]
        [Authorize]
        public async Task<IActionResult> AskOpenAi([FromBody] AirRequestDto request)
        {   
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt is empty");
            var result = await _aiService.AskGrokAsync(new() { Prompt = request.Prompt } );
            return Ok(result);
        }

        [HttpPost("AskGemini")]
        [Authorize]
        public async Task<IActionResult> AskGemini([FromBody] AirRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest("Prompt is empty");
            var result = await _aiService.AskGeminiAsync(new() { Prompt = request.Prompt });
            return Ok(result);
        }
    }
}
