using BLL.Interfaces;
using Domain.DTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AIService : IAIService
    {
        private readonly IConfiguration _config;
        public AIService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<AiResponseDto> AskGrokAsync(AiRequestDto dto)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config["AI:Grok:ApiKey"]}");

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[] { new { role = "user", content = dto.Prompt } }
            };

            var response = await client.PostAsJsonAsync("https://api.groq.com/openai/v1/chat/completions", requestBody);
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            watch.Stop();

            var isSuccess = response.IsSuccessStatusCode;

            string answer = "Request error";

            if (response.IsSuccessStatusCode)
                answer = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() 
                    ?? "No response provided";

            return new AiResponseDto
            {
                Response = answer,
                Provider = "Grok (Llama 3.3)",
                IsSuccess = response.IsSuccessStatusCode,
                ExecutionTimeMs = watch.ElapsedMilliseconds,
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<AiResponseDto> AskGeminiAsync(AiRequestDto dto)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            using var client = new HttpClient();

            var apiKey = _config["AI:Gemini:ApiKey"];
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[] { new { role = "user", parts = new[] { new { text = dto.Prompt } } } },
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

            return new AiResponseDto
            {
                Response = answer,
                Provider = "Gemini 3 Flash",
                IsSuccess = response.IsSuccessStatusCode,
                ExecutionTimeMs = watch.ElapsedMilliseconds,
                CreatedAt = DateTime.UtcNow
            };
         
        }
    }
}
