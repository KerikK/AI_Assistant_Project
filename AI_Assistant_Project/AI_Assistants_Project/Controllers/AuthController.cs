using BLL.Interfaces;
using Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AI_Assistants_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var res = await authService.RegisterAsync(request);
            if (!res.Success)
                return BadRequest(new { res.Message });
            return Ok(res.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var res = await authService.LoginAsync(request);
            if (!res.Success)
                return BadRequest(new { res.Message });
            return Ok(res.response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var res = await authService.RefreshTokenAsync(request.RefreshToken);
            if (!res.Success)
                return BadRequest(new { res.Message });

            return Ok(res.response);
        }

        [HttpGet("Test")]
        [Authorize]
        public IActionResult Test()
        {
            return Ok("Success");
        }
    }
}
