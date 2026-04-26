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
    public class GrokService : AskAiService
    {
        public GrokService(IConfiguration config, IRequestRepository requestRepo,
            IHttpContextAccessor httpContext, IUserRepository userRepo, IMemoryCache cache) :
            base(config, requestRepo, httpContext, userRepo, cache) { }

        public override async Task<AiResponse> Ask(AiRequest req)
        {
            string cacheKey = $"grok_{req.Prompt.GetHashCode()}";
            if (_cache.TryGetValue(cacheKey, out AiResponse? cached)) return cached!;

            var watch = Stopwatch.StartNew();
            using var client = new HttpClient();

            var apiKey = _config["AI:Grok:ApiKey"];
            var model = _config["AI:Grok:Model"] ?? "llama-3.3-70b-versatile";
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var body = new { model = model, messages = new[] { new { role = "user", content = req.Prompt } } };

            var response = await client.PostAsJsonAsync("https://api.groq.com/openai/v1/chat/completions", body);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return BuildResponse("Invalid API key", "Grok", false, 0, (int)watch.ElapsedMilliseconds, req.Id);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                return BuildResponse("API rate limit exceeded", "Grok", false, 0, (int)watch.ElapsedMilliseconds, req.Id);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            watch.Stop();

            if (!response.IsSuccessStatusCode)
                return BuildResponse("Service error", "Grok", false, 0, (int)watch.ElapsedMilliseconds, req.Id);

            string text = json.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString()!;
            int tokens = json.GetProperty("usage").GetProperty("total_tokens").GetInt32();

            var result = BuildResponse(text, $"Grok ({model})", true, tokens, (int)watch.ElapsedMilliseconds, req.Id);
            await SaveResultAsync(req, result);
            return _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
        }
    }
}
