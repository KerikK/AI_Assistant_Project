using AI_Assistant_Project.Models;
using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;

namespace BLL.Services
{
    public abstract class AskAiService
    {
        protected readonly IConfiguration _config;
        protected readonly IRequestRepository _requestRepo;
        protected readonly IHttpContextAccessor _httpContext;
        protected readonly IUserRepository _userRepo;
        protected readonly IMemoryCache _cache;
        protected const int DailyTokenLimit = 100000;

        public AskAiService(IConfiguration config, IRequestRepository requestRepo,
            IHttpContextAccessor httpContext, IUserRepository userRepo, IMemoryCache cache)
        {
            _config = config;
            _requestRepo = requestRepo;
            _httpContext = httpContext;
            _userRepo = userRepo;
            _cache = cache;
        }

        public abstract Task<AiResponse> Ask(AiRequest req);

        protected async Task<int> CalculateRemainingTokensAsync(int userId, int currentUsage)
        {
            var requestsToday = await _requestRepo.GetByUserIdAsync(userId);
            int spentToday = requestsToday
                .Where(r => r.CreatedAt.Date == DateTime.UtcNow.Date && r.Response != null)
                .Sum(r => r.Response.TokensUsed);

            return Math.Max(0, DailyTokenLimit - spentToday - currentUsage);
        }

        protected int GetCurrentUserId()
        {
            var claim = _httpContext.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : -1;
        }

        protected AiResponse BuildResponse(string msg, string prov, bool ok, int t, int ms, int id) => new()
        {
            Response = msg,
            Provider = prov,
            IsSuccess = ok,
            TokensUsed = t,
            ExecutionTimeMs = ms,
            CreatedAt = DateTime.UtcNow,
            RequestId = id
        };

        protected async Task SaveResultAsync(AiRequest req, AiResponse res)
        {
            req.Response = res;
            var user = await _userRepo.GetAsync(GetCurrentUserId());
            if (user != null) user.Requests.Add(req);

            await _requestRepo.CreateAsync(req);
            await _requestRepo.SaveChangeAsync();
        }
    }
}