using BLL.Interfaces;
using DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Diagnostics;
using AI_Assistant_Project.Models;

namespace BLL.Services
{
    public class AIService : IAIService
    {
        private readonly IConfiguration _config;
        private readonly IRequestRepository _requestRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;

        public AIService(
            IConfiguration config,
            IRequestRepository requestRepository,
            IHttpContextAccessor httpContextAccessor,
            IUserRepository userRepository)
        {
            _config = config;
            _requestRepository = requestRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public async Task<Domain.Models.AiResponse> AskGroqAsync(AiRequest req)
        {
            if (await _requestRepository.AnyAsync(r => r.Prompt == req.Prompt))
            {
                var existing = await _requestRepository.FirstOrDefaultAsync(r => r.Prompt == req.Prompt);
                if (existing?.Response != null) return existing.Response;
            }

            var watch = Stopwatch.StartNew();
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config["AI:Groq:ApiKey"]}");

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[] { new { role = "user", content = req.Prompt } }
            };

            var httpResponse = await client.PostAsJsonAsync("https://api.groq.com/openai/v1/chat/completions", requestBody);
            var result = await httpResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            watch.Stop();

            string answer = httpResponse.IsSuccessStatusCode
                ? result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? ""
                : "Error: " + httpResponse.StatusCode;

            var aiResponse = new Domain.Models.AiResponse
            {
                Response = answer,
                Provider = "Grok (Llama 3.3)",
                IsSuccess = httpResponse.IsSuccessStatusCode,
                ExecutionTimeMs = watch.ElapsedMilliseconds,
                CreatedAt = DateTime.UtcNow,
                RequestId = req.Id
            };

            req.Response = aiResponse;
            await _requestRepository.CreateAsync(req);
            await _requestRepository.SaveChangeAsync();

            return aiResponse;
        }

        public async Task<Domain.Models.AiResponse> AskGeminiAsync(AiRequest req)
        {
            var watch = Stopwatch.StartNew();
            using var client = new HttpClient();
            var apiKey = _config["AI:Gemini:ApiKey"];
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[] { new { role = "user", parts = new[] { new { text = req.Prompt } } } }
            };

            var httpResponse = await client.PostAsJsonAsync(url, requestBody);
            var result = await httpResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            watch.Stop();

            string answer = httpResponse.IsSuccessStatusCode
                ? result.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? ""
                : "Error: " + httpResponse.StatusCode;

            var aiResponse = new Domain.Models.AiResponse
            {
                Response = answer,
                Provider = "Gemini 3 Flash",
                IsSuccess = httpResponse.IsSuccessStatusCode,
                ExecutionTimeMs = watch.ElapsedMilliseconds,
                CreatedAt = DateTime.UtcNow,
                RequestId = req.Id
            };

            req.Response = aiResponse;
            await _requestRepository.CreateAsync(req);
            await _requestRepository.SaveChangeAsync();

            return aiResponse;
        }
    }
}