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
    public class AIService : Service<AiRequest>, IAIService
    {
        private readonly IConfiguration _config;
        private readonly IRequestRepository _requestRepo;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepo;
        private readonly IMemoryCache _cache;
        private const int DailyTokenLimit = 100000;

        public AIService(IConfiguration config, IRequestRepository requestRepo,
            IHttpContextAccessor httpContext, IUserRepository userRepo, IMemoryCache cache) : base(requestRepo)
        {
            _config = config;
            _requestRepo = requestRepo;
            _httpContext = httpContext;
            _userRepo = userRepo;
            _cache = cache;
        }

        public async Task<AiResponse> AskGroqAsync(AiRequest req)
        {
            string cacheKey = $"groq_{req.Prompt.GetHashCode()}";
            if (_cache.TryGetValue(cacheKey, out AiResponse? cached)) return cached!;

            var watch = Stopwatch.StartNew();
            using var client = new HttpClient();

            var apiKey = _config["AI:Groq:ApiKey"];
            var model = _config["AI:Groq:Model"] ?? "llama-3.3-70b-versatile";
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var body = new { model = model, messages = new[] { new { role = "user", content = req.Prompt } } };

            var response = await client.PostAsJsonAsync("https://api.groq.com/openai/v1/chat/completions", body);
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            watch.Stop();

            if (!response.IsSuccessStatusCode)
                return BuildResponse("Service error", "Groq", false, 0, (int)watch.ElapsedMilliseconds, req.Id);

            string text = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()!;
            int tokens = json.GetProperty("usage").GetProperty("total_tokens").GetInt32();

            await CalculateRemainingTokensAsync(GetCurrentUserId(), tokens);

            var result = BuildResponse(text, $"Groq ({model})", true, tokens, (int)watch.ElapsedMilliseconds, req.Id);
            await SaveResultAsync(req, result);
            return _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
        }

        public async Task<AiResponse> AskGeminiAsync(AiRequest req)
        {
            string cacheKey = $"gemini_{req.Prompt.GetHashCode()}";
            if (_cache.TryGetValue(cacheKey, out AiResponse? cached)) return cached!;

            var watch = Stopwatch.StartNew();
            using var client = new HttpClient();

            var apiKey = _config["AI:Gemini:ApiKey"];
            var model = _config["AI:Gemini:Model"] ?? "gemini-3-flash-preview";

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";
            var body = new { contents = new[] { new { parts = new[] { new { text = req.Prompt } } } } };

            var response = await client.PostAsJsonAsync(url, body);
            watch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                return BuildResponse($"Error: {response.StatusCode}. Details: {errorDetails}", "Gemini", false, 0, (int)watch.ElapsedMilliseconds, req.Id);
            }

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            string text = json.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString()!;
            int tokens = json.GetProperty("usageMetadata").GetProperty("totalTokenCount").GetInt32();

            await CalculateRemainingTokensAsync(GetCurrentUserId(), tokens);

            var result = BuildResponse(text, $"Gemini ({model})", true, tokens, (int)watch.ElapsedMilliseconds, req.Id);
            await SaveResultAsync(req, result);
            return _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
        }

        private async Task<int> CalculateRemainingTokensAsync(int userId, int currentUsage)
        {
            var requestsToday = await _requestRepo.GetByUserIdAsync(userId);
            int spentToday = requestsToday
                .Where(r => r.CreatedAt.Date == DateTime.UtcNow.Date && r.Response != null)
                .Sum(r => r.Response.TokensUsed);

            return Math.Max(0, DailyTokenLimit - spentToday - currentUsage);
        }

        private int GetCurrentUserId()
        {
            var claim = _httpContext.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : -1;
        }

        private AiResponse BuildResponse(string msg, string prov, bool ok, int t, int ms, int id) => new()
        {
            Response = msg,
            Provider = prov,
            IsSuccess = ok,
            TokensUsed = t,
            ExecutionTimeMs = ms,
            CreatedAt = DateTime.UtcNow,
            RequestId = id
        };

        private async Task SaveResultAsync(AiRequest req, AiResponse res)
        {
            req.Response = res;
            var user = await _userRepo.GetAsync(GetCurrentUserId());
            if (user != null) user.Requests.Add(req);

            await _requestRepo.CreateAsync(req);
            await _requestRepo.SaveChangeAsync();
        }
    }
}