using AI_Assistant_Project.Controllers;
using BLL.Interfaces;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AI_Assistants_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, ILogger<AuthController> _logger) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var res = await authService.RegisterAsync(request);
            if (!res.Success)
            {
                _logger.LogError(res.Message);
                return BadRequest(new { res.Message });
            }
            _logger.LogInformation("Registered successfully");
            return Ok(res.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var res = await authService.LoginAsync(request);
            if (!res.Success)
            {
                _logger.LogError(res.Message);
                return BadRequest(new { res.Message });
            }
            _logger.LogInformation("Logined in successfully");
            return Ok(res.response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var res = await authService.RefreshTokenAsync(request.RefreshToken);
            if (!res.Success)
            {
                _logger.LogError(res.Message);
                return BadRequest(new { res.Message });
            }
            _logger.LogInformation("Token refreshed successfully");
            return Ok(res.response);
        }
    }
}
