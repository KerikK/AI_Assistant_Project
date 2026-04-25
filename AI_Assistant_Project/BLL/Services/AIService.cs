using AI_Assistant_Project.Models;
using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Identity;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AIService : Service<AiRequest>, IAIService
    {
        private readonly IConfiguration _config;
        private readonly IRequestRepository _requestRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRepository _userRepository;
        public AIService(IConfiguration config, IRequestRepository requestRepository, 
            IHttpContextAccessor httpContextAccessor, IUserRepository userRepository) : base(requestRepository)
        {
            _config = config;
            _requestRepository = requestRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        public async Task<AiResponse> AskGrokAsync(AiRequest req)
        {
            if (await _requestRepository.AnyAsync(r => r.Prompt == req.Prompt))
                return (await _requestRepository.FirstOrDefaultAsync(r => r.Prompt == req.Prompt) ??
                    new() { Response = new() { Response = "Response Error: incorrect request from db" } }).Response;

            string test = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1";

            var user = await _userRepository.GetAsync(
                int.Parse(test));

            var watch = System.Diagnostics.Stopwatch.StartNew();
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config["AI:Grok:ApiKey"]}");

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[] { new { role = "user", content = req.Prompt } }
            };

            var response = await client.PostAsJsonAsync("https://api.groq.com/openai/v1/chat/completions", requestBody);
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            watch.Stop();

            var isSuccess = response.IsSuccessStatusCode;

            string answer = "Request error";

            if (response.IsSuccessStatusCode)
                answer = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() 
                    ?? "No response provided";

            var aiResponse = new AiResponse
            {
                Response = answer,
                Provider = "Grok (Llama 3.3)",
                IsSuccess = response.IsSuccessStatusCode,
                ExecutionTimeMs = watch.ElapsedMilliseconds,
                CreatedAt = DateTime.UtcNow
            };
            req.Response = aiResponse;

            user?.Requests.Add(req);
            await _requestRepository.CreateAsync(req);
            await _requestRepository.SaveChangeAsync();

            return aiResponse;
        }

        public async Task<AiResponse> AskGeminiAsync(AiRequest req)
        {
            if (await _requestRepository.AnyAsync(r => r.Prompt == req.Prompt))
                return (await _requestRepository.FirstOrDefaultAsync(r => r.Prompt == req.Prompt) ??
                    new() { Response = new() { Response = "Response Error: incorrect request from db" } }).Response;

            var user = await _userRepository.GetAsync(
                int.Parse(_httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "-1"));

            var watch = System.Diagnostics.Stopwatch.StartNew();
            using var client = new HttpClient();

            var apiKey = _config["AI:Gemini:ApiKey"];
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[] { new { role = "user", parts = new[] { new { text = req.Prompt } } } },
                generationConfig = new { thinkingConfig = new { thinkingLevel = "HIGH" } }
            };

            var response = await client.PostAsJsonAsync(url, requestBody);
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            watch.Stop();
            var isSuccess = response.IsSuccessStatusCode;

            string answer = "Request error";
            if (response.IsSuccessStatusCode)
                answer = result.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString()
                    ?? "No response provided";

            var aiResponse = new AiResponse
            {
                Response = answer,
                Provider = "Gemini 3 Flash",
                IsSuccess = response.IsSuccessStatusCode,
                ExecutionTimeMs = watch.ElapsedMilliseconds,
                CreatedAt = DateTime.UtcNow
            };
            req.Response = aiResponse;

            user?.Requests.Add(req);
            await _requestRepository.CreateAsync(req);
            await _requestRepository.SaveChangeAsync();

            return aiResponse;
        }
    }
}
