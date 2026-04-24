using AI_Assistant_Project.Models;
using BLL.Interfaces;
using Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AIService : IAIService
    {
        private readonly IService<Request> _request;
        public AIService(IService<Request> request)
        {
            _request = request;
        }

        public async Task<AiResponseDto> ProcessAsync(AiRequestDto dto)
        {
            var result = new AiResponseDto();

            result.Response = "This is test response from AI";
            result.Provider = "OpenAI";
            result.FromCache = false;
            result.IsSuccess = true;
            result.CreatedAt = DateTime.UtcNow;

            return await Task.FromResult(result);


        }

        public async Task<AiResponseDto> ProcessSmartAsync(AiRequestDto dto)
        {
            return await ProcessAsync(dto);
        }

      
    }
}
