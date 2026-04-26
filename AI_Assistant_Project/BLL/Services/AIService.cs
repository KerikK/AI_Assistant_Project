using AI_Assistant_Project.Models;
using BLL.Interfaces;
using BLL.Services.LLMServices;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AIService : IAIService
    {
        private readonly GrokService _grok;
        private readonly GeminiService _gemini;
        public AIService(GrokService grokService, GeminiService geminiService)
        {
            _grok = grokService;
            _gemini = geminiService;
        }

        public Task<AiResponse> AskGeminiAsync(AiRequest req)
            => _gemini.Ask(req);

        public Task<AiResponse> AskGrokAsync(AiRequest req)
            => _grok.Ask(req);
    }
}
