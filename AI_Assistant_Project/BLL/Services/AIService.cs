using AI_Assistant_Project.Models;
using BLL.Interfaces;
using DAL.Interfaces;
using Domain.DTO;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http.Json;
using System.Text.Json;

namespace BLL.Services
{
    public class AIService : IAIService
    {
        private readonly IConfiguration _config;
        private readonly IRequestRepository _requestRepository;
        public AIService(IConfiguration config, IRequestRepository requestRepository)
        {
            _config = config;
            _requestRepository = requestRepository;
        }

        public async Task<Response> AskGroqAsync(Request request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config["AI:Groq:ApiKey"]}");

            var requestBody = new
            {
                model = "llama-3.3-70b-versatile",
                messages = new[] { new { role = "user", content = request.Prompt } }
            };

            var httpResponse = await client.PostAsJsonAsync("https://api.groq.com/openai/v1/chat/completions", requestBody);
            var result = await httpResponse.Content.ReadFromJsonAsync<JsonElement>();
            watch.Stop();

            string answer = httpResponse.IsSuccessStatusCode
                ? result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? "No response"
                : "Error: " + httpResponse.StatusCode;

            int tokens = httpResponse.IsSuccessStatusCode
                ? result.GetProperty("usage").GetProperty("total_tokens").GetInt32()
                : 0;

            var response = new Response
            {
                Text = answer,
                Provider = "Grok (Llama 3.3)",
                IsSuccess = httpResponse.IsSuccessStatusCode,
                TokensUsed = tokens,
                ExecutionTimeMs = (int)watch.ElapsedMilliseconds,
                CreatedAt = DateTime.UtcNow,
                RequestId = request.Id
            };

            request.Response = response;

            await _requestRepository.CreateAsync(request);
            await _requestRepository.SaveChangeAsync();
            return response;

        }

        public async Task<Response> AskGeminiAsync(Request request)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            using var client = new HttpClient();

            var apiKey = _config["AI:Gemini:ApiKey"];
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3-flash-preview:generateContent?key={apiKey}";

            var requestBody = new
            {
                contents = new[] { new { role = "user", parts = new[] { new { text = request.Prompt } } } }
            };

            var httpResponse = await client.PostAsJsonAsync(url, requestBody);
            var result = await httpResponse.Content.ReadFromJsonAsync<JsonElement>();
            watch.Stop();

            string answer = httpResponse.IsSuccessStatusCode
                ? result.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? "No response"
                : "Error: " + httpResponse.StatusCode;

            int tokens = httpResponse.IsSuccessStatusCode
                ? result.GetProperty("usageMetadata").GetProperty("totalTokenCount").GetInt32()
                : 0;

            var response = new Response
            {
                Text = answer,
                Provider = "Gemini 3 Flash",
                IsSuccess = httpResponse.IsSuccessStatusCode,
                TokensUsed = tokens,
                ExecutionTimeMs = (int)watch.ElapsedMilliseconds,
                CreatedAt = DateTime.UtcNow,
                RequestId = request.Id
            };

            request.Response = response;

            await _requestRepository.CreateAsync(request);
            await _requestRepository.SaveChangeAsync();

            return response;
        }
    }
}