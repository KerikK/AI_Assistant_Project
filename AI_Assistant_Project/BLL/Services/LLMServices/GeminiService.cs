using AI_Assistant_Project.Models;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Services.LLMServices
{
    public class GeminiService : AskAiService
    {
        public GeminiService(IConfiguration config, IRequestRepository requestRepo,
        IHttpContextAccessor httpContext, IUserRepository userRepo, IMemoryCache cache) :
        base(config, requestRepo, httpContext, userRepo, cache) { }

        public async override Task<AiResponse> Ask(AiRequest req)
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
    }
}
